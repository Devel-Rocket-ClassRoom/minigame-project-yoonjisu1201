using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ArtifactCollectionCardUI : MonoBehaviour
{
    [SerializeField] private Image _artifactImage;
    [SerializeField] private TextMeshProUGUI _artifactNameText;

    private static readonly Color LOCKED_COLOR = Color.black;
    private static readonly Color UNLOCKED_COLOR = Color.white;

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
    }

    private void OnClicked()
    {
        if (_artifactData != null)
            _onSelected?.Invoke(_artifactData);
    }
}

