using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets.UI.CommunityMaps
{
    public class CommunityMapsCanvas : MonoBehaviour
    {
        public void Quit()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void CreateNewMap()
        {
            SceneManager.LoadScene("Level Editor");
        }
    }
}
