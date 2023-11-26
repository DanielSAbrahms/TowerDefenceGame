using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPrefab : MonoBehaviour
{
    public delegate void HealthChanged(float newHealth);

    public List<Transform> waypoints = new();
    public float speed = 5f;

    public float maxHealth = 100f;
    public float health;

    public int reward;
    public int penalty;
    public float hitAnimationTimerDuration = 0.1f;

    private FollowGamePath followGamePath;

    private HealthBarSlider healthBarSlider;

    private float hitAnimationTimer;

    private new Renderer renderer;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            OnHealthChanged?.Invoke(health);
        }
    }

    private void Start()
    {
        followGamePath = gameObject.GetComponent<FollowGamePath>();
        followGamePath.Initialize(speed);

        healthBarSlider = gameObject.AddComponent<HealthBarSlider>();
        healthBarSlider.Initialize(gameObject.transform, health, 0, maxHealth);

        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.white;
    }

    private void Update()
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

    public event HealthChanged OnHealthChanged;

    public void Initialize(List<Transform> waypointsParam)
    {
        waypoints = waypointsParam;
        Health = health = maxHealth;
    }

    public void BeTargeted(Tower tower)
    {
    }

    public void TakeDamage(float damageTaken)
    {
        Health -= damageTaken;

        hitAnimationTimer = hitAnimationTimerDuration;

        if (Health <= 0) Die();
    }

    public void Die()
    {
        GameManager.Instance.DestroyEnemy(gameObject);
    }

    public float GetDistanceTraveled()
    {
        if (followGamePath.IsUnityNull()) return -1f;
        return followGamePath.CalculateTraveledDistance();
    }
}