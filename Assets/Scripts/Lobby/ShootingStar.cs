using UnityEngine;

namespace Lobby
{
    /// <summary>
    /// Handles the movement and lifecycle of a shooting star.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShootingStar : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private TrailRenderer _trailRenderer;

        private void Awake()
        {
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        public void Initialize(Vector3 direction, float speed)
        {
            _direction = direction.normalized;
            _speed = speed;
            
            // Look in the direction of movement
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
            }
        }

        private void Update()
        {
            transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
            
            // Deactivate if out of bounds or below the horizon (Y < 0)
            if (transform.position.y < 0f || Mathf.Abs(transform.position.x) > 20f || transform.position.y > 15f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
