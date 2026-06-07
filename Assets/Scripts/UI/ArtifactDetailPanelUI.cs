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
    [SerializeField] private Image _passiveIcon;
    [SerializeField] private TextMeshProUGUI _passiveNameText;
    [SerializeField] private TextMeshProUGUI _passiveDescText;

    [Header("전용 손님")]
    [SerializeField] private Image _ownerGhostImage;

    public void ShowArtifact(ArtifactSO artifact, ContentRegistrySO registry)
    {
        if (artifact == null) return;

        bool unlocked = UnlockManager.instance != null
            && UnlockManager.instance.IsArtifactUnlocked(artifact);

        SetImage(_artifactImage, artifact.icon, unlocked);
        SetText(_artifactNameText, LocalizationManager.GetArtifactName(artifact.id), unlocked);
        SetText(_artifactDescText, LocalizationManager.GetArtifactDesc(artifact.id), unlocked);
        bool abilityUnlocked = UnlockManager.instance != null && UnlockManager.instance.IsAbilityUnlocked(artifact);
        string passiveDesc = LocalizationManager.GetArtifactPassive(artifact.id);
        if (abilityUnlocked && passiveDesc == "???")
            passiveDesc = LocalizationManager.Get("ui_ending_text");

        SetImage(_passiveIcon, artifact.passiveIcon, unlocked);
        SetText(_passiveNameText, LocalizationManager.GetArtifactPassiveName(artifact.id), unlocked);
        SetText(_passiveDescText, passiveDesc, unlocked);

        GhostSO owner = FindOwnerGhost(artifact, registry);
        if (owner != null)
        {
            bool artifactCollected = UnlockManager.instance != null
                && UnlockManager.instance.GetArtifacCount(artifact) > 0;
            SetImage(_ownerGhostImage, owner.icon, artifactCollected);
        }
        else
        {
            SetImage(_ownerGhostImage, null, false);
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

