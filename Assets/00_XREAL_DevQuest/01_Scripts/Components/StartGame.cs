using UnityEngine;

namespace XREAL
{
    public class StartGame : MonoBehaviour
    {
        public void GameStart()
        {
            Singleton.Level.LoadSceneAsync("Assignment_VR_2");
        }
    }
}
