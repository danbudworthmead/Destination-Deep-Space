using System;
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
            target.Teleport(rocket);
        }

        private void Teleport(RocketController rocket)
        {
            rocket.transform.position = transform.position;
            rocket.transform.rotation = transform.rotation;
            
            var rb = rocket.GetComponent<Rigidbody2D>();
            rb.velocity = transform.up.normalized * rb.velocity.magnitude;
        }
    }
}
