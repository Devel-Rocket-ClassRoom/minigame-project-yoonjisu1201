using System.Collections.Generic;
using UnityEngine;

//골드 오브젝트 3개 만들어두고 재활용. 
public class GoldPool : MonoBehaviour
{
    public static GoldPool instance {  get; private set; }

    [SerializeField] private GoldDropEffect _prefab;
    [SerializeField] private int _poolSize = 3;

    private List<GoldDropEffect> _pool = new();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < _poolSize; i++)
        {
            GoldDropEffect obj = Instantiate(_prefab);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
        }
    }
    public void Spawn(Vector3 position, int amount)
    {
        foreach (var gold in _pool)
        {
            if (!gold.gameObject.activeSelf)
            {
                gold.transform.position = position;
                gold.gameObject.SetActive(true);
                gold.Setup(amount);
                return;
            }
        }
    }
}
