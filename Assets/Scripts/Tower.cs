using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{

    private GameObject targetObject; // What the tower is attacking
    public float damagePerAttack;
    public float rateOfAttack;
    public float attackRange;
    public float attackAnimationDuration;
    private float timeSinceLastAttack;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        GameObject closestEnemy = FindClosestEnemy();
        
        timeSinceLastAttack += Time.deltaTime;

        if (closestEnemy != null) {
            float enemyDistance = Vector3.Distance(gameObject.transform.position, closestEnemy.transform.position);
            
            // Enemy Within Range
            targetObject = (enemyDistance <= attackRange) ? closestEnemy : null;
        }

        if (targetObject != null) {
            EnemyPrefab enemyPrefab = targetObject.GetComponent<EnemyPrefab>();
            TargetEnemy(enemyPrefab);
        } else {
            ResetTarget();
        }
    }

    GameObject FindClosestEnemy() {
        List<GameObject> enemies = GameManager.Instance.GetAllEnemies();
        if (enemies.Count <= 0) return null;

        GameObject closestEnemy = enemies[0];
        if (!closestEnemy.IsDestroyed())
        {
            float closestEnemyDistance = Vector3.Distance(gameObject.transform.position, closestEnemy.transform.position);

            foreach(GameObject enemy in enemies) {
                if (!enemy.IsDestroyed())
                {
                    float distanceToTower = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
                    if (distanceToTower < closestEnemyDistance) {
                        closestEnemy = enemy;
                        closestEnemyDistance = distanceToTower;
                    }
                }
            }
            return closestEnemy;
        }

        return null;
    }

    void TargetEnemy(EnemyPrefab enemyPrefab) {
            if (enemyPrefab != null) {

                enemyPrefab.BeTargeted(this);

                if (timeSinceLastAttack >= rateOfAttack)
                {
                    AttackEnemy(enemyPrefab);
                    timeSinceLastAttack = 0;
                }

                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, gameObject.transform.position);
                lineRenderer.SetPosition(1, enemyPrefab.transform.position);
            } else {
                Debug.LogError("enemyPrefab not found on the enemy GameObject.");
            }
    }

    void AttackEnemy(EnemyPrefab enemy) {
        enemy.TakeDamage(damagePerAttack);
    }

    void ResetTarget() {
        lineRenderer.enabled = false;
    }
 }
