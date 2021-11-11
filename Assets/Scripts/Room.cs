using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Should be attached to the root object of a room. Contains data such as the camera bounds for the room and
/// the starting player and camera position.
/// </summary>
public class Room : MonoBehaviour
{
    [SerializeField] private CameraBounds _cameraBounds;
    public CameraBounds CameraBounds { get { return _cameraBounds; } }

    [SerializeField] private Vector2 _cameraStartPosition;
    public Vector2 CameraStartPosition { get { return _cameraStartPosition; } }

    [SerializeField] private Vector2 _playerStartPosition;
    public Vector2 PlayerStartPosition { get { return _playerStartPosition; } }

    private void Start()
    {
        #region Value Checking
        // Auto-fill camera bounds based on position, width, and height if they aren't set
        if (_cameraBounds.isAllZero())
        {
            _cameraBounds.minX = transform.position.x - transform.lossyScale.x / 2;
            _cameraBounds.maxX = transform.position.x + transform.lossyScale.x / 2;
            _cameraBounds.minY = transform.position.y - transform.lossyScale.y / 2;
            _cameraBounds.maxY = transform.position.y + transform.lossyScale.y / 2;

            // Check if the room is too small to fill the camera in either direction. If so, we set both
            // the bounds to the center. The camera will be centered on the room and will not move.
            if (transform.lossyScale.x < Camera.main.orthographicSize * Screen.width / Screen.height * 2)
            {
                _cameraBounds.minX = transform.position.x;
                _cameraBounds.maxX = transform.position.x;
            }
            if (transform.lossyScale.y < Camera.main.orthographicSize * 2)
            {
                _cameraBounds.minY = transform.position.y;
                _cameraBounds.maxY = transform.position.y;
            }
        }
        // Default camera and player start position to middle of the room
        if (_cameraStartPosition == Vector2.zero)
        {
            _cameraStartPosition = transform.position;
        }
        if (_playerStartPosition == Vector2.zero)
        {
            _playerStartPosition = transform.position;
        }
        #endregion

        // Temporary testing code
        GameManager.Instance.MoveToNewRoom(this);
    }
}

/// <summary>
/// Set of bounding coordinates for the camera in a given room. Each room should have one.
/// If you are unsure what to make these, they should typically be based on the size of the room.
/// Assuming the room is centered on the origin, minX = room-position-x - room-width / 2, maxX = room-position-x + room-width / 2, etc.
/// If a room does not have CameraBounds set in the inspector (ie. they are all 0s), then it will automatically
/// fill in the bounds with the default values just described. 
/// </summary>
[System.Serializable]
public struct CameraBounds
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public CameraBounds(int minX, int maxX, int minY, int maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public bool isAllZero()
    {
        return minX == 0 && maxX == 0 && minY == 0 && maxY == 0;
    }
}
