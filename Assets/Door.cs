using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    /// <summary>
    /// The fog of war in the next room to reveal.
    /// </summary>
    [SerializeField] private GameObject fogOfWar;

    /// <summary>
    /// Enemies to set active when the door is opened.
    /// </summary>
    [SerializeField] private List<Enemy> enemies;

    /// <summary>
    /// Call to open the door and remove the attached fog of war, if any.
    /// </summary>
    public void Interact()
    {
        if (GameManager.Instance.NumberOfActiveEnemies() == 0)
        {
            gameObject.SetActive(false);
            if (fogOfWar != null)
            {
                fogOfWar.SetActive(false);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }
}
