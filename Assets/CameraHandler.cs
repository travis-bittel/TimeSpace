using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        // horzExtent and vertExtent are half the horz and vert size of the camera viewport, respectively.
        // Taken from: https://answers.unity.com/questions/501893/calculating-2d-camera-bounds.html
        float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        float vertExtent = Camera.main.orthographicSize;

        // We get the bounds of the current room. The camera can move to where its viewport lines up with those bounds.
        CameraBounds bounds = GameManager.Instance.CurrentRoom.CameraBounds;

        Vector3 newPosition = new Vector3
        (
            Mathf.Clamp(Player.Instance.transform.position.x, bounds.minX + horzExtent, bounds.maxX - horzExtent),
            Mathf.Clamp(Player.Instance.transform.position.y, bounds.minY + vertExtent, bounds.maxY - vertExtent),
            -10
        );

        // If two bounds are equal (such as when a room is too small to show verticaly or horizontally), we
        // just fix the camera to the position of those bounds.
        if (bounds.minX == bounds.maxX)
        {
            newPosition.x = bounds.minX;
        }
        if (bounds.minY == bounds.maxY)
        {
            newPosition.y = bounds.minY;
        }

        transform.position = newPosition;
    }
}
