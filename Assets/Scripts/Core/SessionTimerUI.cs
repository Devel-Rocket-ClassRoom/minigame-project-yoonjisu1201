using TMPro;
using UnityEngine;

public class SessionTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private SessionManager _session;

    private void Start()
    {
        _session = SessionManager.instance;
    }
    private void Update()
    {
        if (_session == null) return;

        float remaining = Mathf.Max(0f, _session.RemainingTime); // 주로 음수 방지, 체력 0이하 방지용으로 많이 사용

        if (_timerText != null)
        {
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            _timerText.text = $"{minutes}:{seconds:00}";
        }

    }
}
