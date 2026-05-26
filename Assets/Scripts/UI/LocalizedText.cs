using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _key;

    private void Start()
    {
        GetComponent<TMP_Text>().text = LocalizationManager.Get(_key);
    }
}
