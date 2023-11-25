using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyPrefab : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public float speed = 5f;

    public float maxHealth = 100f;
    public float health;

    public int reward;
    public int penalty;

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

    private float hitAnimationTimer = 0f;
    public float hitAnimationTimerDuration = 0.1f;

    public void Initialize(List<Transform> waypointsParam)
    {
        waypoints = waypointsParam;
        Health = health = maxHealth;
    }

    void Start() {
        followGamePath = gameObject.GetComponent<FollowGamePath>();
        followGamePath.Initialize(speed);

        healthBarSlider = gameObject.AddComponent<HealthBarSlider>();
        healthBarSlider.Initialize(gameObject.transform, health, 0, maxHealth);

        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.white;
    }

    void Update()
    {
        if (hitAnimationTimer > 0)
        {
            renderer.material.color = new Color(1f, 0.8f, 0.8f, 1f);

            hitAnimationTimer -= Time.deltaTime;
        }
        else
        {
            hitAnimationTimer = 0f;
            renderer.material.color = Color.white;
        }
    }

    public void BeTargeted(Tower tower) {
        
    }

    public void TakeDamage(float damageTaken) {
        Health -= damageTaken;

        hitAnimationTimer = hitAnimationTimerDuration;

        if (Health <= 0) Die();
    }

    public void Die()
    {
        GameManager.Instance.KillEnemy(gameObject);
    }
    
}
