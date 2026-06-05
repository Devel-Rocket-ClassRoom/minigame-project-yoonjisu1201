using System.Collections;
using UnityEngine;

public class ArtifactDropEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private float _displayTime = 2.5f;

    private Coroutine _coroutine;

    public void Setup(Sprite icon)
    {
        _iconRenderer.sprite = icon;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CoAutoSetActive());
    }

    private IEnumerator CoAutoSetActive()
    {
        yield return new WaitForSeconds(_displayTime);
        gameObject.SetActive(false);
    }
}
