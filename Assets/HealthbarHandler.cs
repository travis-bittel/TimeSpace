using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarHandler : MonoBehaviour
{
    private Slider slider;

    public void InitializeHealthbar(float maxValue)
    {
        slider = GetComponent<Slider>();
        gameObject.SetActive(false);
        slider.maxValue = maxValue;
    }

    public void UpdateHealthbar(float currentHealth)
    {
        slider.value = currentHealth;

        // Only show healthbar when enemy is damaged
        gameObject.SetActive(!(slider.value == slider.maxValue));
    }
}
