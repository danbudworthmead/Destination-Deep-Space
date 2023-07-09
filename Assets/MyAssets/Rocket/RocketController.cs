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
        public bool DeepSpace { get; private set; }
        private float _maxFuel;
        private float _heldTime = 0f;

        private void Awake()
        {
            _maxFuel = fuel;
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        
            if (DeepSpace) return;
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
            animator.SetBool(PropulsionAnimHash, false);
            DeepSpace = true;
            Invoke(nameof(ResetLevel), 2f);
        }

        private void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void AddGravity(Vector2 gravity)
        {
            if (DeepSpace) return;
            rigidbody2D.AddForceAtPosition(gravity, transform.position + transform.up);
        }

        public void Crashed(Planet.Planet planet, Collision2D col)
        {
            Invoke(nameof(ResetLevel), 1f);
        }

        public void WarpGate(WarpGate warpGate, Collider2D col, float power)
        {
            rigidbody2D.AddForce(warpGate.transform.right * power);
        }
    }
}
