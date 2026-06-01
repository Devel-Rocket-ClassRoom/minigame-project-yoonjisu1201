using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ContainerSlotEntry
{
    public GameObject root;
    public Image icon;
    public TextMeshProUGUI nameText;
    public Button editButton;
    public Button clearButton;
}

public class RecipeContainerSetupUI : MonoBehaviour
{
    [SerializeField] private ContainerSlotEntry[] _entries;
    [SerializeField] private GameObject _pickerPanel;

}
