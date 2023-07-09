using MyAssets.Rocket;
using UnityEngine;

public class WarpGate : MonoBehaviour
{
    [SerializeField] private float power;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        var rocket = col.GetComponent<RocketController>();
        if (rocket)
        {
            rocket.WarpGate(this, col, power);
        }
    }
}
