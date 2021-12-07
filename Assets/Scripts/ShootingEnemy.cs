using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy
{
    [SerializeField] private Rigidbody2D rb;

    /// <summary>
    /// Holds the current state of the enemy (Moving, Attacking, etc.)
    /// As enemies have different state lists and actions corresponding to those states, 
    /// this should be kept private and not accessed as it has little meaning to other classes.
    /// </summary>
    [SerializeField] private State state;

    [SerializeField] private GameObject projectile;

    [SerializeField] private float shotCooldown;

    private AudioSource shoot;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        #region Value Checking
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("No Rigidbody2D found on DemoEnemy");
            }
        }
        #endregion
        shoot = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Dialogue.Instance.DialogueActive)
        {
            return;
        }
        // Call current state function if it is not already in progress. For example, if we are in the Attacking state, we want to start an attack
        // if we are not already in the process of one.
        if (actionInProgress != ConvertStateToAction(state))
        {
            StartCoroutine(ConvertStateToAction(state).Method.Name);
        }
    }

    /// <summary>
    /// Action method that fires a 3-round burst of projectiles.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot()
    {
        actionInProgress = Shoot;
        InitializeProjectile();
        yield return new WaitForSeconds(0.15f);
        InitializeProjectile();
        yield return new WaitForSeconds(0.15f);
        InitializeProjectile();
        yield return new WaitForSeconds(shotCooldown);

        actionInProgress = null;
    }

    private void InitializeProjectile()
    {
        GameObject obj = Instantiate(projectile);

        shoot.Play();

        Vector2 direction = new Vector2(transform.position.x, transform.position.y) - new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y);
        direction.Normalize();
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90);
        obj.GetComponent<Projectile>().Initialize(transform.position, rotation, direction);
    }

    /// <summary>
    /// Returns a delegate to the proper action method based on the state given.
    /// </summary>
    /// <param name="state"></param>
    /// <returns>Delegate to the action corresponding to the state</returns>
    private ActionDelegate ConvertStateToAction(State state)
    {
        switch (state)
        {
            case State.Shooting:
                return Shoot;
            default:
                return null;
        }
    }


    /// <summary>
    /// List of all states for this enemy. This list can vary significantly between enemies
    /// and is not meant to be referenced outside of this class.
    /// </summary>
    private enum State
    {
        Shooting,
        Idle
    }
}
