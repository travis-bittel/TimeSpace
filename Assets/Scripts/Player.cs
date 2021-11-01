using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton MonoBehaviour representing the player.
/// </summary>
public class Player : MonoBehaviour
{
    #region Singleton
    private static Player _instance;

    public static Player Instance { get { return _instance; } }

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

    public float Health { get { return _health; } }
    [SerializeField]
    private float _health;

    public float MaxHealth { get { return _maxHealth; } }
    [SerializeField]
    private float _maxHealth;

    public bool IsInvulnerable { get { return _isInvulnerable; } }
    [SerializeField]
    private bool _isInvulnerable;

    public Vector2 Speed { get { return _speed; } }
    [SerializeField]
    private Vector2 _speed;

    /// <summary>
    /// Allows or prevents the player's Velocity from being updated by movement commands. When set to false, immediately sets Velocity to 0.
    /// </summary>
    public bool CanMove { 
        get { return _canMove; } 
        set { 
            _canMove = value;
            _velocity = Vector2.zero;
        }
    }
    [SerializeField]
    private bool _canMove;

    /// <summary>
    /// Current velocity of the player. Updated when a move input is made or released and applied to position each frame in FixedUpdate.
    /// </summary>
    public Vector2 Velocity { get { return _velocity; } }
    [SerializeField]
    private Vector2 _velocity;

    [SerializeField]
    private Rigidbody2D rb;

    /// <summary>
    /// Deals the specified amount of damage to the player and kills them if health is &lt;=0 after the damage is applied.
    /// </summary>
    /// <param name="amount"></param>
    public void Damage(float amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            // Die
        }
    }

    /// <summary>
    /// Called by input system when a movement control is pressed or released. Sets _velocity, which is then applied as translation every frame in Update.
    /// </summary>
    private void OnMove(InputValue value)
    {
        if (_canMove)
        {
            _velocity = value.Get<Vector2>() * _speed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Value Checking
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("No Rigidbody2D found on Player");
            }
        }
        if (_speed == Vector2.zero)
        {
            Debug.LogWarning("Player Speed set to 0");
        }
        if (!_canMove)
        {
            Debug.LogWarning("canMove set to false at start");
        }
        #endregion
        _health = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        rb.position += _velocity * _speed * Time.deltaTime;
    }
}
