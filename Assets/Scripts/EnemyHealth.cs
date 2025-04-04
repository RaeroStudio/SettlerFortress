using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
    [Header("Настройки здоровья")]
    public float maxHealth = 50f;
    private float currentHealth;

    void Start() 
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) 
    {
        currentHealth -= damage;
        if (currentHealth <= 0) 
        {
            Die();
        }
    }

    void Die() 
    {
        // Дополнительная логика смерти (анимация, звуки)
        Destroy(gameObject);
    }
}