using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector2 direction;
    public float damageAmount;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);

        rb.velocity = direction * -20;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().Damage(damageAmount);
            gameObject.SetActive(false);
        }

        // Disable bullet when colliding with wall
        if (other.CompareTag("Room"))
        {
            gameObject.SetActive(false);
        }
    }
}
