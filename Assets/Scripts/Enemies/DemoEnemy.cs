using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DemoEnemy : Enemy
{
    [SerializeField] private Rigidbody2D rb;

    /// <summary>
    /// Holds the current state of the enemy (Moving, Attacking, etc.)
    /// As enemies have different state lists and actions corresponding to those states, 
    /// this should be kept private and not accessed as it has little meaning to other classes.
    /// </summary>
    [SerializeField] private State state;

    /// <summary>
    /// Max distance at which the enemy will attempt to attack the player.
    /// </summary>
    public float SwingRange { get { return _swingRange; } }
    [SerializeField] private float _swingRange;

    /// <summary>
    /// Max distance at which the enemy's attack will hit the player.
    /// </summary>
    public float HitRange { get { return _hitRange; } }
    [SerializeField] private float _hitRange;

    /// <summary>
    /// Time between attack startup and damage occuring.
    /// </summary>
    public float AttackWindup { get { return _attackWindup; } }
    [SerializeField] private float _attackWindup;

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
    }

    // Update is called once per frame
    void Update()
    {
        // Call current state function if it is not already in progress. For example, if we are in the Attacking state, we want to start an attack
        // if we are not already in the process of one.
        if (actionInProgress != ConvertStateToAction(state))
        {
            StartCoroutine(ConvertStateToAction(state).Method.Name);
        }
    }

    /// <summary>
    /// Action method that does a windup, then an attack which hits the player if they are within hitRange.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        actionInProgress = Attack;
        yield return new WaitForSeconds(0.5f);
        if (PlayerInRange(_hitRange))
        {
            Debug.Log("Hit!");
        }
        else
        {
            Debug.Log("Miss!");
        }
        if (!PlayerInRange(_swingRange))
        {
            state = State.Moving;
        }
        actionInProgress = null;
    }

    /// <summary>
    /// Action method to move towards the player.
    /// This is an IEnumerator just so it works with the same delegate type as all other action methods.
    /// Yes, this means every action method should be of return type IEnumerator. Use yield return null
    /// to keep the compiler happy.
    /// </summary>
    /// <returns>Nothing lmao</returns>
    private IEnumerator Move()
    { 
        transform.position = Vector2.MoveTowards(transform.position, Player.Instance.transform.position, Time.deltaTime);
        if (PlayerInRange(_swingRange))
        {
            state = State.Attacking;
        }
        yield return null;
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
            case State.Attacking:
                return Attack;
            case State.Moving:
                return Move;
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
        Attacking,
        Moving
    }
}
