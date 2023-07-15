using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;

namespace MyAssets.Level_Editor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [SerializeField] private GameObject planetPrefab;
        
        private void Update()
        {
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
                    x = (int)pos.x / 1000,
                    y = (int)pos.y / 1000,
                };
            }
            
            var formatter = new BinaryFormatter();
            using var fileStream = File.Create(Application.persistentDataPath + "/level.dat");
            formatter.Serialize(fileStream, level);

            Debug.Log("Level data saved.");
        }

        public void Load()
        {
            DeleteAllProps();
            
            if (File.Exists(Application.persistentDataPath + "/level.dat"))
            {
                var formatter = new BinaryFormatter();
                using var fileStream = File.Open(Application.persistentDataPath + "/level.dat", FileMode.Open);
                var level = (Level)formatter.Deserialize(fileStream);

                level.propCount = 0;
                foreach (var prop in level.props)
                {
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
    }
}
