using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
    }
}
