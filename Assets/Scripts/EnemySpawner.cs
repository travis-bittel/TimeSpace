using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, Interactable
{
    public GameObject[] enemies;

    private int _interactionPriority = 1;
    public int InteractionPriority => _interactionPriority;

    public void Interact()
    {
        foreach (GameObject obj in enemies)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.GetComponent<Enemy>().Initialize();
            }
        }
    }
}
