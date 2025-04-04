using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;
    
    [Header("UI")]
    public Text goldText;
    
    private int currentGold;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        goldText.text = $"Золото: {currentGold}";
    }
}