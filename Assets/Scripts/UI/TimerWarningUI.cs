using System.Collections;
using TMPro;
using UnityEngine;

public class TimerWarningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _fadeDuration = 0.8f;
    [SerializeField] private float _minAlpha = 0.2f;

    private void OnEnable()
    {
        StartCoroutine(CoBlinkLoop());
    }

    private IEnumerator CoBlinkLoop()
    {
        while (true)
        {
            yield return CoFade(_minAlpha, 1f);
            yield return CoFade(1f, _minAlpha);
        }
    }

    private IEnumerator CoFade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Color color = _text.color;
            color.a = Mathf.Lerp(from, to, elapsed / _fadeDuration);
            _text.color = color;
            yield return null;
        }
    }
}
