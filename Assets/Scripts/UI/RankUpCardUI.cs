using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUpCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryLabel;
    [SerializeField] private Image _categoryImage;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;

    [SerializeField] private Sprite _guestSprite;
    [SerializeField] private Sprite _recipeSprite;
    [SerializeField] private Sprite _ingredientSprite;

    public void Setup(Sprite icon, string name, string category)
    {
        _icon.sprite = icon;
        _nameText.text = name;
        _categoryLabel.text = category;

        if (_categoryImage != null)
        {
            _categoryImage.sprite = category switch
            {
                "손님" => _guestSprite,
                "일반레시피" or "전용레시피" => _recipeSprite,
                "일반재료" or "전용재료" => _ingredientSprite,
                _ => null
            };
        }
    }
}
