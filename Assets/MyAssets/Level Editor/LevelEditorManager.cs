using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Input = UnityEngine.Input;

namespace MyAssets.Level_Editor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [SerializeField] private GameObject planetPrefab;
        [SerializeField] private GameObject buttonsParent;
        [SerializeField] private GameObject rocket;
        [SerializeField] private GameObject rocketCanvas;
        
        private bool _playMode;
        private GameObject _rocket;
        private GameObject _rocketCanvas;

        private void Update()
        {
            if (_playMode) return;
            var noUiSelected = EventSystem.current.currentSelectedGameObject == null;
            if (Input.GetMouseButtonDown(0) && noUiSelected)
            {
                var mousePos = Input.mousePosition;
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Spawn(mouseWorldPos);
            }
        }

        public void Play()
        {
            buttonsParent.SetActive(false);
            _rocket = Instantiate(rocket);
            _rocketCanvas = Instantiate(rocketCanvas);
            _rocket.transform.position = new Vector3(-12f, 0f, 0f);
            _playMode = true;
        }

        private void DeleteAllProps()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void Spawn(Vector2 position)
        {
            var prop = Instantiate(planetPrefab, transform);
            prop.transform.position = position;
        }
        
        public void Quit()
        {
            SceneManager.LoadScene("Community Levels");
        }

        public void Passed()
        {
            Invoke(nameof(PassedInternal), 1f);
        }

        private void PassedInternal()
        {
            Destroy(_rocket);
            Destroy(_rocketCanvas);
        }

        public void Failed()
        {
            buttonsParent.SetActive(true);
            Destroy(_rocket);
            Destroy(_rocketCanvas);
            _playMode = false;
        }
    }
}
