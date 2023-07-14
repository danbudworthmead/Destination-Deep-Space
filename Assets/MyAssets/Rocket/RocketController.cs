using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MyAssets.Rocket
{
    public class RocketController : MonoBehaviour
    {
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private Animator animator;
        [SerializeField] private Slider slider;
        [SerializeField] private float fuel;
        [SerializeField] private Image fuelFillImage;
        [SerializeField] private TrailRenderer fire;
        [SerializeField] private GameObject brokenPartPrefab;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private bool _visible;
        private bool _deepSpace;
        private float _maxFuel;
        private float _heldTime;
        private Vector2 _prevVel;
        private readonly Stopwatch _stopwatch = new();
        
        public float Speed => (int)(rigidbody2D.velocity.magnitude * 1000);
        public float SecondsPassed => _stopwatch.ElapsedMilliseconds / 1000f;
        public float Magnitude => rigidbody2D.velocity.magnitude;

        private void OnBecameVisible()
        {
            _visible = true;
        }

        private void OnBecameInvisible()
        {
            _visible = false;
            if (_deepSpace) return;
            Invoke(nameof(ResetLevel), 0.25f);
        }

        private void Awake()
        {
            _maxFuel = fuel;
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void FixedUpdate()
        {
            if (_deepSpace)
            {
                rigidbody2D.rotation = 90f;
                rigidbody2D.AddForce(Vector2.right * 10f);
                transform.localScale = new Vector3(
                    transform.localScale.x * 0.8f, 
                     Mathf.Min(transform.localScale.y * 1.2f, 3f),
                    transform.localScale.z);
                fire.enabled = false;
                return;
            }

            if (fuel == 0 && rigidbody2D.velocity.magnitude < 0.05f)
            {
                ResetLevel();
                return;
            }

            var propulsion = Input.GetMouseButton(0)
                             && fuel > 0
                             && spriteRenderer.enabled;
            if (propulsion)
            {
                if (!_stopwatch.IsRunning)
                {
                    _stopwatch.Start();
                }
                
                fire.emitting = true;
                rigidbody2D.AddForce(transform.up);

                _heldTime += Time.deltaTime;

                var usage = Time.deltaTime * _heldTime;
                usage = Mathf.Max(usage, 0.01f);
                fuel -= usage;
                fuel = Mathf.Max(fuel, 0f);
            }
            else
            {
                fire.emitting = false;
                _heldTime = 0f;
            }
        }

        private void LateUpdate()
        {
            // update UI
            slider.value = fuel / _maxFuel;
            if (fuel <= 0f)
            {
                fuelFillImage.enabled = false;
            }

            _prevVel = rigidbody2D.velocity;
        }

        public void ReachedDeepSpace()
        {
            if (!_visible) return;
            if (_deepSpace) return;
            _deepSpace = true;
            rigidbody2D.velocity = Vector2.zero;
            _stopwatch.Stop();
            Invoke(nameof(NextLevel), 2f);
        }

        private void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void NextLevel()
        {
            var level = SceneManager.GetActiveScene().buildIndex + 1;
            if (level >= SceneManager.sceneCountInBuildSettings)
            {
                level = 0;
            }
            SceneManager.LoadScene(level);
        }

        public void AddGravity(Vector2 gravity)
        {
            if (_deepSpace) return;
            rigidbody2D.AddForceAtPosition(gravity, transform.position + transform.up);
            var forwardForce = Mathf.Abs(gravity.magnitude) * 0.65f;
            rigidbody2D.AddForce(transform.up * forwardForce);
        }

        public void Crashed(GameObject obj, Collision2D col)
        {
            BreakApart();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<Collider2D>().enabled = false;
            Invoke(nameof(ResetLevel), 2f);
        }

        private void BreakApart()
        {
            spriteRenderer.enabled = false;

            var texture = spriteRenderer.sprite.texture;
            var width = texture.width;
            var height = texture.height;
            var spriteBounds = spriteRenderer.sprite.bounds;

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var col = texture.GetPixel(x, y);
                    if (col.a < 0.5f) continue;
            
                    var brokenPart = Instantiate(brokenPartPrefab);
            
                    // Calculate position in world space
                    var pixelPos = new Vector2(x / (float)width, y / (float)height);
                    var worldPos = transform.TransformPoint(spriteBounds.min + Vector3.Scale(spriteBounds.size, pixelPos));
                    brokenPart.transform.position = worldPos;

                    brokenPart.GetComponent<SpriteRenderer>().color = col;

                    Vector2 force = _prevVel * Random.Range(5f, 20f);
                    brokenPart.GetComponent<Rigidbody2D>().AddForce(force);
                }
            }
        }


        public void WarpGate(WarpGate warpGate, Collider2D col, float power)
        {
            rigidbody2D.AddForce(warpGate.transform.up * power);
            transform.rotation = warpGate.transform.rotation;
        }

        public void StartTeleport()
        {
            foreach (var trail in GetComponentsInChildren<TrailRenderer>())
            {
                trail.emitting = false;
            }

            // transform.localScale = Vector3.zero;
            animator.SetBool("teleport", true);
        }

        public void EndTeleport()
        {
            foreach (var trail in GetComponentsInChildren<TrailRenderer>())
            {
                trail.emitting = true;
            }
            
            // transform.localScale = Vector3.one;
            animator.SetBool("teleport", false);
        }

        public void SetVelocity(Vector3 vel)
        {
            rigidbody2D.velocity = vel;
        }
    }
}
