using System;
using MyAssets.Rocket;
using UnityEngine;
using UnityEngine.UI;

namespace MyAssets.UI
{
    public class FuelSlider : MonoBehaviour
    {
        [SerializeField] private Image fuelFillImage;
        
        private RocketController _rocketController;
        private Slider _slider;
        private float _maxFuel;
        
        private void Start()
        {
            _rocketController = FindObjectOfType<RocketController>();
            _maxFuel = _rocketController.Fuel;
            _slider = GetComponentInChildren<Slider>();
        }

        private void LateUpdate()
        {
            var fuel = _rocketController.Fuel;
            
            _slider.value = fuel / _maxFuel;
            if (fuel <= 0f)
            {
                if (fuelFillImage != null)
                {
                    fuelFillImage.enabled = false;
                }
            }
        }
    }
}
