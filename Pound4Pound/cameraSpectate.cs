using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraSpectate : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("Clamping")]
    public float minY;
    public float maxY;
    // public float minX;
    // public float maxX;

    [Header("Spectator")]
    public float spectatorMoveSpeed;
    
    private Vector3 startPosition;
    private Vector3 startRotation;
    private bool isSpectator;
    private float rotX;
    private float rotY;
    private bool initialised;

    void Start()
    {
        isSpectator = false;
        startPosition = new Vector3(7f, 5f, 0);
        initialised = false;
    }

    private void LateUpdate()
    {
        if(isSpectator)
        {
            if(!initialised)
            {
                Cursor.lockState = CursorLockMode.Locked;
                initialised = true;
                transform.LookAt(Vector3.zero);
            }
            Debug.Log("Currently in Spectator mode!!");

            // Get mouse movement inputs
            rotX += Input.GetAxis("Mouse X") * sensX;
            rotY += Input.GetAxis("Mouse Y") * sensY;

            // Clamp rotation
            rotY = Mathf.Clamp(rotY, minY, maxY);
            // rotY = Mathf.Clamp(rotX, minX, maxX);

            // transform.rotation = Quaternion.Euler(-rotY, rotX, 0);
            float camDistFromCenter = Vector3.Distance(transform.position, Vector3.zero);
            if(Input.GetKey("w"))
            {
                if (camDistFromCenter >= 8f)
                {
                    transform.position += transform.forward * Time.deltaTime * spectatorMoveSpeed;
                }
            }

            if(Input.GetKey("s"))
            {
                if (camDistFromCenter <= 65f)
                {
                    transform.position += -transform.forward * Time.deltaTime * spectatorMoveSpeed;
                }
            }

            if(Input.GetKey("a"))
            {
                transform.position += -transform.right * Time.deltaTime * spectatorMoveSpeed;
            }

            if(Input.GetKey("d"))
            {
                transform.position += transform.right * Time.deltaTime * spectatorMoveSpeed;
            }

            transform.LookAt(Vector3.zero);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void ActivateSpectatorCam()
    {
        isSpectator = true;
        transform.position = startPosition;
    }
    public void DeactivateSpectatorCam()
    {
        isSpectator = false;
        transform.position = startPosition;
    }
}
