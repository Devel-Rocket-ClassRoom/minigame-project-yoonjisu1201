using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TruckRankUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _totalExpText;

    private void OnEnable()
    {
        RefreshRate();
    }

    private void RefreshRate()
    {
        int rank = TruckRankManager.instance.CurrentRank;
        float totalExp = TruckRankManager.instance.TotalExp;
        _rankText.text = $"등급 {rank}";
        _totalExpText.text = $"{totalExp}";
    }
}
