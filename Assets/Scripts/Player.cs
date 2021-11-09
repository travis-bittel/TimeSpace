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
    /// This is really more like the player's direction — it doesn't account for their speed multiplier.
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
    /// The Rewind point the player saved when using the first cast of Rewind.
    /// Null indicates that the player does not have a Rewind point saved.
    /// </summary>
    private RewindSavePoint rewindSavePoint;

    /// <summary>
    /// Visual marker for the player's Rewind location.
    /// </summary>
    [SerializeField]
    private GameObject rewindMarker;

    public GameObject test;

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

    private void OnDodgeRoll()
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

    private void OnRewind()
    {
        // Set point if not set, otherwise travel to it
        if (rewindSavePoint == null)
        {
            rewindSavePoint = new RewindSavePoint(transform.position, 0);
            rewindMarker.transform.position = transform.position;
            rewindMarker.SetActive(true);
        } else
        {
            transform.position = rewindSavePoint.position;
            // <Set ammo count here>
            rewindSavePoint = null;
            rewindMarker.SetActive(false);
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
        if (rewindMarker == null)
        {
            Debug.LogWarning("Rewind Marker was null at start");
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

    private void OnFire()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = new Vector2(transform.position.x, transform.position.y) - mousePos;
        direction.Normalize();

        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90);

        test.transform.rotation = rotation;
        test.transform.position = transform.position;


        test.GetComponent<Rigidbody2D>().velocity = direction * -2000f * Time.deltaTime;
    }

    /// <summary>
    /// Stores a Vector2 position and int ammoCount. These values are what the player will be restored to when rewinding to this point.
    /// The player will store 3 of these at all times — one for each charge of Rewind they can hold.
    /// </summary>
    private class RewindSavePoint
    {
        public Vector2 position;
        public int ammoCount;

        public RewindSavePoint(Vector2 position, int ammoCount = 0)
        {
            this.position = position;
            this.ammoCount = ammoCount;
        }
    }
}