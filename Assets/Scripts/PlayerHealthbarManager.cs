using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbarManager : MonoBehaviour
{
    #region Singleton
    private static PlayerHealthbarManager _instance;

    public static PlayerHealthbarManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    private Slider slider;

    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    public void UpdateHealthbar()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        slider.maxValue = Player.Instance.MaxHealth;
        slider.value = Player.Instance.Health;
    }
}
