using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MyAssets.Level_Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        [SerializeField] private Transform partsParent;
        [SerializeField] private AudioSource rocketSound;

        public bool Visible { get; private set; }
        private bool _deepSpace;
        private float _maxFuel;
        private float _heldTime;
        private readonly Stopwatch _stopwatch = new();
        
        private LevelEditorManager _levelEditorManager;
        private CustomLevelLoader _customLevelLoader;
        
        public float Speed => (int)(rigidbody2D.velocity.magnitude * 1000);
        public float SecondsPassed => _stopwatch.ElapsedMilliseconds / 1000f;
        public float Magnitude => rigidbody2D.velocity.magnitude;

        private void OnBecameVisible()
        {
            Visible = true;
            rigidbody2D.velocity = Vector2.right;
        }

        private void OnBecameInvisible()
        {
            Visible = false;
            if (_deepSpace) return;
            Invoke(nameof(ResetLevel), 0.25f);
        }

        private void Awake()
        {
            _maxFuel = fuel;
            _levelEditorManager = FindObjectOfType<LevelEditorManager>();
            _customLevelLoader = FindObjectOfType<CustomLevelLoader>();
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void FixedUpdate()
        {
            if (!Visible)
            {
                rigidbody2D.AddForce(Vector2.right);
            }
            
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

                _heldTime += Time.fixedDeltaTime;

                var usage = Time.fixedDeltaTime * _heldTime;
                usage = Mathf.Max(usage, 0.01f);
                fuel -= usage;
                fuel = Mathf.Max(fuel, 0f);
            }
            else
            {
                fire.emitting = false;
                _heldTime = 0f;
                rocketSound.Stop();
            }
        }

        private void LateUpdate()
        {
            // TODO: this is really shit code, fix it
            if (slider == null)
            {
                slider = FindObjectOfType<Slider>();
                return;
            }
            
            slider.value = fuel / _maxFuel;
            if (fuel <= 0f)
            {
                if (fuelFillImage != null)
                {
                    fuelFillImage.enabled = false;
                }
            }
        }

        public void ReachedDeepSpace()
        {
            if (!Visible) return;
            if (_deepSpace) return;
            _deepSpace = true;
            rigidbody2D.velocity = Vector2.zero;
            _stopwatch.Stop();
            if (_levelEditorManager)
            {
                _levelEditorManager.Passed();
                return;
            }
            if (_customLevelLoader)
            {
                _customLevelLoader.Passed();
                return;
            }
            Invoke(nameof(NextLevel), 2f);
            SaveManager.Unlock(SceneManager.GetActiveScene().buildIndex, _stopwatch.ElapsedMilliseconds);
        }

        private void ResetLevel()
        {
            if (_levelEditorManager)
            {
                _levelEditorManager.Failed();
                return;
            }
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
            _stopwatch.Stop();
            BreakApart();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<Collider2D>().enabled = false;
            Invoke(nameof(ResetLevel), 2f);
        }

        private void BreakApart()
        {
            if (_levelEditorManager) return;
            spriteRenderer.enabled = false;
            foreach (var part in partsParent.GetComponentsInChildren<Rigidbody2D>())
            {
                AddExplosionForce(part, 1000f, transform.position, 100f);
            }
            partsParent.DetachChildren();
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
        
        public static void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
        {
            var explosionDir = rb.position - explosionPosition;
            var explosionDistance = (explosionDir.magnitude / explosionRadius);

            // Normalize without computing magnitude again
            if (upwardsModifier == 0)
            {
                explosionDir /= explosionDistance;
            }
            else
            {
                // If you pass a non-zero value for the upwardsModifier parameter, the direction
                // will be modified by subtracting that value from the Y component of the centre point.
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }

            rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
        }
    }
}
