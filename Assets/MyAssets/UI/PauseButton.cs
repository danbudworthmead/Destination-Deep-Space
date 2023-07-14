using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets.UI
{
    public class PauseButton : MonoBehaviour
    {
        public void Pause()
        {
            Time.timeScale = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
            // gameObject.SetActive(false);
        }
    }
}
