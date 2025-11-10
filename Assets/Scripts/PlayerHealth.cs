using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 3;
    
    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Player Health: {currentHealth}/{maxHealth}");
    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 죽는 함수
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died! Game Over!");
        
        // 게임 오버 UI 표시
        ShowGameOver();
        
        // 게임 일시정지
        Time.timeScale = 0f;
    }

    private void ShowGameOver()
    {
        Debug.Log("=== GAME OVER START ===");
        
        // Canvas 찾기
        Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        Debug.Log($"Canvas: {(canvas != null ? "Found" : "Not Found")}");
        
        if (canvas == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }

        try
        {
            // 게임오버 텍스트 생성
            GameObject textObj = new GameObject("GameOverText");
            textObj.transform.SetParent(canvas.transform, false);
            
            Text gameOverText = textObj.AddComponent<Text>();
            gameOverText.text = "GAME OVER";
            
            // 폰트 설정 - 기본 폰트 사용
            Font[] fonts = Resources.FindObjectsOfTypeAll<Font>();
            if (fonts.Length > 0)
            {
                gameOverText.font = fonts[0];
            }
            
            gameOverText.fontSize = 100;
            gameOverText.fontStyle = FontStyle.Bold;
            gameOverText.alignment = TextAnchor.MiddleCenter;
            gameOverText.color = Color.red;
            
            RectTransform rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            Debug.Log("=== GAME OVER TEXT CREATED ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating GAME OVER text: {e.Message}");
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}