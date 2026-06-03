using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class GuideUI : MonoBehaviour, IPointerClickHandler
{
    public event System.Action OnClicked;
    [SerializeField] private Image panelImage;
    [SerializeField] private TextMeshProUGUI stepLabel;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI sub1Text;
    [SerializeField] private TextMeshProUGUI sub2Text;
    [SerializeField] private RectTransform dashedCircle;

    private float _currentCircleSize;

    private Canvas rootCanvas;
    private Camera _cam;

    private CanvasGroup _canvasGroup;

    private Coroutine _shrinkCoroutine;
    private void Awake()
    {
        _cam = Camera.main;
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        _canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }
    public void Display(int stepNumber, Transform worldTarget, float circleSize = 150f)
    {
        _currentCircleSize = circleSize;

        stepLabel.text = LocalizationManager.GetGuideLabel(stepNumber);
        mainText.text = LocalizationManager.GetGuideMain(stepNumber);
        sub1Text.text = LocalizationManager.GetGuideSub1(stepNumber);
        sub2Text.text = LocalizationManager.GetGuideSub2(stepNumber);

        sub1Text.gameObject.SetActive(true);
        sub2Text.gameObject.SetActive(false);

        PositionCircle(worldTarget);
        gameObject.SetActive(true);

        if (_shrinkCoroutine != null) StopCoroutine(_shrinkCoroutine);
        _shrinkCoroutine = StartCoroutine(CoShrinkCircle(circleSize));
    }
    private IEnumerator CoShrinkCircle(float targetSize)
    {
        float startSize = targetSize * 2f;
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float size = Mathf.Lerp(startSize, targetSize, t);
            dashedCircle.sizeDelta = new Vector2(size, size);
            yield return null;
        }
        dashedCircle.sizeDelta = new Vector2(targetSize, targetSize);
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
        if (_cam == null) _cam = Camera.main;
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
        if (_shrinkCoroutine != null) StopCoroutine(_shrinkCoroutine);
        _shrinkCoroutine = StartCoroutine(CoShrinkCircle(_currentCircleSize));
    }
    public void SetBlocksRaycast(bool blocks)
    {
        _canvasGroup.blocksRaycasts = blocks;
    }
}
