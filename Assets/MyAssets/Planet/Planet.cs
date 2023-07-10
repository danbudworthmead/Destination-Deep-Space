using MyAssets.Rocket;
using TMPro;
using UnityEngine;

namespace MyAssets.Planet
{
    public class Planet : MonoBehaviour
    {
        [SerializeField] private TMP_Text gravityText;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private CircleCollider2D gravityField;

        private float _magnitude;
        private RocketController _rocketController;

        private void Awake()
        {
            _rocketController = FindObjectOfType<RocketController>();
            rigidbody2D.mass *= transform.localScale.x;
        }

        private void FixedUpdate()
        {
            if (!gravityField.bounds.Contains(_rocketController.transform.position)) return;
            var direction = (transform.position - _rocketController.transform.position) * 0.1f;
            _magnitude = (1 - direction.magnitude) * rigidbody2D.mass;
            _magnitude = Mathf.Max(_magnitude, 0f);
            _rocketController.AddGravity(direction.normalized * _magnitude);
        }

        private void LateUpdate()
        {
            gravityText.text = $"{_magnitude:F2}";
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject == _rocketController.gameObject)
            {
                _rocketController.Crashed(this, col);
            }
        }
    }
}
