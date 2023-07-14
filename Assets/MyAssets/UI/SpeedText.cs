using MyAssets.Rocket;
using TMPro;
using UnityEngine;

namespace MyAssets.UI
{
    public class SpeedText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private RocketController _rocket;

        private void Awake()
        {
            _rocket = FindObjectOfType<RocketController>();
        }

        private void LateUpdate()
        {
            text.text = _rocket.Speed == 0 ? string.Empty : $"{_rocket.Speed}km/h";
        }
    }
}
