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
    public GameObject objectPrefab;
    public GameObject spawnPoint;
    public float spawnInterval = 2f;

    public int totalMaxHealth = 200;
    public int totalMinHealth = 50;

    public float baseEnemySpeed = 5f;
    public float enemySpeedScale = 100f;
    public float enemySizeScale = 1f;

    private float timeSinceLastSpawn;

        private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Instance.PlayerHealth = 0;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnObject();
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

    private void SpawnObject()
    {
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
        EnemyPrefab enemyPrefab = newObject.AddComponent<EnemyPrefab>();
        
        float enemyHealth = Random.Range(totalMinHealth, totalMaxHealth);
        
        float enemySpeed = baseEnemySpeed * (enemyHealth / 100) * enemySpeedScale;

        Vector3 healthBarScaleCopy = newObject.GetComponentInChildren<Slider>().gameObject.transform.localScale;
        
        newObject.transform.localScale = newObject.transform.localScale * (enemyHealth / 100) * enemySizeScale;

        newObject.GetComponentInChildren<Slider>().gameObject.transform.localScale = healthBarScaleCopy;
        
        if (enemyPrefab != null)
        {
            enemyPrefab.Initialize(gamePathWaypoints, enemySpeed, 0, enemyHealth);
        }

        enemies.Add(newObject);
    }
    public void EnemyCompletePath(GameObject enemy) {
        enemies.Remove(enemy);
        Destroy(enemy);
        Instance.PlayerHealth += 1;
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
