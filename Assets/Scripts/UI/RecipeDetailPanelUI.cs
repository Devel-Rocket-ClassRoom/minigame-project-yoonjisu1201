using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 도감 레시피 - 오른쪽 상세패널. 선택된 레시피의 상세정보 표시
public class RecipeDetailPanelUI : MonoBehaviour
{
    [Header("전용메뉴 이미지")]
    [SerializeField] private Image _signatureImageMain;

    [Header("레시피 정보")]
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private TextMeshProUGUI _recipeDescText;

    [Header("기본재료 (3종)")]
    [SerializeField] private Image[] _basicIngredientImages;           // 길이 3
    //[SerializeField] private TextMeshProUGUI[] _basicIngredientNames;  // 길이 3

    [Header("특별 버전 (특별재료 → 특별메뉴)")]
    [SerializeField] private Image _specialIngredientImage;
    [SerializeField] private Image _specialMenuImage;

    [Header("일반 버전 (일반재료 → 일반메뉴)")]
    [SerializeField] private Image _normalIngredientImage;
    [SerializeField] private Image _normalMenuImage;

    [Header("전용 손님")]
    [SerializeField] private Image _ownerGhostImage;

    [Header("레시피 메모")]
    [SerializeField] private TextMeshProUGUI _recipeMemoText;

    [Header("기본메뉴 잠금 안내 (기본메뉴일 때만 표시, 3개)")]
    [SerializeField] private GameObject[] _unlockRank2Labels;         // 텍스트 오브젝트 3개

    [Header("기본메뉴일 때 추가로 숨길 오브젝트")]
    [SerializeField] private GameObject[] _signatureOnlyObjects;  // 3개 


    // RecipeCollectionPanelUI에서 선택된 레시피 데이터 받아서 UI 업데이트
    public void ShowRecipe(RecipeSO recipe)
    {
        if (recipe == null) return;

        bool unlocked = !recipe.isSignatureMenu
           || (UnlockManager.instance != null && UnlockManager.instance.IsRecipeUnlocked(recipe));

        // 공통: 레시피 정보
        SetText(_recipeNameText, LocalizationManager.GetRecipeName(recipe.id), unlocked);
        SetText(_recipeDescText, LocalizationManager.GetRecipeDesc(recipe.id), unlocked);

        // 공통: 기본재료
        for (int i = 0; i < _basicIngredientImages.Length; i++)
        {
            bool hasIng = i < recipe.basicIngredients.Count
                          && recipe.basicIngredients[i] != null;

            SetImage(_basicIngredientImages[i], hasIng ? recipe.basicIngredients[i].icon : null, unlocked);
            //_basicIngredientNames[i].text = hasIng && unlocked
            //    ? LocalizationManager.GetIngredientName(recipe.basicIngredients[i].id)
            //    : (hasIng ? "???" : "-");
        }

        // 공통: 메모
        SetText(_recipeMemoText, LocalizationManager.GetRecipeMemo(recipe.id), unlocked);

        // 전용메뉴 / 기본메뉴 분기
        if (recipe.isSignatureMenu)
            ShowSignatureSection(recipe, unlocked);
        else
            ShowBasicSection(recipe, unlocked);
    }

    // 전용메뉴: 섹션 내용 활성화 + 잠금 안내 숨김
    private void ShowSignatureSection(RecipeSO recipe, bool unlocked)
    {
        SetSectionActive(true);
        foreach (var label in _unlockRank2Labels)
            label.SetActive(false);

        SetImage(_signatureImageMain, recipe.specialIcon, unlocked);
        SetImage(_specialIngredientImage, recipe.special_Ingredient?.icon, unlocked);
        SetImage(_specialMenuImage, recipe.specialIcon, unlocked);
        SetImage(_normalIngredientImage, recipe.normalLast_Ing?.icon, unlocked);
        SetImage(_normalMenuImage, recipe.icon, unlocked);
        SetImage(_ownerGhostImage, recipe.ownerGhost?.icon, unlocked);
    }

    // 기본메뉴: 섹션 내용 비활성화(텍스트간판 제외) + 잠금 안내 표시 + 상단에 단일 이미지
    private void ShowBasicSection(RecipeSO recipe, bool unlocked)
    {
        SetSectionActive(false);
        foreach (var label in _unlockRank2Labels)
            label.SetActive(true);

        SetImage(_signatureImageMain, recipe.icon, unlocked);
    }

    // 일반메뉴·특별메뉴 섹션 내용물 + 관련 손님 이미지 활성/비활성
    // (텍스트간판 오브젝트는 이 필드에 포함되지 않으므로 영향 없음)
    private void SetSectionActive(bool active)
    {
        _specialIngredientImage.gameObject.SetActive(active);
        _specialMenuImage.gameObject.SetActive(active);
        _normalIngredientImage.gameObject.SetActive(active);
        _normalMenuImage.gameObject.SetActive(active);
        _ownerGhostImage.gameObject.SetActive(active);

        foreach (var obj in _signatureOnlyObjects)
            obj.SetActive(active);
    }

    private void SetImage(Image img, Sprite sprite, bool unlocked)
    {
        img.sprite = sprite;
        img.color = sprite == null ? Color.clear : (unlocked ? Color.white : Color.black);
    }
    private void SetText(TextMeshProUGUI text, string value, bool unlocked)
    {
        text.text = unlocked ? value : "???";
    }
}
