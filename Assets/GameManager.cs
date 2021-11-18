using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    /// <summary>
    /// The room the player is currently in. Can be updated by calling MoveToNewRoom.
    /// </summary>
    [SerializeField] Room _currentRoom;
    public Room CurrentRoom { get { return _currentRoom; } }

    /// <summary>
    /// Array of all Guns in the game. Use GetGunByID to access elements.
    /// The ID of an element is its index in this array.
    /// </summary>
    public Gun[] availableGunsByID;

    /// <summary>
    /// A dictionary of (weaponName, Gun) pairs containing all guns in the game.
    /// Use GetGunByName to access elements.
    /// </summary>
    public Dictionary<string, Gun> availableGunsByName;

    /// <summary>
    /// Updates the currentRoom field of GameManager, moves the player to the room's
    /// player start position, and moves the camera to the room's camera start position.
    /// </summary>
    /// <param name="newRoom"></param>
    public void MoveToNewRoom(Room newRoom)
    {
        _currentRoom = newRoom;
        Player.Instance.transform.position = newRoom.PlayerStartPosition;
    }

    private void Start()
    {
        availableGunsByName = new Dictionary<string, Gun>();
        for (int i = 0; i < availableGunsByID.Length; i++)
        {
            availableGunsByName.Add(availableGunsByID[i].weaponName, availableGunsByID[i]);
        }
    }
}
