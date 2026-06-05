using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUpCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryLabel;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void Setup(Sprite icon, string name, string category)
    {
        _icon.sprite = icon;
        _nameText.text = name;
        _categoryLabel.text = category;
    }
}
