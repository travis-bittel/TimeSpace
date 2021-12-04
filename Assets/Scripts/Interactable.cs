using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an entity that the player can interract with by pressing the interact button.
/// Defines an Interact method that defines the behavior for when the player interacts with the entity.
/// </summary>
public interface Interactable
{
    public void Interact();
    public int InteractionPriority { get; }
}
