using System.Collections;
using MyAssets.Rocket;
using UnityEngine;

namespace MyAssets.Worm_Hole
{
    public class Wormhole : MonoBehaviour
    {
        [SerializeField] private Wormhole target;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!target) return;
            var rocket = col.GetComponent<RocketController>();
            if (!rocket) return;
            target.StartCoroutine(target.CoTeleport(rocket));
        }

        private void Teleport(RocketController rocket, float magnitude)
        {
            rocket.transform.position = transform.position;
            rocket.transform.rotation = transform.rotation;
            rocket.SetVelocity(transform.up.normalized * magnitude);
        }

        private IEnumerator CoTeleport(RocketController rocket)
        {
            var magnitude = rocket.Magnitude;
            rocket.StartTeleport();
            yield return new WaitForSeconds(0.25f);
            Teleport(rocket, magnitude);
            yield return new WaitForSeconds(0.25f);
            rocket.EndTeleport();
        }
    }
}
