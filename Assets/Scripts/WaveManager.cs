using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    
    [Header("Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float preparationTime = 60f;
    public int baseEnemies = 3;
    public float difficultyMultiplier = 1.15f;

    [Header("UI")]
    public GameObject preparationPanel;
    public Text timerText;

    private int currentWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool waveInProgress;

    void Awake()
    {
        Instance = this;
        preparationPanel.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(1f);
        while(true)
        {
            // Фаза подготовки
            preparationPanel.SetActive(true);
            float timer = preparationTime;
            
            while(timer > 0)
            {
                timerText.text = $"До следующей волны: {Mathf.CeilToInt(timer)}";
                timer -= Time.deltaTime;
                yield return null;
            }
            
            preparationPanel.SetActive(false);

            // Старт волны
            currentWave++;
            SpawnWave();
            
            // Ожидание окончания волны
            yield return new WaitUntil(() => activeEnemies.Count == 0);
            
            // Награда за волну
            GoldManager.Instance.AddGold(Random.Range(10, 100));
            Citadel.Instance.AddSettlers(Random.Range(1, 5));
        }
    }

    void SpawnWave()
    {
        int enemiesToSpawn = Mathf.RoundToInt(baseEnemies * Mathf.Pow(difficultyMultiplier, currentWave));
        
        for(int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(enemy);
            
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            enemyAI.OnDeath += HandleEnemyDeath;
        }
        
        UpdateUI();
    }

    void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        UpdateUI();
    }

    void UpdateUI()
    {
        Citadel.Instance.UpdateWaveUI(currentWave, activeEnemies.Count);
    }
}