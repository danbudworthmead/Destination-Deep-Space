using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets.Background
{
    public class BackgroundStarsParticleSystem : MonoBehaviour
    {
        private void Awake()
        {
            var particles = GetComponent<ParticleSystem>();
            particles.randomSeed = (uint)SceneManager.GetActiveScene().buildIndex;
            particles.Play();
        }
    }
}
