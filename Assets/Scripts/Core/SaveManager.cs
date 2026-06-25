using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string SavePath
    {
        get
        {
            if (AuthManager.Instance == null || !AuthManager.Instance.IsLoggedIn)
                return string.Empty;

            string safeUserId = GetSafeFileName(AuthManager.Instance.UserId);
            return Path.Combine(Application.persistentDataPath, $"save_{safeUserId}.json");
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSave() => !string.IsNullOrEmpty(SavePath) && File.Exists(SavePath);

    public void Save()
    {
        SaveData data = CreateSaveData();
        WriteLocalSave(data);
    }

    public void Load()
    {
        if (!TryReadLocalSave(out SaveData data))
        {
            Debug.LogWarning("[SaveManager] 저장 파일 없음");
            return;
        }

        ApplySaveData(data);

        Debug.Log("[SaveManager] 불러오기 완료");
    }

    public void DeleteSave()
    {
        if (HasSave())
            File.Delete(SavePath);
    }


    private string GetSafeFileName(string value)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
            value = value.Replace(invalidChar, '_');

        return value;
    }
    private SaveData CreateSaveData()
    {
        var data = new SaveData();

        data.lastSavedAtUtcTicks = System.DateTime.UtcNow.Ticks;
        data.totalGold = GoldManager.Instance.TotalGold;

        data.currentRank = TruckRankManager.instance.CurrentRank;
        data.totalExp = TruckRankManager.instance.TotalExp;
        data.cookingGuideDone = CookingGuideManager.IsGuideDone();

        data.cookSlotLevel = UpgradeManager.instance.CookSlotLevel;
        data.speedUpLevel = UpgradeManager.instance.SpeedUpLevel;
        data.cookBoardLevel = UpgradeManager.instance.OrderBoardLevel;
        data.orderHintLevel = UpgradeManager.instance.OrderHintLevel;
        data.containerSlotLevel = UpgradeManager.instance.ContainerSlotCount;

        UnlockManager.instance.WriteTo(data);

        return data;
    }

    private void ApplySaveData(SaveData data)
    {
        GoldManager.Instance.LoadFrom(data);
        TruckRankManager.instance.LoadFrom(data);
        UpgradeManager.instance.LoadFrom(data);
        UnlockManager.instance.LoadFrom(data);

        if (data.cookingGuideDone || data.currentRank > 1)
            CookingGuideManager.MarkGuideDone();
        else
            CookingGuideManager.ResetGuideDone();
    }

    private void WriteLocalSave(SaveData data)
    {
        if (string.IsNullOrEmpty(SavePath))
        {
            Debug.LogWarning("[SaveManager] 로그인된 유저가 없어 로컬 저장을 건너뜁니다.");
            return;
        }

        string json = JsonUtility.ToJson(data, prettyPrint: true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveManager] 저장 완료: {SavePath}");
    }

    private bool TryReadLocalSave(out SaveData data)
    {
        data = null;

        if (!HasSave())
            return false;

        try
        {
            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<SaveData>(json);
            return data != null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] 로컬 저장 데이터 읽기 실패: {ex.Message}");
            return false;
        }
    }

    public async UniTask<bool> SaveToFirebaseAsync()
    {
        SaveData data = CreateSaveData();
        return await SaveDataToFirebaseAsync(data);
    }

    private async UniTask<bool> SaveDataToFirebaseAsync(SaveData data)
    {
        DatabaseReference saveRef = GetUserSaveRef();

        if (saveRef == null) return false;

        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            await saveRef.SetRawJsonValueAsync(json);

            Debug.Log("[SaveManager] Firebase 저장 완료");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] Firebase 저장 실패: {ex.Message}");
            return false;
        }
    }

    public async UniTask<bool> LoadFromFirebaseAsync()
    {
        var result = await TryReadFirebaseSaveAsync();

        if (!result.success)
            return false;

        ApplySaveData(result.data);

        Debug.Log("[SaveManager] Firebase 불러오기 완료");
        return true;
    }

    private async UniTask<(bool success, SaveData data)> TryReadFirebaseSaveAsync()
    {
        DatabaseReference saveRef = GetUserSaveRef();

        if (saveRef == null) return (false, null);

        try
        {
            DataSnapshot snapshot = await saveRef.GetValueAsync();

            if (!snapshot.Exists)
            {
                Debug.LogWarning("[SaveManager] Firebase 저장 데이터가 없습니다.");
                return (false, null);
            }

            string json = snapshot.GetRawJsonValue();
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            return data != null ? (true, data) : (false, null);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] Firebase 불러오기 실패: {ex.Message}");
            return (false, null);
        }
    }

    public async UniTask<bool> SaveWithBackupAsync()
    {
        SaveData data = CreateSaveData();
        bool localSaved = false;

        try
        {
            WriteLocalSave(data);
            localSaved = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] 로컬 저장 실패: {ex.Message}");
        }

        bool firebaseSaved = await SaveDataToFirebaseAsync(data);
        if (!firebaseSaved)
            Debug.LogWarning("[SaveManager] Firebase 저장 실패. 로컬 저장 데이터를 백업으로 유지합니다.");

        return localSaved;
    }

    public async UniTask<bool> LoadLatestAsync()
    {
        bool hasLocal = TryReadLocalSave(out SaveData localData);
        var firebaseResult = await TryReadFirebaseSaveAsync();
        bool hasFirebase = firebaseResult.success;

        if (!hasLocal && !hasFirebase)
            return false;

        SaveData latestData;
        bool shouldUploadLocalToFirebase = false;
        bool shouldWriteFirebaseToLocal = false;

        if (hasLocal && hasFirebase)
        {
            if (localData.lastSavedAtUtcTicks >= firebaseResult.data.lastSavedAtUtcTicks)
            {
                latestData = localData;
                shouldUploadLocalToFirebase = localData.lastSavedAtUtcTicks > firebaseResult.data.lastSavedAtUtcTicks;
            }
            else
            {
                latestData = firebaseResult.data;
                shouldWriteFirebaseToLocal = true;
            }
        }
        else if (hasLocal)
        {
            latestData = localData;
            shouldUploadLocalToFirebase = true;
        }
        else
        {
            latestData = firebaseResult.data;
            shouldWriteFirebaseToLocal = true;
        }

        ApplySaveData(latestData);

        if (shouldUploadLocalToFirebase)
            await SaveDataToFirebaseAsync(latestData);

        if (shouldWriteFirebaseToLocal)
        {
            try
            {
                WriteLocalSave(latestData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveManager] Firebase 데이터를 로컬에 동기화 실패: {ex.Message}");
            }
        }

        Debug.Log("[SaveManager] 최신 저장 데이터 불러오기 완료");
        return true;
    }

    private async UniTask<bool> HasFirebaseSaveAsync()
    {
        var result = await TryReadFirebaseSaveAsync();
        return result.success;
    }

    public async UniTask<bool> HasAnySaveAsync()
    {
        if (HasSave())
            return true;

        if (AuthManager.Instance == null || !AuthManager.Instance.IsLoggedIn)
            return false;

        return await HasFirebaseSaveAsync();
    }

    private async UniTask<bool> DeleteFirebaseSaveAsync()
    {
        DatabaseReference saveRef = GetUserSaveRef();

        if (saveRef == null) return false;

        try
        {
            await saveRef.RemoveValueAsync();

            Debug.Log("[SaveManager] Firebase 저장 데이터 삭제 완료");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] Firebase 저장 데이터 삭제 실패: {ex.Message}");
            return false;
        }
    }

    public async UniTask<bool> DeleteAllSaveAsync()
    {
        DeleteSave();

        if (AuthManager.Instance == null || !AuthManager.Instance.IsLoggedIn)
            return true;

        bool firebaseDeleted = await DeleteFirebaseSaveAsync();

        return firebaseDeleted;
    }

    private DatabaseReference GetUserSaveRef()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsLoggedIn)
        {
            Debug.LogError("[SaveManager] 로그인된 유저가 없습니다.");
            return null;
        }

        string uid = AuthManager.Instance.UserId;

        return FirebaseInitializer.Instance
            .RootReference
            .Child("users")
            .Child(uid)
            .Child("saveData");
    }
}
