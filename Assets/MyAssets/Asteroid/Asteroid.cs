using MyAssets.Rocket;
using UnityEngine;

namespace MyAssets.Asteroid
{
    public class Asteroid : MonoBehaviour
    {
        private bool _canSuicide;

        private void OnBecameVisible()
        {
            _canSuicide = true;
        }

        private void OnBecameInvisible()
        {
            if (_canSuicide)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var rocket = col.gameObject.GetComponent<RocketController>();
            if (!rocket) return;
            rocket.Crashed(gameObject, col);
        }
    }
}
