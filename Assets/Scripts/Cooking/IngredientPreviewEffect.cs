using UnityEngine;
using System.Collections;

public class IngredientPreviewEffect : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Vector3 _originLocalPos;
    private Coroutine _coroutine;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _originLocalPos = transform.localPosition;
        gameObject.SetActive(false);
    }
    public void Play(Sprite icon)
    {
        transform.localPosition = _originLocalPos;
        _renderer.color = Color.white;
        _renderer.sprite = icon;
        gameObject.SetActive(true);

        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CoAnimate());
    }
    private IEnumerator CoAnimate()
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 endPos = _originLocalPos + Vector3.up * 0.5f;
        Color color = _renderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(_originLocalPos, endPos, t);
            color.a = 1f - t;
            _renderer.color = color;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
