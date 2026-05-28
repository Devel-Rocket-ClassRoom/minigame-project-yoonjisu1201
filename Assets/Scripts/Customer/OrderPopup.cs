using UnityEngine;

public class OrderPopup : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _orderIcon;
    [SerializeField] private SpriteRenderer _menuIcon;
    [SerializeField] private Transform _emptyCoverPivot;
    //나중에 에셋추가되면 [SerializeField] private SpriteRenderer __gaugeBarIcon;


    //손님 컨트롤러에서 사용
    public void Show(RecipeSO recipe, GhostSO ghostData)
    {
        if (recipe == null) return;

        if (recipe.isSignatureMenu && recipe.ownerGhost == ghostData)
            _menuIcon.sprite = recipe.specialIcon;
        else
            _menuIcon.sprite = recipe.icon;

        SetGauge(1f);
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
