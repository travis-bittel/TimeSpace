using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    /// <summary>
    /// The fog of war in the next room to reveal.
    /// </summary>
    [SerializeField] private GameObject fogOfWar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Call to open the door and remove the attached fog of war, if any.
    /// </summary>
    public void Interact()
    {
        gameObject.SetActive(false);
        if (fogOfWar != null)
        {
            fogOfWar.SetActive(false);
        }
    }
}
