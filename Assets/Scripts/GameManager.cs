using UnityEngine;
using System.Collections.Generic;
using GameObject = UnityEngine.GameObject;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Transform> gamePathWaypoints = new List<Transform>();

    private Dictionary<GameObject, float> dyingEnemies = new Dictionary<GameObject, float>();
    
    private List<GameObject> enemies = new List<GameObject>();

    private const float EnemyDeathDuration = 0.3f;

    private int currentWave = 0;
    private int currentEnemyInWave = 0;
    private List<int> enemiesForCurrentWave;

    private int playerHealth;
    public int PlayerHealth
    {
        get { return playerHealth; }
        set
        {
            playerHealth = value;
            OnScoreChanged?.Invoke(playerHealth);
        }
    }

    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;
    public float spawnInterval = 2f;

    private float timeSinceLastSpawn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Instance.PlayerHealth = 100;
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
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            switch (currentEnemyInWave)
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
                default:
                    currentEnemyInWave += 1;
                    break;
            }


            timeSinceLastSpawn = 0;
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
        Instance.PlayerHealth -= 10;
    }

    public List<GameObject> GetAllEnemies() {
        return enemies;
    }

    public void KillEnemy(GameObject enemy)
    {
        dyingEnemies.Add(enemy, EnemyDeathDuration);

        FollowGamePath followGamePath = enemy.GetComponent<FollowGamePath>();
        Destroy(followGamePath);
    }
    
    public void DestroyEnemy(GameObject enemy)
    {
        dyingEnemies.Remove(enemy);
        enemies.Remove(enemy);
        Destroy(enemy);
    }
}
