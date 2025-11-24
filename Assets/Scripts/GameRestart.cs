using UnityEngine;

public class GameRestart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        //LevelManager.Instance.LoadSceneAsync("Assignment");
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assignment");
    }
}
