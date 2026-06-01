using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GuideUI : MonoBehaviour, IPointerClickHandler
{
    public event System.Action OnClicked;
    [SerializeField] private Image panelImage;
    [SerializeField] private TextMeshProUGUI stepLabel;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI sub1Text;
    [SerializeField] private TextMeshProUGUI sub2Text;
    [SerializeField] private RectTransform dashedCircle;
  
    private Canvas rootCanvas;
    private Camera _cam;

    private CanvasGroup _canvasGroup;
    private void Awake()
    {
        _cam = Camera.main;
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        _canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }
    public void Display(int stepNumber, Transform worldTarget, float circleSize = 150f)
    {
        stepLabel.text = LocalizationManager.GetGuideLabel(stepNumber);
        mainText.text = LocalizationManager.GetGuideMain(stepNumber);
        sub1Text.text = LocalizationManager.GetGuideSub1(stepNumber);
        sub2Text.text = LocalizationManager.GetGuideSub2(stepNumber);

        sub1Text.gameObject.SetActive(true);
        sub2Text.gameObject.SetActive(false);

        dashedCircle.sizeDelta = new Vector2(circleSize, circleSize);

        PositionCircle(worldTarget);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void PositionCircle(Transform worldTarget)
    {
        if (worldTarget == null)
        {
            dashedCircle.gameObject.SetActive(false);
            return;
        }
        dashedCircle.gameObject.SetActive(true);

        Vector3 screenPos = _cam.WorldToScreenPoint(worldTarget.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _cam,
            out Vector2 localPos
        );
        dashedCircle.localPosition = localPos;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }
    public void ShowSub(int index)
    {
        sub1Text.gameObject.SetActive(index == 0);
        sub2Text.gameObject.SetActive(index == 1);
    }
    public void MoveCircle(Transform worldTarget)
    {
        PositionCircle(worldTarget);
    }
    public void SetBlocksRaycast(bool blocks)
    {
        _canvasGroup.blocksRaycasts = blocks;
    }
}
