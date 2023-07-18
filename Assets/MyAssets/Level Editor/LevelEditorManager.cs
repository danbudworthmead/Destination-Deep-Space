using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyAssets.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
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
        [SerializeField] private GameObject submitPanel;
        [SerializeField] private TMP_Text levelName;
        [SerializeField] private TMP_Text errorText;

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
            buttonsParent.SetActive(false);
            submitPanel.SetActive(true);
        }

        public void Failed()
        {
            buttonsParent.SetActive(true);
            Destroy(_rocket);
            Destroy(_rocketCanvas);
            _playMode = false;
        }

        public async void SubmitLevel()
        {
            errorText.text = string.Empty;
            if (levelName.text == string.Empty)
            {
                errorText.text = "Please enter a name for your level";
                return;
            }

            var props = new List<Level.Prop>();
            for (var idx = 0; idx < transform.childCount; idx++)
            {
                var child = transform.GetChild(idx);
                props.Add(new Level.Prop
                {
                    id = "planet",
                    x = (int)(child.transform.position.x * 1000f),
                    y = (int)(child.transform.position.y * 1000f),
                });
            }

            var level = new Level()
            {
                name = levelName.text,
                props = props.ToArray(),
            };

            var json = JsonUtility.ToJson(level);

            // Create a byte array from the JSON data
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using var webRequest = new UnityWebRequest($"{Constants.Uri}/map", "POST");
            webRequest.uploadHandler = new UploadHandlerRaw(jsonBytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Set the Content-Type header to indicate JSON data
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            var asyncOperation = webRequest.SendWebRequest();

            // Handle the response
            while (!webRequest.isDone)
            {
                errorText.text = $"Uploading... {webRequest.uploadProgress}%";
                await Task.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Request completed successfully
                errorText.text = "Level submitted successfully.";
            }
            else
            {
                // Request failed, handle the error
                errorText.text = $"{webRequest.result}: {webRequest.error} {webRequest.downloadHandler.text}";
            }
        }
    }
}
