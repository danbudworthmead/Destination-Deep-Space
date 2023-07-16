using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets.Background
{
    [ExecuteInEditMode]
    public class BackgroundStarsParticleSystem : MonoBehaviour
    {
        private void Awake()
        {
            var particles = GetComponent<ParticleSystem>();
            particles.Stop();
            particles.randomSeed = (uint)SceneManager.GetActiveScene().buildIndex;
            particles.Play();
        }
    }
}
