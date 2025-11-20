using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    // Assignment 씬 이름 그대로 적기 (Hierarchy 상단에 있는 씬 이름)
    public string mainSceneName = "Assignment";

    public void OnClickStart()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
