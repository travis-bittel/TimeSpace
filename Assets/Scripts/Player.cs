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

    public float DodgeRollSpeedMultiplier { get { return _dodgeRollSpeedMultiplier; } }
    [SerializeField]
    private float _dodgeRollSpeedMultiplier;

    public float DodgeRollDuration { get { return _dodgeRollDuration; } }
    [SerializeField]
    private float _dodgeRollDuration;

    public bool IsRolling
    {
        get { return _isRolling; }
    }
    [SerializeField]
    private bool _isRolling;

    /// <summary>
    /// During the Dodge Roll, the latest input is stored. At the end of the roll, the player's velocity is set to that value to create a smoother player experience.
    /// </summary>
    [SerializeField]
    private Vector2 storedVelocity;

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
        // If we are rolling, store the input for use after the roll ends.
        if (_isRolling)
        {
            storedVelocity = value.Get<Vector2>();
        } else if (_canMove)
        {
            _velocity = value.Get<Vector2>();
        }
    }

    private void OnDodgeRoll(InputValue value)
    {
        StartCoroutine(DodgeRoll());
    }

    private IEnumerator DodgeRoll()
    {
        storedVelocity = _velocity;
        _isInvulnerable = true;
        _isRolling = true;
        // Default direction if no movement is held. Should be the player's facing direction.
        // For now just make it whatever.
        if (_velocity == Vector2.zero)
        {
            _velocity = Vector2.up;
        }

        // Double movement speed during roll
        _velocity *= _dodgeRollSpeedMultiplier;
        float timePassed = 0;
        while (timePassed < _dodgeRollDuration)
        {
            rb.position += _velocity * _speed * Time.deltaTime;
            timePassed += Time.deltaTime;
            yield return null;
        }

        // See storedVelocity for an explanation
        if (storedVelocity != Vector2.zero)
        {
            _velocity = storedVelocity;
        } else
        {
            _velocity = Vector2.zero;
        }
        storedVelocity = Vector2.zero;
        _isInvulnerable = false;
        _isRolling = false;
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
