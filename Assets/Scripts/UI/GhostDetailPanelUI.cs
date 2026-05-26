using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//도감 유령탭 - 오른쪽 상세패널 담당. 선택된 유령의 상세정보 표시
public class GhostDetailPanelUI : MonoBehaviour
{
    [Header("유령 기본 정보")]
    [SerializeField] private Image _ghostIcon;
    [SerializeField] private TextMeshProUGUI _ghostNameText;
    [SerializeField] private TextMeshProUGUI _ghostDescText;

    [Header("좋아하는 메뉴")]
    [SerializeField] private Image _menuIcon;
    [SerializeField] private TextMeshProUGUI _menuNameText;
    [SerializeField] private Image _specialIngredientIcon;
    [SerializeField] private TextMeshProUGUI _specialIngredientText;

    [Header("유물")]
    [SerializeField] private Image _artifactIcon;
    [SerializeField] private TextMeshProUGUI _artifactNameText;
    [SerializeField] private TextMeshProUGUI _artifactDescText;
    [SerializeField] private Image[] _artifactStars;
    [SerializeField] private TextMeshProUGUI _guestBookText;



    private static readonly Color STAR_ON = new Color(1f, 0.85f, 0.1f, 1f);
    private static readonly Color STAR_OFF = new Color(0.6f, 0.6f, 0.6f, 0.4f);

    //GhostCollectionPanelUI에서 선택된 유령 데이터 받아서 UI 업데이트
    public void showGhost(GhostSO ghost, ContentRegistrySO registry)
    {
        if (ghost == null) return;

        bool unlocked = UnlockManager.instance != null
            && UnlockManager.instance.IsGhostUnlocked(ghost);

        if (unlocked)
            ShowUnlocked(ghost, registry);
        else
            ShowLocked(ghost);
    }

    private void ShowUnlocked(GhostSO ghost, ContentRegistrySO registry)
    {
        _ghostIcon.sprite = ghost.icon;
        _ghostNameText.text = LocalizationManager.GetGhostName(ghost.id);
        _ghostDescText.text = LocalizationManager.GetGhostDesc(ghost.id);

        //메뉴
        RecipeSO signature = FindSignatureRecipe(ghost, registry);
        if (signature != null)
        {
            _menuIcon.sprite = signature.icon;
            _menuIcon.color = Color.white;
            _menuNameText.text = LocalizationManager.GetRecipeName(signature.id);

            if (signature.special_Ingredient != null)
            {
                _specialIngredientIcon.sprite = signature.special_Ingredient.icon;
                _specialIngredientText.text = LocalizationManager.GetIngredientName(signature.special_Ingredient.id);
            }
        }
        else
        {
            _menuIcon.color = Color.clear;
            _menuNameText.text = "-";
        }

        //유물
        if (ghost.artifact != null)
        {
            _artifactIcon.sprite = ghost.artifact.icon;
            _artifactIcon.color = Color.white;
            _artifactNameText.text = LocalizationManager.GetArtifactName(ghost.artifact.id);

            _artifactDescText.text = LocalizationManager.GetArtifactPassive(ghost.artifact.id);

            int count = UnlockManager.instance.GetArtifacCount(ghost.artifact);
            for (int i = 0; i < _artifactStars.Length; i++)
                _artifactStars[i].color = i < count ? STAR_ON : STAR_OFF;
        }
        else
        {
            //유물이 없는 경우 (기본유령)
        }

        //방명록
        bool _guestBookUnlocked = ghost.artifact != null
            && UnlockManager.instance.IsArtifactUnlocked(ghost.artifact);
        if (_guestBookUnlocked)
            _guestBookText.text = LocalizationManager.GetArtifactMemoir(ghost.artifact.id);
        else
            _guestBookText.text = LocalizationManager.Get("ui_label_memoir_locked");
    }
    private void ShowLocked(GhostSO ghost)
    {
        _ghostIcon.sprite = ghost.icon;
        _ghostIcon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        _ghostNameText.text = "???";

        _menuIcon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        _menuNameText.text = "???";

        _artifactIcon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        _artifactNameText.text = "???";
        _artifactDescText.text = "???";

        foreach (var star in _artifactStars)
        {
            star.color = STAR_OFF;
        }

        _guestBookText.text = "???";
    }
    private RecipeSO FindSignatureRecipe(GhostSO ghost, ContentRegistrySO registry)
    {
        if (registry == null) return null;
        foreach (var recipe in registry.allRecipes)
        {
            if (recipe.isSignatureMenu && recipe.ownerGhost == ghost)
                return recipe;
        }
        return null;
    }
}
