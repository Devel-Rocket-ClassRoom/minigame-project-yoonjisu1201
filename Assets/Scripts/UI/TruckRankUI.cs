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
    private void Start()
    {
        //도감UI가 켜질 때마다 등급 정보 새로고침
        RefreshRate(); 
    }

    private void RefreshRate()
    {
        if (TruckRankManager.instance == null) return;
        int rank = TruckRankManager.instance.CurrentRank;
        float totalExp = TruckRankManager.instance.TotalExp;
        _rankText.text = $"등급 {rank}";
        _totalExpText.text = $"{totalExp}";
    }
}
