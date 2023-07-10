using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyAssets.Rocket
{
    public class RocketController : MonoBehaviour
    {
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private Slider slider;
        [SerializeField] private float fuel;
        [SerializeField] private Image fuelFillImage;
    
        private static readonly int PropulsionAnimHash = Animator.StringToHash("propulsion");
        private bool _deepSpace;
        private float _maxFuel;
        private float _heldTime;

        private void OnBecameInvisible()
        {
            if (_deepSpace) return;
            Invoke(nameof(ResetLevel), 1f);
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

        private void FixedUpdate()
        {
            if (_deepSpace) return;

            if (fuel == 0 && rigidbody2D.velocity.magnitude < 0.01f)
            {
                ResetLevel();
                return;
            }
            
            var propulsion = Input.GetMouseButton(0) && fuel > 0;
            animator.SetBool(PropulsionAnimHash, propulsion);
            if (propulsion)
            {
                rigidbody2D.AddForce(transform.up);

                _heldTime += Time.deltaTime;
            
                fuel -= Time.deltaTime * _heldTime;
                fuel = Mathf.Max(fuel, 0f);
            }
            else
            {
                _heldTime = 0f;
            }
        }

        private void LateUpdate()
        {
            // update UI
            var speed = (int)(rigidbody2D.velocity.magnitude * 1000);
            speedText.text = $"{speed}km/h";
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
            var forwardForce = Mathf.Abs(gravity.magnitude) * 0.5f;
            rigidbody2D.AddForce(transform.up * forwardForce);
        }

        public void Crashed(Planet.Planet planet, Collision2D col)
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
