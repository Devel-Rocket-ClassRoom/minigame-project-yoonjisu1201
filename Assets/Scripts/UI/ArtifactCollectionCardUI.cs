using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ArtifactCollectionCardUI : MonoBehaviour
{
    [SerializeField] private Image _artifactImage;
    [SerializeField] private TextMeshProUGUI _artifactNameText;
    [SerializeField] private Image[] _stars;

    private static readonly Color LOCKED_COLOR = Color.black;
    private static readonly Color UNLOCKED_COLOR = Color.white;
    private static readonly Color STAR_ON = new Color(1f, 0.85f, 0.1f, 1f);
    private static readonly Color STAR_OFF = new Color(0.6f, 0.6f, 0.6f, 0.4f);

    private ArtifactSO _artifactData;
    private Button _button;
    private System.Action<ArtifactSO> _onSelected;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    public void Setup(ArtifactSO artifact, System.Action<ArtifactSO> onSelected)
    {
        _artifactData = artifact;
        _onSelected = onSelected;
        Refresh();
    }

    private void Refresh()
    {
        if (_artifactData == null) return;

        bool unlocked = UnlockManager.instance != null
            && UnlockManager.instance.IsArtifactUnlocked(_artifactData);

        _artifactImage.sprite = _artifactData.icon;
        _artifactImage.color = unlocked ? UNLOCKED_COLOR : LOCKED_COLOR;
        _artifactNameText.text = unlocked
            ? LocalizationManager.GetArtifactName(_artifactData.id)
            : "???";
        _button.interactable = true;

        int count = UnlockManager.instance != null
        ? UnlockManager.instance.GetArtifacCount(_artifactData) : 0;
        for (int i = 0; i < _stars.Length; i++)
            _stars[i].color = i < count ? STAR_ON : STAR_OFF;
    }

    private void OnClicked()
    {
        if (_artifactData != null)
            _onSelected?.Invoke(_artifactData);
    }
}

