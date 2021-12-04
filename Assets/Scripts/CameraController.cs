using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    private float defaultCameraZ;

    [SerializeField]
    [Range(0,10)]
    private float cameraStiffness;

    [SerializeField]
    private Transform playerTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        defaultCameraZ = -10f;
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position - (playerTransform.position - Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())) / (cameraStiffness + 2);
        transform.position = Vector3.ProjectOnPlane(transform.position, Vector3.back);
        transform.position += Vector3.forward * defaultCameraZ;
    }
}