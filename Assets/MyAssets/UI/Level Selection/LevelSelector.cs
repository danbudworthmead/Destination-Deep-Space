using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MyAssets.UI.Level_Selection
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private Rocket rocket;
        private Collider2D[] _starColliders;
        
        private void Start()
        {
            var rng = new System.Random(0);
            var unlocked = 7;
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = unlocked;
            _starColliders = new Collider2D[transform.childCount];
            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                _starColliders[i] = transform.GetComponent<Collider2D>();
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

            var firstRocketPos = transform.GetChild(0).transform.position;
            rocket.MoveTo(firstRocketPos);
            rocket.transform.position = firstRocketPos;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                foreach (var starCollider in _starColliders)
                {
                    if (starCollider.bounds.Contains(mousePos))
                    {
                        rocket.MoveTo(starCollider.transform.position);
                    }
                }
            }
        }
    }
}
