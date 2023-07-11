using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyAssets.Asteroid
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject asteroidPrefab;
        [SerializeField] private BoxCollider2D spawnArea;
        [SerializeField] private float minFrequency = 1f;
        [SerializeField] private float maxFrequency = 3f;
        [SerializeField] private float minSize = 1f;
        [SerializeField] private float maxSize = 3f;
        [SerializeField] private float minForce = 1f;
        [SerializeField] private float maxForce = 4f;

        private void Start()
        {
            Invoke(nameof(Spawn), 0f);
        }

        private void Spawn()
        {
            var asteroid = Instantiate(asteroidPrefab, transform);
            var pos = GetRandomPoint(spawnArea);
            var size = Random.Range(minSize, maxSize);
            asteroid.transform.position = pos;
            asteroid.transform.localScale = Vector3.one * size;
            
            var rb = asteroid.GetComponent<Rigidbody2D>();
            var force = Random.Range(minForce, maxForce);
            rb.velocity = transform.up * force;
            rb.AddTorque(Random.Range(-10, 10));

            var delay = Random.Range(minFrequency, maxFrequency);
            Invoke(nameof(Spawn), delay);
        }

        private Vector3 GetRandomPoint(BoxCollider2D col)
        {
            var bounds = col.bounds;
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector3(x, y, 0);
        }
    }
}
