using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets.UI.MainMenu
{
    public class MainMenuButtons : MonoBehaviour
    {
        public void Campaign()
        {
            SceneManager.LoadScene("Level Selection");
        }
    }
}
