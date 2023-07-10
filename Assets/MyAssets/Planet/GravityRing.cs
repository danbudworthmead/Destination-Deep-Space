using MyAssets.Rocket;
using TMPro;
using UnityEngine;

namespace MyAssets.Planet
{
    public class GravityRing : MonoBehaviour
    {
        [SerializeField] private TMP_Text gravityText;
        [SerializeField] private new CircleCollider2D collider2D;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        
        private float _gravity;
        
        private void OnTriggerStay2D(Collider2D other)
        {
            var rocket = other.GetComponent<RocketController>();
            if (!rocket) return;

            var planetPos = transform.position;
            var rocketPos = rocket.transform.position;

            var dist = Vector2.Distance(planetPos, rocketPos);
            var radius = collider2D.radius * transform.lossyScale.x;
            _gravity = (1 - Mathf.Clamp01(dist / radius)) * rigidbody2D.mass;
            
            var direction = planetPos - rocketPos;
            rocket.AddGravity(direction.normalized * _gravity);
        }

        private void LateUpdate()
        {
            gravityText.text = $"{_gravity:F2}";
        }
    }
}
