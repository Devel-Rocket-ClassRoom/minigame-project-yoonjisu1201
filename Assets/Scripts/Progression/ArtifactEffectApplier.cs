using UnityEngine;

// UnlockManager의 OnAbilityUnlocked 이벤트를 받아 GameContext에 유물 효과를 적용
public class ArtifactEffectApplier : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;

    private void Start()
    {
        // 씬 로드 시 이미 해금된 능력들 재적용
        foreach (var artifact in _registry.allArtifacts)
        {
            if (UnlockManager.instance.IsAbilityUnlocked(artifact))
                ApplyEffect(artifact);
        }
    }

    private void OnEnable()
    {
        UnlockManager.instance.OnAbilityUnlocked += ApplyEffect;
    }
    private void OnDisable()
    {
        if (UnlockManager.instance != null)
            UnlockManager.instance.OnAbilityUnlocked -= ApplyEffect;
    }

    private void ApplyEffect(ArtifactSO artifact)
    {
        switch (artifact.effectType)
        {
            case ArtifactEffectType.ExitSpeedBoost:
                GameContext.exitSpeedMultiplier += artifact.effectValue;
                break;

            case ArtifactEffectType.FailedFoodConsolation:
                GameContext.consolationChance = artifact.effectValue;
                GameContext.consolationGold = Mathf.RoundToInt(artifact.effectValue2);
                break;

            case ArtifactEffectType.SpoilTimeExtension:
                GameContext.spoilTimeMultiplier += artifact.effectValue;
                break;

            case ArtifactEffectType.PatienceBoost:
                GameContext.customerPatienceMultiplier += artifact.effectValue;
                break;

            case ArtifactEffectType.SessionEndBonus:
                GameContext.sessionEndBonusGoldPerDish = Mathf.RoundToInt(artifact.effectValue);
                break;

            case ArtifactEffectType.FirstCookSpeedBoost:
                GameContext.firstCookSpeedMultiplier = artifact.effectValue;
                break;

            case ArtifactEffectType.ConsecutiveSatisfactionBonus:
                GameContext.consecutiveSatisfiedRequired = Mathf.RoundToInt(artifact.effectValue);
                GameContext.consecutiveSatisfiedBonus = Mathf.RoundToInt(artifact.effectValue2);
                break;

            case ArtifactEffectType.PatienceRefill:
                GameContext.patienceRefillChance = artifact.effectValue;
                break;
        }

        Debug.Log($"유물 효과 적용: {artifact.id} ({artifact.effectType})");
    }
}
