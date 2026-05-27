using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDetailPanelUI : MonoBehaviour
{
    [Header("유물 정보")]
    [SerializeField] private Image _artifactImage;
    [SerializeField] private TextMeshProUGUI _artifactNameText;
    [SerializeField] private TextMeshProUGUI _artifactDescText;

    [Header("유물 효과")]
    [SerializeField] private TextMeshProUGUI _passiveNameText;
    [SerializeField] private TextMeshProUGUI _passiveDescText;

    [Header("전용 손님")]
    [SerializeField] private Image _ownerGhostImage;
    [SerializeField] private TextMeshProUGUI _ownerGhostNameText;

    public void ShowArtifact(ArtifactSO artifact, ContentRegistrySO registry)
    {
        if (artifact == null) return;

        bool unlocked = UnlockManager.instance != null
            && UnlockManager.instance.IsArtifactUnlocked(artifact);

        SetImage(_artifactImage, artifact.icon, unlocked);
        SetText(_artifactNameText, LocalizationManager.GetArtifactName(artifact.id), unlocked);
        SetText(_artifactDescText, LocalizationManager.GetArtifactDesc(artifact.id), unlocked);
        SetText(_passiveNameText, LocalizationManager.GetArtifactPassiveName(artifact.id), unlocked);
        SetText(_passiveDescText, LocalizationManager.GetArtifactPassive(artifact.id), unlocked);

        GhostSO owner = FindOwnerGhost(artifact, registry);
        if (owner != null)
        {
            bool ghostUnlocked = UnlockManager.instance != null
                && UnlockManager.instance.IsGhostUnlocked(owner);
            SetImage(_ownerGhostImage, owner.icon, ghostUnlocked);
            SetText(_ownerGhostNameText, LocalizationManager.GetGhostName(owner.id), ghostUnlocked);
        }
        else
        {
            SetImage(_ownerGhostImage, null, false);
            _ownerGhostNameText.text = "-";
        }
    }

    private GhostSO FindOwnerGhost(ArtifactSO artifact, ContentRegistrySO registry)
    {
        if (registry == null) return null;
        foreach (var ghost in registry.allGhosts)
            if (ghost.artifact == artifact)
                return ghost;
        return null;
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

