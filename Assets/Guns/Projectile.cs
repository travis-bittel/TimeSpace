using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector2 direction;
    public float damage;
    public float speed;

    [Header("Targetting")]
    public bool hitPlayer;
    public bool hitEnemies;
    public bool hitObstacles;

    public bool destroyWhenDeactivated;

    [Header("Final Projectile Properties")]
    [Tooltip("Whether this projectile is affected by the player's ammo count (final shot)")]
    public bool useFinalShot;
    public float finalShotDamage;
    public float finalShotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
        Assert.AreNotEqual(speed, 0, "Projectile speed was set to 0");
        Assert.AreNotEqual(finalShotSpeed, 0, "Projectile final shot speed was set to 0");
    }

    public void Initialize(Vector2 position, Quaternion rotation, Vector2 direction)
    {
        transform.SetPositionAndRotation(position, rotation);
        this.direction = direction;
        gameObject.SetActive(true);

        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);

        if (useFinalShot && Player.Instance.AmmoRemaining == 0)
        {
            rb.velocity = direction * (-finalShotSpeed);
        }
        else
        {
            rb.velocity = direction * (-speed);
        }

        // Temporary for visual effect, replace with different sprite later
        if (useFinalShot && Player.Instance.AmmoRemaining == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hitPlayer && !Player.Instance.IsInvulnerable)
        {
            Player.Instance.Damage(damage);
            gameObject.SetActive(false);
        }
        if (other.CompareTag("Enemy") && hitEnemies)
        {
            if (useFinalShot && Player.Instance.AmmoRemaining == 0)
            {
                other.GetComponent<Entity>().Damage(finalShotDamage);
            }
            else
            {
                other.GetComponent<Entity>().Damage(damage);
            }
            gameObject.SetActive(false);
        }
        if (other.CompareTag("Obstacle") && hitObstacles)
        {
            if (useFinalShot && Player.Instance.AmmoRemaining == 0)
            {
                other.GetComponent<Entity>().Damage(finalShotDamage);
            }
            else
            {
                other.GetComponent<Entity>().Damage(damage);
            }
            gameObject.SetActive(false);
        }
        // Disable bullet when colliding with wall
        if (other.CompareTag("Room"))
        {
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (destroyWhenDeactivated)
        {
            Destroy(gameObject);
        }
    }
}
