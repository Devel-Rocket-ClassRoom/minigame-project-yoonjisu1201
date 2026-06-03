using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeCardUI
{
    public Button button;
    public Image cardImage;
    public Image icon;
    public TextMeshProUGUI nameText;
    public Image[] stars;
}

// 상세 패널에서 레벨 1/2/3 행 각각에 대응하는 UI 요소 묶음
// 강화하기 버튼은 상세 패널에서 1개 공유 → 여기에 없음
[System.Serializable]
public class UpgradeLevelEntryUI
{
    public GameObject root;
    public Image rowImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI costText;
}
public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private UpgradeCardUI[] _cards;

    [SerializeField] private Sprite _cardNormal;
    [SerializeField] private Sprite _cardSelected;
    [SerializeField] private Sprite _starFilled;
    [SerializeField] private Sprite _starEmpty;

    [SerializeField] private TextMeshProUGUI _detailTitleText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeButtonText;
    [SerializeField] private UpgradeLevelEntryUI[] _levelEntries;

    private int _selectedIndex = 0;

    private void OnEnable()
    {
        Refresh();
    }
    private void Start()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            int idx = i;
            _cards[i].button.onClick.AddListener(() => SelectCard(idx));
        }

        _upgradeButton.onClick.AddListener(() =>
        {
            UpgradeManager.instance.TryUpgrade((UpgradeType)_selectedIndex);
            Refresh();
        });

        SelectCard(0);
    }

    private void SelectCard(int index)
    {
        _selectedIndex = index;
        for (int i = 0; i < _cards.Length; i++)
            _cards[i].cardImage.sprite = i == _selectedIndex ? _cardSelected : _cardNormal;

        Refresh();
    }
    private void Refresh()
    {
        if (UpgradeManager.instance == null || GoldManager.Instance == null) return;
        var um = UpgradeManager.instance;

        for (int i = 0; i < _cards.Length; i++)
        {
            var type = (UpgradeType)i;
            int current = um.GetCurrentLevel(type);
            int max = um.GetCosts(type).Length;
            if (_cards[i].nameText != null)
                _cards[i].nameText.text = LocalizationManager.GetUpgradeName(type);

            for (int s = 0; s < _cards[i].stars.Length; s++)
            {
                if (s >= max)
                {
                    _cards[i].stars[s].gameObject.SetActive(false);
                }
                else
                {
                    _cards[i].stars[s].gameObject.SetActive(true);
                    _cards[i].stars[s].sprite = s < current ? _starFilled : _starEmpty;
                }
            }
        }

        UpgradeType selected = (UpgradeType)_selectedIndex;
        int[] costs = um.GetCosts(selected);
        int currentLevel = um.GetCurrentLevel(selected);
        bool canUpgrade = um.CanUpgrade(selected);

        _detailTitleText.text = LocalizationManager.GetUpgradeName(selected);

        bool maxed = currentLevel >= costs.Length;
        _upgradeButton.interactable = !maxed && canUpgrade;
        if (_upgradeButtonText != null)
            _upgradeButtonText.text = maxed ? "MAX" : "강화하기";

        for (int i = 0; i < _levelEntries.Length; i++)
        {
            var entry = _levelEntries[i];

            if (i >= costs.Length)
            {
                entry.root.SetActive(false);
                continue;
            }

            entry.root.SetActive(true);
            entry.levelText.text = $"Lv {i + 1}";
            entry.descText.text = LocalizationManager.GetUpgradeLevelDesc(selected, i);
            entry.costText.text = $"{costs[i]}G";

            bool completed = currentLevel > i;
            if (entry.rowImage != null)
                entry.rowImage.color = completed
                    ? new Color(0xCD / 255f, 0xCD / 255f, 0xCD / 255f)
                    : Color.white;
        }
    }

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
}
