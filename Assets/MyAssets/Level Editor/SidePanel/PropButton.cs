using UnityEngine;
using UnityEngine.EventSystems;

namespace MyAssets.Level_Editor.SidePanel
{
    public class PropButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameObject prefab;

        public void Create()
        {
            LevelEditorManager.instance.Spawn(prefab);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Create();
        }
    }
}
