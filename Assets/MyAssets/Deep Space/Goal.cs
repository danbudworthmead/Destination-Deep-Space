using MyAssets.Rocket;
using UnityEngine;

namespace MyAssets.Deep_Space
{
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            var rocket = col.GetComponent<RocketController>();
            if (rocket)
            {
                rocket.ReachedDeepSpace();
            }
        }
    }
}
