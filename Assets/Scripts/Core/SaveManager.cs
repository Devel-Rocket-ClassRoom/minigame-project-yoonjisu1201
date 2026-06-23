using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Database;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

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

    public bool HasSave() => File.Exists(SavePath);

    public void Save()
    {
        SaveData data = CreateSaveData();

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveManager] 저장 완료: {SavePath}");
    }

    public void Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] 저장 파일 없음");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));

        ApplySaveData(data);

        Debug.Log("[SaveManager] 불러오기 완료");
    }

    public void DeleteSave()
    {
        if (HasSave())
            File.Delete(SavePath);
    }

    private SaveData CreateSaveData()
    {
        var data = new SaveData();

        data.totalGold = GoldManager.Instance.TotalGold;

        data.currentRank = TruckRankManager.instance.CurrentRank;
        data.totalExp = TruckRankManager.instance.TotalExp;

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
    }

    public async UniTask<bool> SaveToFirebaseAsync()
    {
        DatabaseReference saveRef = GetUserSaveRef();

        if (saveRef == null) return false;

        try
        {
            SaveData data = CreateSaveData();

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
        DatabaseReference saveRef = GetUserSaveRef();

        if (saveRef == null) return false;

        try
        {
            DataSnapshot snapshot = await saveRef.GetValueAsync();

            if (!snapshot.Exists)
            {
                Debug.LogWarning("[SaveManager] Firebase 저장 데이터가 없습니다.");
                return false;
            }

            string json = snapshot.GetRawJsonValue();

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            ApplySaveData(data);

            Debug.Log("[SaveManager] Firebase 불러오기 완료");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] Firebase 불러오기 실패: {ex.Message}");
            return false;
        }
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
