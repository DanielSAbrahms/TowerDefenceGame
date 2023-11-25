using UnityEngine;
using System.Collections.Generic;
using GameObject = UnityEngine.GameObject;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Transform> gamePathWaypoints = new List<Transform>();

    private Dictionary<GameObject, float> dyingEnemies = new Dictionary<GameObject, float>();
    
    private List<GameObject> enemies = new List<GameObject>();

    private const float EnemyDeathDuration = 0.3f;

    private GameOverDisplay gameOverDisplay;
    private HealthDisplay healthDisplay;
    private MoneyDisplay moneyDisplay;

    private int currentWave = 0;
    private int currentEnemyInWaveIndex;
    private List<int> enemiesForCurrentWave;

    private bool isGamePlaying = true;
    
    public int playerStarterHealth;
    public int playerStarterMoney;

    private int playerHealth;
    public int PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            playerHealth = value;
            OnHealthChanged?.Invoke(playerHealth);
        }
    }
    
    private int playerMoney;
    public int PlayerMoney
    {
        get { return playerMoney; }
        set
        {
            playerMoney = value;
            OnMoneyChanged?.Invoke(playerMoney);
        }
    }

    public delegate void HealthChanged(int newHealth);
    public event HealthChanged OnHealthChanged;
    
    public delegate void MoneyChanged(int newAmount);
    public event MoneyChanged OnMoneyChanged;
    
    public float spawnInterval = 2f;

    private float timeSinceLastSpawn;

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
        WavePlan wavePlan = gameObject.GetComponent<WavePlan>();
        enemiesForCurrentWave = wavePlan.GetEnemiesInWave(currentWave);

        gameOverDisplay = GameObject.Find("GameOverDisplay").GetComponent<GameOverDisplay>();
        gameOverDisplay.gameObject.SetActive(false);
        
        healthDisplay = GameObject.Find("HealthDisplay").GetComponent<HealthDisplay>();
        healthDisplay.gameObject.SetActive(true);
        
        moneyDisplay = GameObject.Find("MoneyDisplay").GetComponent<MoneyDisplay>();
        moneyDisplay.gameObject.SetActive(true);
        
        Instance.PlayerHealth = playerStarterHealth;
        Instance.PlayerMoney = playerStarterMoney;
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (isGamePlaying)
        {
            if (timeSinceLastSpawn >= spawnInterval)
            {
                if (currentEnemyInWaveIndex >= enemiesForCurrentWave.Count)
                {
                    EndWave();
                }
                else
                {
                    switch (enemiesForCurrentWave[currentEnemyInWaveIndex])
                    {
                        case 0:
                            SpawnObject(Resources.Load<GameObject>("Prefabs/EnemyCube"));
                            break;
                        case 1:
                            SpawnObject(Resources.Load<GameObject>("Prefabs/SmallEnemyCube"));
                            break;
                        case 2:
                            SpawnObject(Resources.Load<GameObject>("Prefabs/HugeEnemyCube"));
                            break;
                    }

                    currentEnemyInWaveIndex += 1;
                    timeSinceLastSpawn = 0;
                }
            }


            Dictionary<GameObject, float> dyingEnemiesCopy = dyingEnemies;
            dyingEnemies = new Dictionary<GameObject, float>();

            foreach (KeyValuePair<GameObject, float> enemy in dyingEnemiesCopy)
            {
                GameObject enemyObject = enemy.Key;
                float timeLeft = enemy.Value;

                timeLeft -= Time.deltaTime;

                if (timeLeft <= 0) DestroyEnemy(enemyObject);
                else dyingEnemies.Add(enemyObject, timeLeft);
            }
        }
    }

    private void SpawnObject(GameObject objectPrefab)
    {
        Transform spawnPoint = gamePathWaypoints[0];
        
        var targetOffset = new Vector3(spawnPoint.transform.position.x, 
            spawnPoint.transform.position.y + (objectPrefab.transform.localScale.y / 2), 
            spawnPoint.transform.position.z);
        // Instantiate the object
        // You can change the position and rotation to suit your needs
        // Instantiate(objectPrefab, targetOffset, Quaternion.identity);

        GameObject newObject = Instantiate(objectPrefab, targetOffset, Quaternion.identity);

        FollowGamePath followGamePath = newObject.AddComponent<FollowGamePath>();
        followGamePath.waypoints = gamePathWaypoints;
        
        // Get the script attached to the new object
        EnemyPrefab enemyPrefab = newObject.GetComponent<EnemyPrefab>();
        
        if (enemyPrefab != null)
        {
            enemyPrefab.Initialize(gamePathWaypoints);
        }

        enemies.Add(newObject);
    }
    public void EnemyCompletePath(GameObject enemy) {
        enemies.Remove(enemy);
        Destroy(enemy);
        Instance.PlayerHealth -= enemy.GetComponent<EnemyPrefab>().penalty;

        if (Instance.PlayerHealth <= 0)
        {
            GameOver();
        }
    }

    public List<GameObject> GetAllEnemies() {
        return enemies;
    }

    // Killing an enemy stops them, but allows a small period of animation
    // Prevents enemies disappearing instantly upon taking enough damage
    public void KillEnemy(GameObject enemy)
    {
        dyingEnemies.Add(enemy, EnemyDeathDuration);
        playerMoney += enemy.GetComponent<EnemyPrefab>().reward;
        FollowGamePath followGamePath = enemy.GetComponent<FollowGamePath>();
        Destroy(followGamePath);
    }
    
    // Destroying an enemy removes them from the game entirely
    public void DestroyEnemy(GameObject enemy)
    {
        dyingEnemies.Remove(enemy);
        enemies.Remove(enemy);
        Destroy(enemy);
    }

    public void GameOver()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        isGamePlaying = false;

        healthDisplay.gameObject.SetActive(false);
        gameOverDisplay.gameObject.SetActive(true);
    }

    private void EndWave()
    {
        // TODO Add multiple Wave logic
        isGamePlaying = false;
    }
}
