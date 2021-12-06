using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour, Interactable
{
    public int InteractionPriority => _interactionPriority;
    private int _interactionPriority;

    public string linkedScene;

    public void Interact()
    {
        SceneManager.LoadScene(linkedScene);
    }

}
