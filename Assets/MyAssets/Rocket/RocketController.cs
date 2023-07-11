using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    
        private static readonly int PropulsionAnimHash = Animator.StringToHash("propulsion");
        private bool _deepSpace;
        private float _maxFuel;
        private float _heldTime;
        private readonly Stopwatch _stopwatch = new();
        
        public float Speed => (int)(rigidbody2D.velocity.magnitude * 1000);
        public float SecondsPassed => _stopwatch.ElapsedMilliseconds / 1000f;

        private void OnBecameInvisible()
        {
            if (_deepSpace) return;
            ResetLevel();
        }

        private void Awake()
        {
            _maxFuel = fuel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
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
            
            var propulsion = Input.GetMouseButton(0) && fuel > 0;
            animator.SetBool(PropulsionAnimHash, propulsion);
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
        }

        public void ReachedDeepSpace()
        {
            if (_deepSpace) return;
            animator.SetBool(PropulsionAnimHash, false);
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
            Invoke(nameof(ResetLevel), 1f);
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void WarpGate(WarpGate warpGate, Collider2D col, float power)
        {
            rigidbody2D.AddForce(warpGate.transform.up * power);
            transform.rotation = warpGate.transform.rotation;
        }
    }
}
