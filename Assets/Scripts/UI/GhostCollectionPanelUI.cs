using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//카드 5장 + 페이지네이션 관리
public class GhostCollectionPanelUI : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;
    [SerializeField] private GhostCollectionCardUI[] _cards;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TextMeshProUGUI _pageText;
    [SerializeField] private GhostDetailPanelUI _detailPanel;
    [SerializeField] private GameObject _rightObject;
    [SerializeField] private GameObject _lockedPageObject;

    private const int TOTAL_PAGES = 2;
    private int _currentPage = 0;
    private GhostSO _selectedGhost;

    private void Start()
    {
        _leftButton.onClick.AddListener(PrevPage);
        _rightButton.onClick.AddListener(NextPage);

        if (UnlockManager.instance != null)
            UnlockManager.instance.OnGhostUnlocked += OnGhostUnlockedHandler;

        RefreshPage();
        if (_registry.allGhosts.Count > 0)
            OnCardSelected(_registry.allGhosts[0]);
    }

    private void OnEnable()
    {
        if (_selectedGhost != null)
            OnCardSelected(_selectedGhost);
    }

    //해금 여부검사는 GhostCollectionCardUI에서 함
    private void RefreshPage()
    {
        int startIndex = _currentPage * _cards.Length;

        for (int i = 0; i < _cards.Length; i++)
        {
            int ghostIndex = startIndex + i;
            if (ghostIndex < _registry.allGhosts.Count)
                _cards[i].Setup(_registry.allGhosts[ghostIndex], OnCardSelected);
            else
                break;
        }
        _pageText.text = $"{_currentPage + 1} / {TOTAL_PAGES}";
        _leftButton.interactable = _currentPage > 0;
        _rightButton.interactable = _currentPage < TOTAL_PAGES - 1;
    }
    private void OnCardSelected(GhostSO ghost)
    {
        _selectedGhost = ghost;
        bool unlocked = UnlockManager.instance != null
            && UnlockManager.instance.IsGhostUnlocked(ghost);
        _rightObject.SetActive(unlocked);
        _lockedPageObject.SetActive(!unlocked);
        if (unlocked)
            _detailPanel.showGhost(ghost, _registry);
    }
    private void OnGhostUnlockedHandler(GhostSO _) => RefreshPage();

    private void OnDestroy()
    {
        if (UnlockManager.instance != null)
            UnlockManager.instance.OnGhostUnlocked -= OnGhostUnlockedHandler;
    }

    private void PrevPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            RefreshPage();
        }
    }
    private void NextPage()
    {
        if (_currentPage < TOTAL_PAGES - 1)
        {
            _currentPage++;
            RefreshPage();
        }
    }
}
