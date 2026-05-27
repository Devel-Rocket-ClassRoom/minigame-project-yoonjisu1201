using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//도감 유령탭 - 오른쪽 상세패널 담당. 선택된 유령의 상세정보 표시
public class GhostDetailPanelUI : MonoBehaviour
{
    [Header("유령 기본 정보")]
    [SerializeField] private Image _ghostIcon;
    [SerializeField] private TextMeshProUGUI _ghostNameText;
    [SerializeField] private TextMeshProUGUI _ghostDescText;

    [Header("좋아하는 메뉴")]
    [SerializeField] private Image _menuIcon;
    [SerializeField] private Image _menuIcon2;
    [SerializeField] private Image _menuIcon3;
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

        RecipeSO signature = FindSignatureRecipe(ghost, registry);

        ShowGhostBase(ghost, unlocked, registry);
        ShowMenuSection(ghost, signature, unlocked, registry);
        ShowArtifactSection(ghost, unlocked);
    }

    private void ShowGhostBase(GhostSO ghost, bool unlocked, ContentRegistrySO registry)
    {
        _ghostIcon.sprite = ghost.icon;
        _ghostIcon.color = unlocked ? Color.white : Color.black;

        if (unlocked)
        {
            int index = registry.allGhosts.IndexOf(ghost);
            string number = (index + 1).ToString("D2");
            _ghostNameText.text = $"{number}. {LocalizationManager.GetGhostName(ghost.id)}";
            _ghostDescText.text = LocalizationManager.GetGhostDesc(ghost.id);
        }
        else
        {
            _ghostNameText.text = "???";
            _ghostDescText.text = "???";
        }
    }
    private void ShowMenuSection(GhostSO ghost, RecipeSO signature, 
        bool unlocked, ContentRegistrySO registry)
    {
        if (signature != null)
        {
            _menuIcon.sprite = signature.icon;
            _menuIcon.color = unlocked ? Color.white : Color.black;
            _menuNameText.text = unlocked ? LocalizationManager.GetRecipeName(signature.id) : "???";

            _menuIcon2.sprite = null;
            _menuIcon2.color = Color.clear;
            _menuIcon3.sprite = null;
            _menuIcon3.color = Color.clear;

            if (signature.special_Ingredient != null)
            {
                _specialIngredientIcon.sprite = signature.special_Ingredient.icon;
                _specialIngredientIcon.color = unlocked ? Color.white : Color.black;
                _specialIngredientText.text = unlocked
                    ? LocalizationManager.GetIngredientName(signature.special_Ingredient.id)
                    : "???";
            }
            else
            {
                _specialIngredientIcon.sprite = null;
                _specialIngredientIcon.color = Color.clear;
                _specialIngredientText.text = "-";
            }
        }
        else
        {
            List<RecipeSO> basicMenus = FindBasicMenus(ghost, registry);

            SetBasicMenuIcon(_menuIcon, basicMenus, 0, unlocked);
            SetBasicMenuIcon(_menuIcon2, basicMenus, 1, unlocked);
            SetBasicMenuIcon(_menuIcon3, basicMenus, 2, unlocked);

            _menuNameText.text = ""; //임시로 비움 공간때문에
            _specialIngredientIcon.sprite = null;
            _specialIngredientIcon.color = Color.clear;
            _specialIngredientText.text = "-";
        }

        
    }
    private List<RecipeSO> FindBasicMenus(GhostSO ghost, ContentRegistrySO registry)
    {
        var result = new List<RecipeSO>();
        foreach (var recipe in registry.allRecipes)
        {
            if (!recipe.isSignatureMenu && recipe.unlockRank == ghost.unlockRank)
                result.Add(recipe);
        }
        return result;
    }
    private void SetBasicMenuIcon(Image icon, List<RecipeSO> menus, int index, bool unlocked)
    {
        if (index < menus.Count && menus[index].icon != null)
        {
            icon.sprite = menus[index].icon;
            icon.color = Color.white;
        }
        else
        {
            icon.sprite = null;
            icon.color = Color.clear;
        }
    }
    private void ShowArtifactSection(GhostSO ghost, bool unlocked)
    {
        if (ghost.artifact == null)
        {
            _artifactIcon.sprite = null;
            _artifactIcon.color = Color.clear;
            _artifactDescText.text = "-";
            _guestBookText.text = "-";
            foreach (var star in _artifactStars)
                star.color = STAR_OFF;
            return;
        }

        _artifactIcon.sprite = ghost.artifact.icon;
        _artifactIcon.color = unlocked ? Color.white : Color.black;
        _artifactDescText.text = unlocked
            ? LocalizationManager.GetArtifactPassive(ghost.artifact.id)
            : "???";

        int count = unlocked ? UnlockManager.instance.GetArtifacCount(ghost.artifact) : 0;
        for (int i = 0; i < _artifactStars.Length; i++)
            _artifactStars[i].color = i < count ? STAR_ON : STAR_OFF;

        bool memoirUnlocked = unlocked
            && UnlockManager.instance.IsArtifactUnlocked(ghost.artifact);

        _guestBookText.text = memoirUnlocked
            ? LocalizationManager.GetArtifactMemoir(ghost.artifact.id)
        : LocalizationManager.Get("ui_label_memoir_locked");
    }
    //private void ShowUnlocked(GhostSO ghost, ContentRegistrySO registry)
    //{
    //    _ghostIcon.sprite = ghost.icon;

    //    int index = registry.allGhosts.IndexOf(ghost);
    //    string number = (index + 1).ToString("D2");
    //    _ghostNameText.text = $"{number}. {LocalizationManager.GetGhostName(ghost.id)}";
    //    _ghostDescText.text = LocalizationManager.GetGhostDesc(ghost.id);

    //    //메뉴
    //    RecipeSO signature = FindSignatureRecipe(ghost, registry);
    //    if (signature != null)
    //    {
    //        _menuIcon.sprite = signature.icon;
    //        _menuIcon.color = Color.white;
    //        _menuNameText.text = LocalizationManager.GetRecipeName(signature.id);

    //        if (signature.special_Ingredient != null)
    //        {
    //            _specialIngredientIcon.sprite = signature.special_Ingredient.icon;
    //            _specialIngredientIcon.color = Color.white;
    //            _specialIngredientText.text = LocalizationManager.GetIngredientName(signature.special_Ingredient.id);
    //        }
    //    }
    //    else
    //    {
    //        _menuIcon.color = Color.clear;
    //        _menuIcon.sprite = null;
    //        _menuNameText.text = "-";
    //        _specialIngredientIcon.sprite = null;
    //        _specialIngredientIcon.color = Color.clear;
    //        _specialIngredientText.text = "-";
    //    }

    //    //유물
    //    if (ghost.artifact != null)
    //    {
    //        _artifactIcon.sprite = ghost.artifact.icon;
    //        _artifactIcon.color = Color.white;
    //        //_artifactNameText.text = LocalizationManager.GetArtifactName(ghost.artifact.id);

    //        _artifactDescText.text = LocalizationManager.GetArtifactPassive(ghost.artifact.id);

    //        int count = UnlockManager.instance.GetArtifacCount(ghost.artifact);
    //        for (int i = 0; i < _artifactStars.Length; i++)
    //            _artifactStars[i].color = i < count ? STAR_ON : STAR_OFF;
    //    }
    //    else
    //    {
    //        _artifactIcon.sprite = null;
    //        _artifactIcon.color = Color.clear;
    //        _artifactDescText.text = "-";
    //        foreach (var star in _artifactStars)
    //            star.color = STAR_OFF;

    //    }

    //    //방명록
    //    bool _guestBookUnlocked = ghost.artifact != null
    //        && UnlockManager.instance.IsArtifactUnlocked(ghost.artifact);
    //    if (_guestBookUnlocked)
    //        _guestBookText.text = LocalizationManager.GetArtifactMemoir(ghost.artifact.id);
    //    else
    //        _guestBookText.text = LocalizationManager.Get("ui_label_memoir_locked");
    //}
    //private void ShowLocked(GhostSO ghost)
    //{
    //    _ghostIcon.sprite = ghost.icon;
    //    _ghostIcon.color = Color.black;
    //    _ghostNameText.text = "???";
    //    _ghostDescText.text = "???";

    //    _menuIcon.color = Color.black;
    //    _menuNameText.text = "???";
    //    _specialIngredientIcon.color = Color.black;
    //    _specialIngredientText.text = "???";

    //    _artifactIcon.color = Color.black;
    //    //_artifactNameText.text = "???";
    //    _artifactDescText.text = "???";

    //    foreach (var star in _artifactStars)
    //    {
    //        star.color = STAR_OFF;
    //    }

    //    _guestBookText.text = "???";
    //}
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
