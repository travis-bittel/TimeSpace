using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HealthbarHandler : MonoBehaviour
{
    private Slider slider;

    // The health value we lerp towards each frame
    private float targetValue;

    [SerializeField] private float adjustmentSpeed;

    public void InitializeHealthbar(float maxValue, float currentValue)
    {
        slider = GetComponent<Slider>();
        gameObject.SetActive(false);
        slider.maxValue = maxValue;
        slider.value = maxValue;
        Assert.AreNotEqual(adjustmentSpeed, 0, "Healthbar adjustment speed was 0");
    }

    public void UpdateHealthbar(float currentHealth)
    {
        targetValue = currentHealth;

        // Only show healthbar when enemy is damaged
        gameObject.SetActive(targetValue != slider.maxValue);
    }

    private void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, adjustmentSpeed * Time.deltaTime);
        if (Mathf.Abs(targetValue - slider.value) < 0.01f)
        {
            slider.value = targetValue;
        }
    }
}
