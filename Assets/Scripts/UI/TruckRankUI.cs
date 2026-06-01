using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TruckRankUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private Slider _expBar;

    private void OnEnable()
    {
        RefreshRate();
    }
    private void Start()
    {
        RefreshRate(); 
    }

    private void RefreshRate()
    {
        if (TruckRankManager.instance == null) return;
        int rank = TruckRankManager.instance.CurrentRank;
        float totalExp = TruckRankManager.instance.TotalExp;

        _rankText.text = $"트럭등급 {rank}";

        if (rank >= RankThresholds.MAX_RANK)
        {
            _expText.text = "트럭등급 MAX";
            if (_expBar != null) _expBar.value = 1f;
            return;
        }

        float prevThreshold = RankThresholds.GetRequiredExp(rank - 1);
        float nextThreshold = RankThresholds.GetRequiredExp(rank);
        float progress = totalExp - prevThreshold;
        float required = nextThreshold - prevThreshold;

        _expText.text = $"{(int)progress} / {(int)required}";
        if (_expBar != null) _expBar.value = required > 0f ? progress / required : 1f;
    }
}
