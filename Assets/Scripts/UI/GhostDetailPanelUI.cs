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
    [SerializeField] private TextMeshProUGUI _ghostTypeText;

    [Header("좋아하는 메뉴")]
    [SerializeField] private Image _menuIcon;
    [SerializeField] private TextMeshProUGUI _menuNameText;
    [SerializeField] private TextMeshProUGUI _basicMenuText; // 기본유령 전용 고정 텍스트

    [Header("유물")]
    [SerializeField] private TextMeshProUGUI _guestBookText;



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
        int index = registry.allGhosts.IndexOf(ghost);
        _ghostIcon.sprite = ghost.icon;
        _ghostIcon.color = unlocked ? Color.white : Color.black;

        if (unlocked)
        {
            string number = (index + 1).ToString("D2");
            _ghostNameText.text = $"{number}. {LocalizationManager.GetGhostName(ghost.id)}";
            _ghostDescText.text = LocalizationManager.GetGhostDesc(ghost.id);
            _ghostTypeText.text = $"타입: {ghost.patienceType}";
        }
        else
        {
            _ghostNameText.text = "???";
            _ghostDescText.text = "???";
            _ghostTypeText.text = "타입: ???";
        }
    }
    private void ShowMenuSection(GhostSO ghost, RecipeSO signature, 
        bool unlocked, ContentRegistrySO registry)
    {
        bool isBasicGhost = signature == null;

        _menuIcon.gameObject.SetActive(!isBasicGhost);
        _menuNameText.gameObject.SetActive(!isBasicGhost);
        if (_basicMenuText != null) _basicMenuText.gameObject.SetActive(isBasicGhost);

        if (!isBasicGhost)
        {
            _menuIcon.sprite = signature.icon;
            _menuIcon.color = unlocked ? Color.white : Color.black;
            _menuNameText.text = unlocked ? LocalizationManager.GetRecipeName(signature.id) : "???";
        }
    }
    private void ShowArtifactSection(GhostSO ghost, bool unlocked)
    {
        if (ghost.artifact == null)
        {
            _guestBookText.text = "-";
            return;
        }

        bool memoirUnlocked = unlocked
            && UnlockManager.instance.IsArtifactUnlocked(ghost.artifact);

        _guestBookText.text = memoirUnlocked
            ? LocalizationManager.GetArtifactMemoir(ghost.artifact.id)
            : LocalizationManager.Get("ui_label_memoir_locked");
    }
    
    private RecipeSO FindSignatureRecipe(GhostSO ghost, ContentRegistrySO registry)
    {
        if (registry == null) return null;
        foreach (var recipe in registry.allRecipes)
        {
            if (recipe.ownerGhost == ghost)
                return recipe;
        }
        return null;
    }
}
