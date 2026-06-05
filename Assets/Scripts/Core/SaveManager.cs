using System.IO;
using UnityEngine;

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
        var data = new SaveData();

        data.totalGold = GoldManager.Instance.TotalGold;

        data.currentRank = TruckRankManager.instance.CurrentRank;
        data.totalExp = TruckRankManager.instance.TotalExp;

        data.cookSlotLevel = UpgradeManager.instance.CookSlotLevel;
        data.speedUpLevel = UpgradeManager.instance.SpeedUpLevel;
        data.cookBoardLevel = UpgradeManager.instance.OrderBoardLevel;
        data.orderHintLevel = UpgradeManager.instance.OrderHintLevel;
        data.containerSlotLevel = UpgradeManager.instance.ContainerSlotCount;

        //
        UnlockManager.instance.WriteTo(data);

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
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

        GoldManager.Instance.LoadFrom(data);
        TruckRankManager.instance.LoadFrom(data);
        UpgradeManager.instance.LoadFrom(data);
        UnlockManager.instance.LoadFrom(data);

        Debug.Log("[SaveManager] 불러오기 완료");
    }

    public void DeleteSave()
    {
        if (HasSave())
            File.Delete(SavePath);
    }


}
