using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all guns the player can wield. Each gun should have it's own ScriptableObject in the Guns folder.
/// The GameManager stores a reference to each gun type in the game and has a variable tracking which gun they are currently wielding.
/// </summary>
[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun", order = 1)]
public class Gun : ScriptableObject
{
    public string weaponName;
    public int maxAmmo;
    public float reloadTime;
    public int shotsPerSecond;
    /// <summary>
    /// Whether the gun will fire repeatedly while the fire button is held.
    /// </summary>
    public bool fireContinuously;
    public GameObject projectile;
    /// <summary>
    /// Number of projectiles to include in the object pool.
    /// </summary>
    public int projectilePoolSize;
}
