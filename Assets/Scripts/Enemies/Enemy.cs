using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all enemies. Contains global functionality like health and taking damage.
/// </summary>
public abstract class Enemy : Entity
{
    protected delegate IEnumerator ActionDelegate();
    /// <summary>
    /// Used to prevent starting an action when we are already in the middle of one, such as an attack.
    /// Only used when necessary. Movement does not require this as it occurs every frame.
    /// </summary>
    protected ActionDelegate actionInProgress;

    protected override void Start()
    {
        if (gameObject.activeInHierarchy)
        {
            GameManager.Instance.RegisterEnemy(this);
        }
        _health = _maxHealth;
        // This looks weird, but we want to support not having a healthbar on the enemy
        if (healthbar == null)
        {
            healthbar = GetComponentInChildren<HealthbarHandler>();
        }
        if (healthbar != null)
        {
            healthbar.InitializeHealthbar(_maxHealth, _health);
        }

    }

    protected void OnEnable()
    {
        GameManager.Instance.RegisterEnemy(this);
    }
    protected void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterEnemy(this);
        }
    }
}
