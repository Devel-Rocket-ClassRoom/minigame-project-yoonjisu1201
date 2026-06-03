using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _key;

    private void Awake()
    {
        GetComponent<TMP_Text>().text = LocalizationManager.Get(_key);
    }
}
