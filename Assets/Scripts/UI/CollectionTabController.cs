using UnityEngine;
using UnityEngine.UI; // Button

[System.Serializable]
public class TabButtonData
{
    public Button button;
    public GameObject activeImage;
    public GameObject inactiveImage;
}
public class CollectionTabController : MonoBehaviour
{
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TabButtonData[] _tabButtons;
    [SerializeField] private float _activeScale = 1.1f;

    private void Start()
    {
        for (int i = 0; i < _tabButtons.Length; i++)
        {
            // 루프 반복마다 새 변수 생성. 그냥 i만 사용하면 모든 버튼이 마지막 인덱스로되버림
            int index = i;
            _tabButtons[i].button.onClick.AddListener(() => SwitchTab(index));
        }
        SwitchTab(0);
    }
    public void SwitchTab(int index)
    {
        for (int i = 0; i < _panels.Length; i++)
            _panels[i].SetActive(i == index);

        for (int i = 0; i < _tabButtons.Length; i++)
        {
            bool isActive = i == index;
            var data = _tabButtons[i];

            data.activeImage.SetActive(isActive);
            data.inactiveImage.SetActive(!isActive);
            data.button.transform.localScale = isActive ? Vector3.one * _activeScale : Vector3.one;
        }
    }
    public void Open()
    {
        gameObject.SetActive(true);
        SwitchTab(0);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
