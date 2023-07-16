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
        [SerializeField] private GameObject ButtonsParent;
        [SerializeField] private GameObject Rocket;
        [SerializeField] private GameObject RocketCanvas;
        
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

        public void Save()
        {
            if (_playMode) return;
            
            var level = new Level
            {
                propCount = transform.childCount
            };
            
            for (var i = 0; i < level.propCount; i++)
            {
                var child = transform.GetChild(i);
                var pos = child.transform.position;
                level.props[i] = new Level.Prop
                {
                    x = (int)pos.x * 1000,
                    y = (int)pos.y * 1000,
                };
            }
            
            var formatter = new BinaryFormatter();
            using var fileStream = File.Create(Application.persistentDataPath + "/level.dat");
            formatter.Serialize(fileStream, level);
            fileStream.Close();

            Debug.Log("Level data saved.");
        }

        public void Load()
        {
            if (_playMode) return;
            
            DeleteAllProps();
            
            if (File.Exists(Application.persistentDataPath + "/level.dat"))
            {
                var formatter = new BinaryFormatter();
                using var fileStream = File.Open(Application.persistentDataPath + "/level.dat", FileMode.Open);
                var level = (Level)formatter.Deserialize(fileStream);
                fileStream.Close();

                level.propCount = 0;
                foreach (var prop in level.props)
                {
                    if (prop == null) return;
                    var pos = new Vector2
                    {
                        x = prop.x / 1000f,
                        y = prop.y / 1000f,
                    };
                    Spawn(pos);
                }
                
                Debug.Log("Level data loaded.");
            }
            else
            {
                Debug.Log("No level data found.");
            }
        }

        public void Play()
        {
            ButtonsParent.SetActive(false);
            _rocket = Instantiate(Rocket);
            _rocketCanvas = Instantiate(RocketCanvas);
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
            throw new NotImplementedException();
        }

        public void Failed()
        {
            ButtonsParent.SetActive(true);
            Destroy(_rocket);
            Destroy(_rocketCanvas);
            _playMode = false;
        }
    }
}
