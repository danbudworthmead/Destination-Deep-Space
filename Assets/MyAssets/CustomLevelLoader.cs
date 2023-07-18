using MyAssets.Level_Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public class CustomLevelLoader : MonoBehaviour
    {
        [SerializeField] private GameObject planet;
        
        private static Level _level;

        private void Start()
        {
            if (_level == null) return;
            
            foreach (var prop in _level.props)
            {
                switch (prop.id)
                {
                    case "planet":
                    {
                        SpawnPlanet(prop);
                        break;
                    }
                }
            }
        }

        private void SpawnPlanet(Level.Prop propData)
        {
            var prop = Instantiate(planet, transform);
            prop.transform.position = new Vector3(propData.x / 1000f, propData.y / 1000f, 0f);
        }

        public static void SetLevel(Level level)
        {
            _level = level;
        }

        public void Quit()
        {
            SceneManager.LoadScene("Community Levels");
        }

        public void Passed()
        {
            throw new System.NotImplementedException();
        }
    }
}
