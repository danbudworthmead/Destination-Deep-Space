using MyAssets.Rocket;
using TMPro;
using UnityEngine;

namespace MyAssets.UI
{
    public class TimerText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private RocketController _rocket;

        private void Awake()
        {
            _rocket = FindObjectOfType<RocketController>();
        }

        private void LateUpdate()
        {
            text.text = $"{_rocket.SecondsPassed:F2}";
        }
    }
}
