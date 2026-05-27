using UnityEngine;
using UnityEngine.UI;
using TMPro;

//손님 카드 1장 담당. 손님 데이터를 받아서 해금 여부에 따라 아이콘, 이름 표시
public class GhostCollectionCardUI : MonoBehaviour
{
    [SerializeField] private Image _ghostimage;
    [SerializeField] private TextMeshProUGUI _ghostNameText;

    private static readonly Color LOCKED_COLOR = Color.black;
    private static readonly Color UNLOCKED_COLOR = Color.white;

    private GhostSO _ghostData;
    private Button _button;
    private System.Action<GhostSO> _onSelected;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    //GhostCollectionPanelUI 에서 실행할 메서드
    public void Setup(GhostSO ghost, System.Action<GhostSO> onSelected)
    {
        _ghostData = ghost;
        _onSelected = onSelected;
        Refresh();
    }
    private void Refresh()
    {
        if (_ghostData == null) return;

        bool unlocked = UnlockManager.instance != null
            && UnlockManager.instance.IsGhostUnlocked(_ghostData);

        _ghostimage.sprite = _ghostData.icon;
        _ghostimage.color = unlocked ? UNLOCKED_COLOR : LOCKED_COLOR;
        _ghostNameText.text = unlocked ? LocalizationManager.GetGhostName(_ghostData.id) : "???";
        _button.interactable = true;

    }
    //눌렀을때 상세정보 창이랑 연결
    private void OnClicked()
    {
        if (_ghostData != null)
        _onSelected?.Invoke(_ghostData);
    }

}
