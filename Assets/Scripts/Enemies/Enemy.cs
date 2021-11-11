using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all enemies. Contains global functionality like health and taking damage.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    public float Health { get { return _health; } }
    [SerializeField] protected float _health;

    public float MaxHealth { get { return _maxHealth; } }
    [SerializeField] protected float _maxHealth;

    public bool IsInvulnerable { get { return _isInvulnerable; } }
    [SerializeField] protected bool _isInvulnerable;

    public Vector2 Speed { get { return _speed; } }
    [SerializeField] protected Vector2 _speed;

    protected delegate IEnumerator ActionDelegate();
    /// <summary>
    /// Used to prevent starting an action when we are already in the middle of one, such as an attack.
    /// Only used when necessary. Movement does not require this as it occurs every frame.
    /// </summary>
    protected ActionDelegate actionInProgress;

    /// <summary>
    /// Deals the specified amount of damage to the enemy and kills them if health is &lt;=0 after the damage is applied.
    /// </summary>
    /// <param name="amount"></param>
    public virtual void Damage(float amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            // Die
        }
    }

    /// <summary>
    /// Returns whether the player is within the given range of the enemy.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual bool PlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, Player.Instance.transform.position) <= range;
    }
}
