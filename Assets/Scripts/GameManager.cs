using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void HealthChanged(int newHealth);

    public delegate void MoneyChanged(int newAmount);

    public int playerStarterHealth;
    public int playerStarterMoney;

    public int towerCost;

    public float spawnInterval = 2f;
    public List<Transform> gamePathWaypoints = new();
    private int currentEnemyInWaveIndex;
    private readonly int currentWave = 0;

    private readonly List<GameObject> enemies = new();
    private List<int> enemiesForCurrentWave;

    private GameOverDisplay gameOverDisplay;
    private HealthDisplay healthDisplay;
    private bool isGamePlaying = true;
    private MoneyDisplay moneyDisplay;

    private int playerHealth;

    private int playerMoney;
    private float timeSinceLastSpawn;
    public static GameManager Instance { get; private set; }
    private WavePlan wavePlan;
    

    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            OnHealthChanged?.Invoke(playerHealth);
        }
    }

    public int PlayerMoney
    {
        get => playerMoney;
        set
        {
            playerMoney = value;
            OnMoneyChanged?.Invoke(playerMoney);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        wavePlan = gameObject.GetComponent<WavePlan>();
        enemiesForCurrentWave = wavePlan.GetEnemiesInWave(currentWave);

        gameOverDisplay = GameObject.Find("GameOverDisplay").GetComponent<GameOverDisplay>();
        gameOverDisplay.gameObject.SetActive(false);

        healthDisplay = GameObject.Find("HealthDisplay").GetComponent<HealthDisplay>();
        healthDisplay.gameObject.SetActive(true);

        moneyDisplay = GameObject.Find("MoneyDisplay").GetComponent<MoneyDisplay>();
        moneyDisplay.gameObject.SetActive(true);

        PlayerHealth = playerStarterHealth;
        healthDisplay.UpdateHealthDisplay(PlayerHealth);
        PlayerMoney = playerStarterMoney;
        moneyDisplay.UpdateMoneyDisplay(PlayerMoney);
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (isGamePlaying)
            if (timeSinceLastSpawn >= spawnInterval)
            {
                if (currentEnemyInWaveIndex >= enemiesForCurrentWave.Count)
                {
                    EndWave();
                }
                else
                {
                    GameObject enemyToSpawn = wavePlan.GetEnemyToSpawn(enemiesForCurrentWave[currentEnemyInWaveIndex]);
                    if (!enemyToSpawn.IsUnityNull()) SpawnObject(enemyToSpawn);
                    
                    currentEnemyInWaveIndex += 1;
                    timeSinceLastSpawn = 0;
                }
            }
    }

    public event HealthChanged OnHealthChanged;
    public event MoneyChanged OnMoneyChanged;

    private void SpawnObject(GameObject objectPrefab)
    {
        var spawnPoint = gamePathWaypoints[0];

        var targetOffset = new Vector3(spawnPoint.transform.position.x,
            spawnPoint.transform.position.y + objectPrefab.transform.localScale.y / 2,
            spawnPoint.transform.position.z);
        // Instantiate the object
        // You can change the position and rotation to suit your needs
        // Instantiate(objectPrefab, targetOffset, Quaternion.identity);

        var newObject = Instantiate(objectPrefab, targetOffset, Quaternion.identity);

        var followGamePath = newObject.AddComponent<FollowGamePath>();
        followGamePath.waypoints = gamePathWaypoints;

        // Get the script attached to the new object
        var enemyPrefab = newObject.GetComponent<EnemyPrefab>();

        if (!enemyPrefab.IsUnityNull()) enemyPrefab.Initialize(gamePathWaypoints);

        enemies.Add(newObject);
    }

    public void EnemyCompletePath(GameObject enemy)
    {
        // Penalize player for enemy survival
        PlayerHealth -= enemy.GetComponent<EnemyPrefab>().penalty;

        enemies.Remove(enemy);
        Destroy(enemy);

        if (PlayerHealth <= 0) GameOver();
    }

    public List<GameObject> GetAllEnemies()
    {
        return enemies;
    }

    // Destroying an enemy removes them from the game entirely
    public void DestroyEnemy(GameObject enemy)
    {
        // Give player money for destroying enemy
        PlayerMoney += enemy.GetComponent<EnemyPrefab>().reward;

        enemies.Remove(enemy);
        Destroy(enemy);
    }

    public void GameOver()
    {
        foreach (var enemy in enemies) Destroy(enemy);

        isGamePlaying = false;

        healthDisplay.gameObject.SetActive(false);
        gameOverDisplay.gameObject.SetActive(true);
    }

    private void EndWave()
    {
        // TODO Add multiple Wave logic
        isGamePlaying = false;
    }

    public bool TrySpawnTower(Vector3 clickPosition)
    {
        if (PlayerMoney >= towerCost)
        {
            SpawnTowerAtPosition(clickPosition);
            PlayerMoney -= towerCost;
            return true;
        }

        return false;
        // TODO: Notify the player of insufficient funds
    }

    public void SpawnTowerAtPosition(Vector3 position)
    {
        var towerPrefab = Resources.Load<GameObject>("Prefabs/Tower");
        Instantiate(towerPrefab, position, Quaternion.identity);
    }
}