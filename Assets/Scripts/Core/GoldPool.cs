using UnityEngine;
using UnityEngine.Pool;

//골드 오브젝트 3개 만들어두고 재활용. 
public class GoldPool : MonoBehaviour
{
    public static GoldPool instance {  get; private set; }

    [SerializeField] private GoldDropEffect _prefab;
    [SerializeField] private int _poolSize = 3;

    //private List<GoldDropEffect> _pool = new();
    private ObjectPool<GoldDropEffect> _objectPool;

    private void Awake()
    {
        instance = this;
        //for (int i = 0; i < _poolSize; i++)
        //{
        //    GoldDropEffect obj = Instantiate(_prefab);
        //    obj.gameObject.SetActive(false);
        //    _pool.Add(obj);
        //}
        _objectPool = new ObjectPool<GoldDropEffect>(
            CreateGold, OnGetGold, OnReleaseGold, OnDestroyGold, true,
            _poolSize, _poolSize);
    }
    private GoldDropEffect CreateGold()
    {
        return Instantiate(_prefab);
    }
    private void OnGetGold(GoldDropEffect obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseGold(GoldDropEffect obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyGold(GoldDropEffect obj)
    {
        Destroy(obj.gameObject);
    }

    public void Spawn(Vector3 position, int amount)
    {
        //foreach (var gold in _pool)
        //{
        //    if (!gold.gameObject.activeSelf)
        //    {
        //        gold.transform.position = position;
        //        gold.gameObject.SetActive(true);
        //        gold.Setup(amount);
        //        return;
        //    }
        //}
        GoldDropEffect gold = _objectPool.Get();
        gold.transform.position = position;
        gold.Setup(amount, ReturnToPool);
    }
    private void ReturnToPool(GoldDropEffect gold)
    {
        _objectPool.Release(gold);
    }
}
