using UnityEngine;
using UnityEngine.UI;

public class Citadel : MonoBehaviour 
{
    public static Citadel Instance;

    [Header("Настройки")]
    [SerializeField] private int maxSettlers = 10;
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private GameObject settlerPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("UI")]
    [SerializeField] private Text settlersText;
    [SerializeField] private Slider healthSlider;

    private int currentSettlers;
    private float health;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        currentSettlers = 5;
        health = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        UpdateHealthUI();
        
        if(health <= 0) Defeat();
    }

    public void AddSettlers(int amount)
    {
        if(settlerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Не настроен префаб или точка спавна!");
            return;
        }

        int settlersToAdd = Mathf.Min(amount, maxSettlers - currentSettlers);
        if(settlersToAdd <= 0) return;

        currentSettlers += settlersToAdd;
        
        for(int i = 0; i < settlersToAdd; i++)
        {
            Instantiate(settlerPrefab, spawnPoint.position, Quaternion.identity);
        }
        
        UpdateUI();
    }

    void UpdateHealthUI()
    {
        if(healthSlider != null)
            healthSlider.value = health / maxHealth;
    }

    void UpdateUI()
    {
        if(settlersText != null)
            settlersText.text = $"Поселенцы: {currentSettlers}/{maxSettlers}";
    }

    void Defeat()
    {
        Debug.Log("Цитадель уничтожена!");
        Time.timeScale = 0;
        // Дополнительная логика при поражении
    }
}