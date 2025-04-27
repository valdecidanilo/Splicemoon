using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Sprite[] healthBarConditions;
    
    [SerializeField] private Image healthBarImage;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private float lerpDuration = 0.5f;

    private Coroutine _currentLerpCoroutine;
    public event Action OnDeath; 

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
    }

    private void VerifyDeath()
    {
        if(currentHealth <= 0) OnDeath?.Invoke();
    }
    public void SetMaxHealth(int healthMax)
    {
        maxHealth = healthMax;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        var healthPercentage = (float)currentHealth / maxHealth;
        var index = (healthBarConditions.Length - 1) - Mathf.FloorToInt(healthPercentage * (healthBarConditions.Length - 1));
        index = Mathf.Clamp(index, 0, healthBarConditions.Length - 1);
        healthBarImage.sprite = healthBarConditions[index];
        VerifyDeath();
        if (_currentLerpCoroutine != null)
        {
            StopCoroutine(_currentLerpCoroutine);
        }
        _currentLerpCoroutine = StartCoroutine(LerpHealthBar());
    }

    private IEnumerator LerpHealthBar()
    {
        var elapsedTime = 0f;
        var startFillAmount = healthBarImage.fillAmount;
        var targetFillAmount = (float)currentHealth / maxHealth;
        var startHealthValue = Mathf.RoundToInt(startFillAmount * maxHealth);
        var targetHealthValue = currentHealth;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / lerpDuration);
            
            healthBarImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, t);
            
            var currentHealthValue = Mathf.RoundToInt(Mathf.Lerp(startHealthValue, targetHealthValue, t));
            if (healthText) healthText.text = $"{currentHealthValue}/ {maxHealth}";
            
            yield return null;
        }
        healthBarImage.fillAmount = targetFillAmount;
        if (healthText) healthText.text = $"{targetHealthValue}/ {maxHealth}";
        
        _currentLerpCoroutine = null;
    }
}
