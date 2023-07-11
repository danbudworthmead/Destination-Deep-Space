using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Input = UnityEngine.Input;

namespace MyAssets.Level_Editor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [SerializeField] private GameObject planetPrefab;
        private Level _level = new();
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Spawn(mouseWorldPos);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveLevel();
            } 
            else if (Input.GetKeyDown(KeyCode.R))
            {
                LoadLevel();
            }
        }

        private void SaveLevel()
        {
            var formatter = new BinaryFormatter();
            using var fileStream = File.Create(Application.persistentDataPath + "/level.dat");
            formatter.Serialize(fileStream, _level);

            Debug.Log("Level data saved.");
        }

        private void LoadLevel()
        {
            if (File.Exists(Application.persistentDataPath + "/level.dat"))
            {
                var formatter = new BinaryFormatter();
                using var fileStream = File.Open(Application.persistentDataPath + "/level.dat", FileMode.Open);
                _level = (Level)formatter.Deserialize(fileStream);
                Debug.Log("Level data loaded.");
            }
            else
            {
                Debug.Log("No level data found.");
            }
        }

        private void Spawn(Vector2 position)
        {
            var prop = Instantiate(planetPrefab);
            prop.transform.position = position;
            _level.props[_level.propCount++] = new Level.Prop
            {
                x = (int)(position.x * 1000),
                y = (int)(position.y * 1000),
            };
        }
    }
}
