using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

/// <summary>
/// Singleton MonoBehaviour representing the player.
/// </summary>
public class Player : MonoBehaviour
{
    #region Singleton
    private static Player _instance;

    public static Player Instance { get => _instance; }

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

    public float Health { get => _health; }
    [SerializeField] private float _health;

    public float MaxHealth { get => _maxHealth; }
    [SerializeField] private float _maxHealth;

    public bool IsInvulnerable { get => _isInvulnerable; }
    [SerializeField] private bool _isInvulnerable;

    public Vector2 Speed { get => _speed; }
    [SerializeField] private Vector2 _speed;

    /// <summary>
    /// Allows or prevents the player's Velocity from being updated by movement commands. When set to false, immediately sets Velocity to 0.
    /// </summary>
    public bool CanMove { 
        get => _canMove;
        set { 
            _canMove = value;
            _velocity = Vector2.zero;
        }
    }
    [SerializeField] private bool _canMove;

    /// <summary>
    /// Current velocity of the player. Updated when a move input is made or released and applied to position each frame in FixedUpdate.
    /// This is really more like the player's direction — it doesn't account for their speed multiplier.
    /// </summary>
    public Vector2 Velocity { get => _velocity; }
    [SerializeField] private Vector2 _velocity;

    [SerializeField] private Rigidbody2D rb;

    public float DodgeRollSpeedMultiplier { get => _dodgeRollSpeedMultiplier; }
    [SerializeField] private float _dodgeRollSpeedMultiplier;

    public float DodgeRollDuration { get => _dodgeRollDuration; }
    [SerializeField] private float _dodgeRollDuration;

    public bool IsRolling
    {
        get { return _isRolling; }
    }
    [SerializeField] private bool _isRolling;

    /// <summary>
    /// During the Dodge Roll, the latest input is stored. At the end of the roll, the player's velocity is set to that value to create a smoother player experience.
    /// </summary>
    [SerializeField] private Vector2 storedVelocity;

    /// <summary>
    /// The Rewind point the player saved when using the first cast of Rewind.
    /// Null indicates that the player does not have a Rewind point saved.
    /// </summary>
    private RewindSavePoint rewindSavePoint;

    /// <summary>
    /// Visual marker for the player's Rewind location.
    /// </summary>
    [SerializeField] private GameObject rewindMarker;

    public Gun EquippedGun { get => _equippedGun; }

    [SerializeField] private Gun _equippedGun;

    /// <summary>
    /// Object pool for the bullets fired by the player. 
    /// Is updated to the new bullet type when a new weapon is equipped.
    /// </summary>
    private GameObject[] bulletPool;

    [SerializeField] private float timeSinceLastShot;

    [SerializeField] private int ammoRemaining;

    public bool IsReloading { get => _isReloading; }

    [SerializeField] private bool _isReloading;

    public float ReloadProgress { get => _reloadProgress; }
    private float _reloadProgress;

    /// <summary>
    /// Whether the fire button is currently held. Used to support weapons firing continuously.
    /// </summary>
    private bool isFiring;

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
            Assert.IsNotNull(rb);
        }
        Assert.IsNotNull("rewindMarker was null");
        Assert.IsTrue(_canMove, "canMove set to false at start");
        Assert.IsNotNull(_equippedGun, "equippedGun was null at start");
        #endregion

        _health = _maxHealth;
        UpdateObjectPool();
        ammoRemaining = _equippedGun.maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        rb.position += _velocity * _speed * Time.deltaTime;
        timeSinceLastShot += Time.deltaTime;

        // We shoot when the button is first pressed and each frame if our gun fires continuously.
        if (_equippedGun.fireContinuously && isFiring)
        {
            Shoot();
        }
    }

    /// <summary>
    /// Re-initializes the player's object pool with GameObjects for the current weapon's projectiles.
    /// Should be called on startup and any time the player switches weapons.
    /// </summary>
    private void UpdateObjectPool()
    {
        // Destroy old projectiles
        if (bulletPool != null)
        {
            foreach (GameObject obj in bulletPool)
            {
                Destroy(obj);
            }
        }

        // Create new ones
        GameObject[] newBulletPool = new GameObject[_equippedGun.projectilePoolSize];
        for (int i = 0; i < _equippedGun.projectilePoolSize; i++)
        {
            newBulletPool[i] = Instantiate(_equippedGun.projectile);
            newBulletPool[i].SetActive(false);
        }
        bulletPool = newBulletPool;
    }

    private void InitializeProjectile(Vector2 position, Vector2 direction)
    {
        foreach (GameObject obj in bulletPool)
        {
            if (!obj.activeSelf)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90);
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.GetComponent<Projectile>().direction = direction;
                obj.SetActive(true);
                break;
            }
        }
    }

    /// <summary>
    /// Is called by the InputSystem on both press and release.
    /// </summary>
    /// <param name="value"></param>
    private void OnFire(InputValue value)
    {
        isFiring = value.isPressed;
        Shoot();
    }

    /// <summary>
    /// Checks if the player is eligible to shoot and, if so, shoots.
    /// </summary>
    private void Shoot()
    {
        if (isFiring && !_isReloading && timeSinceLastShot >= (float) 1 / _equippedGun.shotsPerSecond)
        {
            if (ammoRemaining > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector2 direction = new Vector2(transform.position.x, transform.position.y) - mousePos;
                direction.Normalize();

                InitializeProjectile(transform.position, direction);
                timeSinceLastShot = 0;
                ammoRemaining--;
            }
            else
            {
                OnReload();
            }
        }
    }

    private void OnReload()
    {
        if (!IsReloading && ammoRemaining < _equippedGun.maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        ReloadSliderManager.Instance.SetReloadBarActive(true);
        _isReloading = true;
        // We need to track the reload progress for the reload bar display
        while (_reloadProgress < _equippedGun.reloadTime)
        {
            _reloadProgress += Time.deltaTime;
            yield return null;
        }
        ammoRemaining = _equippedGun.maxAmmo;
        _isReloading = false;
        _reloadProgress = 0;
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