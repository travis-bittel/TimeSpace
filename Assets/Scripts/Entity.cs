using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for anything which moves, can be damaged, etc.
/// Main difference between this and enemy is that enemies are registered to GameManager's activeEnemies list while entities are not.
/// </summary>
public class Entity : MonoBehaviour
{
    public float Health { get { return _health; } }
    [SerializeField] protected float _health;

    public float MaxHealth { get { return _maxHealth; } }
    [SerializeField] protected float _maxHealth;

    public bool IsInvulnerable { get { return _isInvulnerable; } }
    [SerializeField] protected bool _isInvulnerable;

    public Vector2 Speed { get { return _speed; } }
    [SerializeField] protected Vector2 _speed;

    protected HealthbarHandler healthbar;

    public virtual void Damage(float amount)
    {
        _health -= amount;
        if (healthbar != null)
        {
            healthbar.UpdateHealthbar(_health);
        }
        if (_health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void Start()
    {
        _health = _maxHealth;
        // This looks weird, but we want to support not having a healthbar on the entity
        if (healthbar == null)
        {
            healthbar = GetComponentInChildren<HealthbarHandler>();
        }
        if (healthbar != null)
        {
            healthbar.InitializeHealthbar(_maxHealth, _health);
        }

    }

    /// <summary>
    /// Returns whether the player is within the given range of the entity.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual bool PlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, Player.Instance.transform.position) <= range;
    }
}
