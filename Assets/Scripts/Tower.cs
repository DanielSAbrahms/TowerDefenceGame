using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float damagePerAttack;
    public float rateOfAttack;
    public float attackRange;
    private const float targetPollingRate = 0.1f;
    private LineRenderer lineRenderer;

    private GameObject targetObject; // What the tower is attacking
    private float timeSinceLastAttack;
    private float timeSinceLastTargetUpdate;
    
    // Start is called before the first frame update
    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Optional: Configure the appearance of the line
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // Set the number of vertex points the line renderer will use
        lineRenderer.positionCount = 2;

        ResetTarget();

        timeSinceLastAttack = 0;
        timeSinceLastTargetUpdate = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastTargetUpdate += Time.deltaTime;
        
        if (timeSinceLastTargetUpdate > targetPollingRate)
        {
            var farthestEnemyInPathAndRange = FindFarthestEnemyInPathAndRange();
            
            if (!farthestEnemyInPathAndRange.IsUnityNull())
            {
                targetObject = farthestEnemyInPathAndRange;
            }
            
            timeSinceLastTargetUpdate = 0;
        }

        if (!targetObject.IsUnityNull())
        {
            var enemyPrefab = targetObject.GetComponent<EnemyPrefab>();
            TargetEnemy(enemyPrefab);
        }
        else
        {
            ResetTarget();
        }
    }

    private GameObject FindClosestEnemy()
    {
        var enemies = GameManager.Instance.GetAllEnemies();
        if (enemies.Count <= 0) return null;

        var closestEnemy = enemies[0];
        if (!closestEnemy.IsDestroyed())
        {
            var closestEnemyDistance = Vector3.Distance(gameObject.transform.position, closestEnemy.transform.position);

            foreach (var enemy in enemies)
                if (!enemy.IsDestroyed())
                {
                    var distanceToTower = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
                    if (distanceToTower < closestEnemyDistance)
                    {
                        closestEnemy = enemy;
                        closestEnemyDistance = distanceToTower;
                    }
                }

            return closestEnemy;
        }

        return null;
    }

    private GameObject FindFarthestEnemyInPathAndRange()
    {
        List<GameObject> enemies = GameManager.Instance.GetAllEnemies();

        GameObject farthestEnemy = null;
        float farthestEnemyProgress = 0;
        
        foreach (GameObject enemy in enemies)
        {
            float enemyProgress = enemy.GetComponent<EnemyPrefab>().GetDistanceTraveled();
            
            var enemyDistance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
            bool isEnemyInRange = enemyDistance < attackRange;

            if (enemyProgress > farthestEnemyProgress && isEnemyInRange)
            {
                farthestEnemy = enemy;
                farthestEnemyProgress = enemyProgress;
            }
        }

        return farthestEnemy;
    }

    private void TargetEnemy(EnemyPrefab enemyPrefab)
    {
        if (!enemyPrefab.IsUnityNull())
        {
            enemyPrefab.BeTargeted(this);

            if (timeSinceLastAttack >= rateOfAttack)
            {
                AttackEnemy(enemyPrefab);
                timeSinceLastAttack = 0;
            }

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, gameObject.transform.position);
            lineRenderer.SetPosition(1, enemyPrefab.transform.position);
        }
        else
        {
            Debug.LogError("enemyPrefab not found on the enemy GameObject.");
        }
    }

    private void AttackEnemy(EnemyPrefab enemy)
    {
        enemy.TakeDamage(damagePerAttack);
    }

    private void ResetTarget()
    {
        lineRenderer.enabled = false;
    }
}