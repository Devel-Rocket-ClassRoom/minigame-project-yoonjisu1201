using UnityEngine;

public class OrderPopup : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _orderIcon;
    [SerializeField] private SpriteRenderer _menuIcon;
    [SerializeField] private Transform _emptyCoverPivot;
    [SerializeField] private Sprite _normalOrderSprite;
    [SerializeField] private Sprite _signatureOrderSprite;
    [SerializeField] private SpriteRenderer _firstIngredientIcon;
    [SerializeField] private SpriteRenderer _favoriteStarIcon;
    //나중에 에셋추가되면 [SerializeField] private SpriteRenderer __gaugeBarIcon;


    //손님 컨트롤러에서 사용
    public void Show(RecipeSO recipe, GhostSO ghostData)
    {
        if (recipe == null) return;

        bool isSignature = recipe.isSignatureMenu && recipe.ownerGhost == ghostData;
        if (isSignature)
            _menuIcon.sprite = recipe.specialIcon;
        else
            _menuIcon.sprite = recipe.icon;

        int level = UpgradeManager.instance.OrderHintLevel;
        if (level >= 1)
            _orderIcon.sprite = isSignature ? _signatureOrderSprite : _normalOrderSprite;

        if (_firstIngredientIcon != null)
        {
            bool showHint = level >= 2 && recipe.basicIngredients != null && recipe.basicIngredients.Count > 0;
            _firstIngredientIcon.enabled = showHint;
            if (showHint)
                _firstIngredientIcon.sprite = recipe.basicIngredients[0].icon;
        }
        if (_favoriteStarIcon != null)
            _favoriteStarIcon.enabled = level >= 3 && isSignature;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void SetGauge(float ratio)
    {
        if (_emptyCoverPivot == null) return;
        Vector3 scale = _emptyCoverPivot.localScale;
        scale.y = 1f - ratio;
        _emptyCoverPivot.localScale = scale;
    }
}
