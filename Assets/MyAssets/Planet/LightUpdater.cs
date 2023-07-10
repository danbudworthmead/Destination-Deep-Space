using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MyAssets.Planet
{
    [ExecuteInEditMode]
    public class LightUpdater : MonoBehaviour
    {
        [SerializeField] private Light2D light;
        private void Update()
        {
            var scale = transform.localScale.x;
            light.pointLightInnerRadius = scale;
            light.pointLightOuterRadius = scale * 2;
        }
    }
}
