using System.Threading.Tasks;
using MyAssets.Level_Editor;
using MyAssets.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyAssets
{
    public class CustomLevelLoader : MonoBehaviour
    {
        [SerializeField] private GameObject planet;
        [SerializeField] private GameObject votingCanvas;
        
        private static string _levelID;
        private static Level _level;

        public static void SetLevel(string levelID, Level level)
        {
            _levelID = levelID;
            _level = level;
        }

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

        public void Quit()
        {
            SceneManager.LoadScene("Community Levels");
        }

        public void Passed()
        {
            votingCanvas.SetActive(true);
        }

        public async void Vote(int score)
        {
            foreach (var button in votingCanvas.GetComponentsInChildren<Button>())
            {
                button.interactable = false;
            }

            var uri = $"{Constants.Uri}/vote/{_levelID}/{score}";
            
            using var webRequest = new UnityWebRequest(uri, "POST");
            webRequest.uploadHandler = new UploadHandlerRaw(null);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Set the Content-Type header to indicate JSON data
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            var asyncOperation = webRequest.SendWebRequest();
            
            // Handle the response
            while (!webRequest.isDone)
            {
                await Task.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Voted successfully");
            }
            else
            {
                foreach (var button in votingCanvas.GetComponentsInChildren<Button>())
                {
                    button.interactable = true;
                }
                
                // Request failed, handle the error
                Debug.LogError($"{webRequest.result}: {webRequest.error} {webRequest.downloadHandler.text}");
            }
        }
    }
}
