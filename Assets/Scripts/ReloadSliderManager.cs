using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton class that handles the reload bar displayed above the player.
/// </summary>
[RequireComponent(typeof(Slider))]
public class ReloadSliderManager : MonoBehaviour
{
    #region Singleton
    private static ReloadSliderManager _instance;

    public static ReloadSliderManager Instance { get { return _instance; } }

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
        slider = GetComponent<Slider>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// While the bar is active, it will automatically update itself to display the current reload progress of the player.
    /// The bar will automatically hide itself when it detects that the reload is complete.
    /// </summary>
    public void SetReloadBarActive(bool active)
    {
        gameObject.SetActive(active);
    }

    // Update is only called on active objects
    private void Update()
    {
        slider.value = slider.maxValue * Player.Instance.ReloadProgress / Player.Instance.EquippedGun.reloadTime;
        if (!Player.Instance.IsReloading)
        {
            SetReloadBarActive(false);
        }
    }
}
