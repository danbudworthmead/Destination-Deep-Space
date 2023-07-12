using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MyAssets.UI.Level_Selection
{
    public class LevelSelector : MonoBehaviour
    {
        private void Start()
        {
            var rng = new System.Random(0);
            var unlocked = 7;
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = unlocked;
            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);

                if (i < unlocked)
                {
                    lineRenderer.SetPosition(i, child.transform.position);
                }

                var light2D = child.GetComponentInChildren<Light2D>();
                light2D.intensity = i >= unlocked ? 5 : 10;
                var lightColour = Color.HSVToRGB(0.05f * rng.Next(20), 0.3f, 1f);
                light2D.color = lightColour;

                var spriteRenderer = child.GetComponentInChildren<SpriteRenderer>();
                var rotation = rng.Next(36) * 10;
                spriteRenderer.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.back);
                if (i >= unlocked)
                {
                    spriteRenderer.color /= 10;
                } 
                else if (i == unlocked - 1)
                {
                    spriteRenderer.color /= 6;
                }
            }
        }
    }
}
