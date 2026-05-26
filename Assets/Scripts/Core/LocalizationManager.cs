using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using System.Resources;
using UnityEngine.iOS;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    private readonly Dictionary<string, string> _table = new Dictionary<string, string>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCSV("DataTables/StringTableKr");
    }

    private void LoadCSV(string resourcePath)
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcePath);
        if (csv == null)
        {
            Debug.LogError($"[LocalizationManager] CSV 파일을 찾을 수 없음 : {resourcePath}");
        }

        string[] lines = csv.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            int commaIndex = lines[i].IndexOf(',');
            if (commaIndex < 0) continue;

            string key = lines[i].Substring(0, commaIndex);
            string value = lines[i].Substring(commaIndex + 1);

            if (!string.IsNullOrEmpty(key))
                _table[key] = value;
        }
        Debug.Log($"[LocalizationManager] {_table.Count}개 텍스트 로드 완료");
    }

    public static string Get(string key)
    {
        if (instance == null) return key; //?
        if (instance._table.TryGetValue(key, out string value)) return value;
        Debug.LogWarning($"[LocalizationManager] 키 없음: '{key}'");
        return key;
    }

    //SO에서 아이디를 키로 사용
    private static string NormalizeId(string id) => id.ToLower().Replace(" ", "_");

    public static string GetGhostName(string id) => Get(NormalizeId(id) + "_name");
    public static string GetGhostDesc(string id) => Get(NormalizeId(id) + "_desc");
    public static string GetArtifactName(string id) => Get(NormalizeId(id) + "_name");
    public static string GetArtifactPassive(string id) => Get(NormalizeId(id) + "_passive");
    public static string GetArtifactMemoir(string id) => Get(NormalizeId(id) + "_memoir");
    public static string GetRecipeName(string id) => Get(NormalizeId(id) + "_name");
    public static string GetIngredientName(string id) => Get(NormalizeId(id) + "_name");
}
