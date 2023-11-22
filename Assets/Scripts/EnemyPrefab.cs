using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefab : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public float speed;
    public int currentWaypointIndex = 0;

    public float MaxHealth = 100f;

    public float health = 100f;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            OnHealthChanged?.Invoke(health);
        }
    }

    private HealthBarSlider healthBarSlider;

    public delegate void HealthChanged(float newHealth);
    public event HealthChanged OnHealthChanged;

    private FollowGamePath followGamePath;

    private new Renderer renderer;

    public void Initialize(List<Transform> waypointsParam, float speedParam, int currentWaypointIndexParam, float maxHealthParam)
    {
        waypoints = waypointsParam;
        speed = speedParam;
        currentWaypointIndex = currentWaypointIndexParam;
        MaxHealth = maxHealthParam;
        Health = maxHealthParam;
    }

    void Start() {
        followGamePath = gameObject.GetComponent<FollowGamePath>();
        followGamePath.Initialize(speed);

        healthBarSlider = gameObject.AddComponent<HealthBarSlider>();
        healthBarSlider.Initialize(gameObject.transform, health, 0, MaxHealth);

        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.white;
    }

    public void BeTargeted(Tower tower) {
        
    }

    public void TakeDamage(float damageTaken) {
        Health -= damageTaken;

        if (Health <= 0) Die();
    }

    public void Die()
    {
        GameManager.Instance.KillEnemy(gameObject);
    }
    
}
