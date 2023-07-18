using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace MyAssets.UI.CommunityMaps
{
    public class CommunityMapsButtons : MonoBehaviour
    {
        [SerializeField] private GameObject levelButton;
        [SerializeField] private GameObject loading;
        [SerializeField] private string request;
        
        private const string Uri = "localhost:30674";

        private void OnEnable()
        {
            loading.SetActive(true);
            Populate();
        }

        private void OnDisable()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private async void Populate()
        {
            using var webRequest = UnityWebRequest.Get($"{Uri}/{request}");
            var asyncOperation = webRequest.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            // Check for errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                loading.GetComponent<TMP_Text>().text = webRequest.error;
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                // Request completed successfully
                var data = webRequest.downloadHandler.text;
                var levels = JsonUtility.FromJson<LevelData>(data);
                loading.SetActive(false);
                await CreateButtons(levels);
                Debug.Log($"Response: {data}");
            }
        }

        private async Task CreateButtons(LevelData levelData)
        {
            var levels = levelData.maps;
            
            var height = levelButton.GetComponent<RectTransform>().sizeDelta.y;
            var rect = GetComponent<RectTransform>();
            
            
            foreach (var level in levels)
            {
                var button = Instantiate(levelButton, transform);
                button.transform.Find("ID").GetComponent<TMP_Text>().text = $"{level.id}";
                button.transform.Find("Name").GetComponent<TMP_Text>().text = level.name;
                button.transform.Find("Score").GetComponent<TMP_Text>().text = $"{level.score}";
                
                var size = rect.sizeDelta;
                size.y = height * transform.childCount;
                rect.sizeDelta = size;
                
                await Task.Delay(10);
            }
        }
    }
}
