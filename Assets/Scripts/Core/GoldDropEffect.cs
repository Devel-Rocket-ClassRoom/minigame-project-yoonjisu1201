using System.Collections;
using TMPro;
using UnityEngine;

public class GoldDropEffect : MonoBehaviour
{
    [SerializeField] private TextMeshPro _amountText;
    [SerializeField] private float _liftTime = 2.5f;

    private System.Action<GoldDropEffect> _onFinished;

    public void Setup(int amount, System.Action<GoldDropEffect> onFinished)
    {
        _amountText.text = $"+{amount}G";
        _onFinished = onFinished;
        StartCoroutine(CoAutoSetActive());
    }
    private IEnumerator CoAutoSetActive()
    {
        yield return new WaitForSeconds(_liftTime);
        _onFinished?.Invoke(this);
    }
}
