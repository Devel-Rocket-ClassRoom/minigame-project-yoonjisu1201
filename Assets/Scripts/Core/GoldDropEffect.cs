using System.Collections;
using TMPro;
using UnityEngine;

public class GoldDropEffect : MonoBehaviour
{
    [SerializeField] private TextMeshPro _amountText;
    [SerializeField] private float _liftTime = 2.5f;

    public void Setup(int amount)
    {
        _amountText.text = $"+{amount}G";
        StartCoroutine(CoAutoSetActive());
    }
    private IEnumerator CoAutoSetActive()
    {
        yield return new WaitForSeconds(_liftTime);
        gameObject.SetActive(false);
    }
}
