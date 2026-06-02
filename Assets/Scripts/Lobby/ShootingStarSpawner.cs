using UnityEngine;
using System.Collections;

namespace Lobby
{
    /// <summary>
    /// Spawns shooting stars at random intervals from the top-right to the bottom-left.
    /// </summary>
    public class ShootingStarSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject _starPrefab;
        [SerializeField] private float _minInterval = 0.5f;
        [SerializeField] private float _maxInterval = 1.2f;

        [Header("Position Settings")]
        [SerializeField] private Vector2 _spawnXRange = new Vector2(-7f, 11f);
        [SerializeField] private float _spawnY = 6f;
        [SerializeField] private float _spawnZ = 0.05f;

        [Header("Movement Settings")]
        [SerializeField] private Vector3 _moveDirection = new Vector3(-1f, -0.6f, 0f);
        [SerializeField] private float _minSpeed = 10f;
        [SerializeField] private float _maxSpeed = 22f;

        private System.Collections.Generic.Queue<ShootingStar> _pool = new System.Collections.Generic.Queue<ShootingStar>();

        private void Start()
        {
            if (_starPrefab == null)
            {
                Debug.LogError("ShootingStarSpawner: Star Prefab is not assigned.");
                return;
            }
            // Start two independent spawn routines to increase the number of stars and vary timing.
            StartCoroutine(SpawnRoutine());
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_minInterval, _maxInterval));
                SpawnStar();
            }
        }

        private void SpawnStar()
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(_spawnXRange.x, _spawnXRange.y),
                _spawnY,
                _spawnZ
            );

            ShootingStar star;
            if (_pool.Count > 0 && !_pool.Peek().gameObject.activeSelf)
            {
                star = _pool.Dequeue();
                star.transform.position = spawnPos;
                star.gameObject.SetActive(true);
            }
            else
            {
                GameObject starObj = Instantiate(_starPrefab, spawnPos, Quaternion.identity, transform);
                star = starObj.GetComponent<ShootingStar>();
            }

            if (star != null)
            {
                star.Initialize(_moveDirection, Random.Range(_minSpeed, _maxSpeed));
                _pool.Enqueue(star);
            }
        }
    }
}
