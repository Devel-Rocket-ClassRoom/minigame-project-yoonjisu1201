using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactCollectionPanelUI : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;
    [SerializeField] private ArtifactCollectionCardUI[] _cards;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TextMeshProUGUI _pageText;
    [SerializeField] private ArtifactDetailPanelUI _detailPanel;

    private int _currentPage = 0;
    private int TotalPages => Mathf.CeilToInt((float)_registry.allArtifacts.Count / _cards.Length);

    private void Start()
    {
        _leftButton.onClick.AddListener(PrevPage);
        _rightButton.onClick.AddListener(NextPage);

        if (UnlockManager.instance != null)
            UnlockManager.instance.OnArtifactUnlocked += _ => RefreshPage();

        RefreshPage();

        if (_registry.allArtifacts.Count > 0)
            _detailPanel.ShowArtifact(_registry.allArtifacts[0], _registry);
    }

    private void RefreshPage()
    {
        int startIndex = _currentPage * _cards.Length;

        for (int i = 0; i < _cards.Length; i++)
        {
            int artifactIndex = startIndex + i;
            if (artifactIndex < _registry.allArtifacts.Count)
            {
                _cards[i].gameObject.SetActive(true);
                _cards[i].Setup(_registry.allArtifacts[artifactIndex], OnCardSelected);
            }
            else
                _cards[i].gameObject.SetActive(false);
        }

        _pageText.text = $"{_currentPage + 1} / {TotalPages}";
        _leftButton.interactable = _currentPage > 0;
        _rightButton.interactable = _currentPage < TotalPages - 1;
    }

    private void OnCardSelected(ArtifactSO artifact)
    {
        _detailPanel.ShowArtifact(artifact, _registry);
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
        if (_currentPage < TotalPages - 1)
        {
            _currentPage++;
            RefreshPage();
        }
    }
}
