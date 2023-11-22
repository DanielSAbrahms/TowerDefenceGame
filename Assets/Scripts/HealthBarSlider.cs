using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSlider : MonoBehaviour
{
    private Slider slider;
    private GameObject sliderObject;
    public Transform target; // The enemy to which this health bar belongs
    public float CurrentHealth;
    
    public float MinHealth;
    public float MaxHealth;

    private const float VerticalOffset = 2f;

    private EnemyPrefab enemyPrefab;

    void Awake() {
        slider = gameObject.GetComponentInChildren<Slider>();
        sliderObject = slider.gameObject;

        enemyPrefab = gameObject.GetComponent<EnemyPrefab>();
        enemyPrefab.OnHealthChanged += UpdateHealthBar;

        target = gameObject.transform;
    }
    void Start()
    {
        Image background = slider.AddComponent<Image>();
        background.color = Color.Lerp(Color.red, Color.green, slider.value / slider.maxValue);
    }
    void Update()
    {
        slider.minValue = MinHealth;
        slider.maxValue = MaxHealth;
        slider.value = CurrentHealth;

        // Position
        sliderObject.transform.position =
         new Vector3(target.position.x, target.position.y + VerticalOffset, target.position.z);

        //Scale
        // transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Rotation to face the camera
        sliderObject.transform.LookAt(Camera.main.transform);
        sliderObject.transform.Rotate(0, 180, 0); // Adjust if the health bar is backwards
        
    }

    public void Initialize(Transform t, float currentHealth, float minHealth, float maxHealth) {
        target = t;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        MinHealth = minHealth;
    }

    public void UpdateHealthBar(float newHealth)
    {
        
        CurrentHealth = newHealth;
        slider.value = CurrentHealth;
    }

    void OnDestroy()
    {
         // Always unsubscribe from the event when the GameObject is destroyed
         if (enemyPrefab != null) {
             enemyPrefab.OnHealthChanged -= UpdateHealthBar;
         }
    }
}
