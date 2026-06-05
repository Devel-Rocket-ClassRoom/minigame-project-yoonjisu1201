using System.Collections.Generic;
using UnityEngine;

public class ArtifactDropPool : MonoBehaviour
{
    public static ArtifactDropPool instance { get; private set; }

    [SerializeField] private ArtifactDropEffect _prefab;
    [SerializeField] private int _poolSize = 3;

    private List<ArtifactDropEffect> _pool = new();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < _poolSize; i++)
        {
            var obj = Instantiate(_prefab);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
        }
    }

    public void Spawn(Vector3 position, Sprite icon)
    {
        foreach (var effect in _pool)
        {
            if (!effect.gameObject.activeSelf)
            {
                effect.transform.position = position;
                effect.gameObject.SetActive(true);
                effect.Setup(icon);
                return;
            }
        }
    }
}
