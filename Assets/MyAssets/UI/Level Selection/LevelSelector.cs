using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace MyAssets.UI.Level_Selection
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private Rocket rocket;
        [SerializeField] private TMP_Text levelText;
        private Collider2D[] _starColliders;
        private Camera _camera;

        private int _unlocked;

        private int _selectedLevel;
        private Collider2D _lastHovered;

        private void Start()
        {
            SaveManager.Load();
            _unlocked = SaveManager.GetHighestUnlockedLevel();
            _camera = Camera.main;
            var rng = new System.Random(0);
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = _unlocked;
            _starColliders = new Collider2D[_unlocked];
            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (i < _unlocked)
                {
                    lineRenderer.SetPosition(i, child.transform.position);
                    _starColliders[i] = child.GetComponent<Collider2D>();
                }
                else
                {
                    child.gameObject.SetActive(false);
                }

                var light2D = child.GetComponentInChildren<Light2D>();
                light2D.intensity = 5;
                var lightColour = Color.HSVToRGB(0.05f * rng.Next(20), 0.3f, 1f);
                light2D.color = lightColour;

                var spriteRenderer = child.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.color = lightColour;

                spriteRenderer.transform.localScale = new Vector3(
                    rng.Next(50, 150) * 0.01f,
                    rng.Next(50, 150) * 0.01f,
                    1f);
                
                var rotation = rng.Next(36) * 10;
                spriteRenderer.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.back);
                if (i >= _unlocked)
                {
                    spriteRenderer.color /= 10;
                } 
                else if (i == _unlocked - 1)
                {
                    spriteRenderer.color /= 2;
                }
            }

            var firstRocketPos = transform.GetChild(0).transform.position;
            rocket.MoveTo(firstRocketPos);
            rocket.transform.position = firstRocketPos;

            GoToLastStar();
        }

        private void GoToLastStar()
        {
            if (_lastHovered == null)
            {
                _lastHovered = _starColliders.Last();
            }
        }

        private void Update()
        {
            var hovered = GetStarHovered();
            if (hovered != null)
            {
                _lastHovered = hovered;
            }

            hovered = _lastHovered;
            
            var nearest = GetNearestStar();
            
            if (hovered == null || nearest == null) return;

            foreach (var starCollider in _starColliders)
            {
                if (!starCollider.gameObject.activeInHierarchy) break;
                starCollider.GetComponentInChildren<Light2D>().intensity = starCollider == hovered ? 10 : 5;
            }

            _selectedLevel = hovered.transform.GetSiblingIndex();
            levelText.text = $"Level: {_selectedLevel + 1}";

            if (_selectedLevel > nearest.transform.GetSiblingIndex())
            {
                rocket.MoveTo(_starColliders[nearest.transform.GetSiblingIndex() + 1].transform.position);
            }
            else if (_selectedLevel < nearest.transform.GetSiblingIndex())
            {
                rocket.MoveTo(_starColliders[nearest.transform.GetSiblingIndex() - 1].transform.position);
            }
        }

        public void PlayLevel()
        {
            SceneManager.LoadScene($"Level {_selectedLevel}");
        }

        private Collider2D GetNearestStar()
        {
            var nearestDist = 0.1f;
            Collider2D nearestStar = null;
            foreach (var starCollider in _starColliders)
            {
                var dist = Vector2.Distance(starCollider.transform.position, rocket.transform.position);
                if (dist > nearestDist) continue;
                nearestDist = dist;
                nearestStar = starCollider;
            }

            return nearestStar;
        }

        private Collider2D GetStarHovered()
        {
            if (!Input.GetMouseButton(0)) return null;
            
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            foreach (var starCollider in _starColliders)
            {
                if (starCollider.transform.GetSiblingIndex() >= _unlocked) break;
                if (starCollider.OverlapPoint(mousePosition)) return starCollider;
            }

            return null;
        }

        public void Quit()
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
