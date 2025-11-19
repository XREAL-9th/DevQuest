using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float lifeTime = 1.5f;

    private TextMeshProUGUI damageText;
    private Color startColor;
    private float timer;

    void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        if (damageText == null)
        {
            Debug.LogError("DamageTextAnimator requires a TextMeshProUGUI component.");
            enabled = false;
        }
        startColor = damageText.color;
    }

    public void SetDamageValue(float damage)
    {
        damageText.text = Mathf.CeilToInt(damage).ToString();
        damageText.color = startColor;
        timer = lifeTime;
    }

    void Update()
    {
        Debug.Log(".");
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        float alpha = timer / lifeTime;

        Color currentColor = damageText.color;
        currentColor.a = alpha;
        damageText.color = currentColor;

        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
