using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyAssets.Networking;
using MyAssets.Planet;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Input = UnityEngine.Input;

namespace MyAssets.Level_Editor
{
    public class LevelEditorManager : MonoBehaviour
    {
        public static LevelEditorManager instance;
        
        [SerializeField] private GameObject planetPrefab;
        [SerializeField] private GameObject editModeButtons;
        [SerializeField] private GameObject rocket;
        [SerializeField] private GameObject rocketCanvas;
        [SerializeField] private GameObject submitPanel;
        [SerializeField] private TMP_Text levelName;
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private Button uploadButton;
        [SerializeField] private GameObject propPanel;
        [SerializeField] private GameObject editPanel;
        [SerializeField] private TMP_Text propCountText;
        
        [SerializeField] private Slider scaleSlider;
        [SerializeField] private Slider gravRingSlider;

        private bool _playMode;
        private GameObject _rocket;
        private GameObject _rocketCanvas;
        private Vector2 _mouseWorldPosition;
        
        private PlacedProp _selectedProp;
        private PlacedProp _draggedProp;
        private readonly List<PlacedProp> _placedProps = new();
        private Vector2 _dragOffset;

        private const int MaxProps = 10;
        private int _propCount;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            propPanel.SetActive(true);
            editPanel.SetActive(false);
            UpdatePropCountText();
        }

        private void Update()
        {
            if (_playMode) return;
            UpdateMousePosition();
            UpdatePropPosition();
            UpdateSelectedProp();
        }

        private void UpdateSliders()
        {
            if (_selectedProp == null) return;
            scaleSlider.value = _selectedProp.transform.localScale.x;
            gravRingSlider.value = _selectedProp.GetComponentInChildren<GravityRing>().transform.localScale.x;
        }

        private void UpdateSidePanel()
        {
            if (_selectedProp)
            {
                ShowEditPanel();
            }
            else
            {
                ShowPropsPanel();
            }
        }

        public void ShowPropsPanel()
        {
            propPanel.SetActive(true);
            editPanel.SetActive(false);
            _selectedProp = null;
        }

        public void ShowEditPanel()
        {
            propPanel.SetActive(false);
            editPanel.SetActive(true);
        }

        private void UpdatePropCountText()
        {
            propCountText.text = $"{_propCount}/{MaxProps}";
        }

        private void UpdateSelectedProp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                _draggedProp = null;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                var minDist = 1.0f;
                foreach (var prop in _placedProps)
                {
                    var dist = Vector2.Distance(prop.transform.position, _mouseWorldPosition);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        _draggedProp = prop;
                    }
                }

                if (_draggedProp != null)
                {
                    _dragOffset = (Vector2)_draggedProp.transform.position - _mouseWorldPosition;
                    _selectedProp = _draggedProp;
                    ShowEditPanel();
                    UpdateSliders();
                }
            }
        }

        private void UpdatePropPosition()
        {
            if (_draggedProp == null) return;
            _draggedProp.transform.position = _mouseWorldPosition + _dragOffset;
        }

        private void UpdateMousePosition()
        {
            var mousePos = Input.mousePosition;
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        }

        public void SpawnProp(GameObject prefab)
        {
            if (_propCount >= MaxProps) return;
            var prop = Instantiate(prefab, transform);
            prop.transform.position = _mouseWorldPosition;
            _selectedProp = prop.AddComponent<PlacedProp>();
            _placedProps.Add(_selectedProp);
            _propCount++;
            UpdatePropCountText();
        }

        public void DeleteProp()
        {
            _placedProps.Remove(_selectedProp);
            Destroy(_selectedProp.gameObject);
            ShowPropsPanel();
            _propCount--;
            UpdatePropCountText();
        }

        public void Play()
        {
            editModeButtons.SetActive(false);
            propPanel.SetActive(true);
            editPanel.SetActive(false);
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
            editModeButtons.SetActive(false);
            submitPanel.SetActive(true);
        }

        public void Failed()
        {
            editModeButtons.SetActive(true);
            Destroy(_rocket);
            Destroy(_rocketCanvas);
            _playMode = false;
        }

        public async void SubmitLevel()
        {
            uploadButton.interactable = false;
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
                    scale = (int)(child.localScale.x * 1000f),
                    gravityRadius = (int)(child.GetComponentInChildren<GravityRing>().transform.localScale.x * 1000f),
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

            using var webRequest = new UnityWebRequest($"{Constants.Uri}/submitmap", "POST");
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
                uploadButton.interactable = true;
            }
        }

        public void SetScale(Slider slider)
        {
            _selectedProp.transform.localScale = Vector3.one * slider.value;
        }
        
        public void SetGravityRingRadius(Slider slider)
        {
            var ring = _selectedProp.GetComponentInChildren<GravityRing>();
            ring.transform.localScale = Vector3.one * slider.value;
        }
    }
}