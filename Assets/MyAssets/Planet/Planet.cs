using MyAssets.Rocket;
using UnityEngine;

namespace MyAssets.Planet
{
    public class Planet : MonoBehaviour
    {
        [SerializeField] private new Rigidbody2D rigidbody2D;

        private void Start()
        {
            rigidbody2D.mass *= transform.localScale.x;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var rocket = col.gameObject.GetComponent<RocketController>();
            if (!rocket) return;
            rocket.Crashed(gameObject, col);
        }
    }
}
