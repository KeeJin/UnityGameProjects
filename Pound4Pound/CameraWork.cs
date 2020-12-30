using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraWork : MonoBehaviourPun
{
    #region Private Fields

    // [Tooltip("The distance in the local x-z plane to the target")]
    // [SerializeField]
    // private float distance = 5.0f;


    // [Tooltip("The height we want the camera to be above the target")]
    // [SerializeField]
    // private float height = 4.5f;


    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;


    [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
    [SerializeField]
    private bool followOnStart = false;


    [Tooltip("The Smoothing for the camera to follow the target")]
    [SerializeField]
    private float smoothSpeed = 5f;

    private Vector3 targetOffset;
    private float playerOffset;

    // cached transform of the target
    Transform cameraTransform;


    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;


    // Cache for camera offset
    // Vector3 cameraOffset = Vector3.zero;

    private float playerPosX;
    private float playerPosY;
    private float playerPosZ;


    #endregion


    #region MonoBehaviour Callbacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase
    /// </summary>
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<CameraWork>());
        }
        // Start following the target if wanted.
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    void Update()
    {
        playerPosX = this.transform.position.x;
        playerPosY = this.transform.position.y;
        playerPosZ = this.transform.position.z;
        playerOffset = Vector3.Distance(this.transform.position, Vector3.zero);
        // if (this.transform.position.y > 5f)
        // {
        //     targetOffset = new Vector3(0, -3.4f, 6f);
        // }
        if (playerOffset < 7.5f)
        {
            float xpos = 0f;
            float zpos = 0f;
            if(playerPosX > 6)
            {
                xpos = 5f;
            }
            else if(playerPosX < 6)
            {
                xpos = -5f;
            }
            if(playerPosZ > 6)
            {
                zpos = 5f;
            }
            else if(playerPosZ < 6)
            {
                zpos = -5f;
            }
            targetOffset = new Vector3(xpos, playerPosY + 3.8f, zpos);
        }
        // else
        // {
        //     targetOffset = new Vector3(this.transform.position.x, 9.4f-Mathf.Min(playerOffset, 6f), this.transform.position.z);
        // }
    }


    void LateUpdate()
    {
        // The transform target may not destroy on level load,
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }
        
        // only follow is explicitly declared
        if (isFollowing) {
            Follow ();
        }
    }


    #endregion


    #region Public Methods


    /// <summary>
    /// Raises the start following event.
    /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    /// </summary>
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        // Cut();
    }


    #endregion


    #region Private Methods


    /// <summary>
    /// Follow the target smoothly
    /// </summary>
    void Follow()
    {
        // cameraOffset.z = -distance;
        // cameraOffset.x = height;


        cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + targetOffset, smoothSpeed*Time.deltaTime*5);
        // cameraTransform.position = this.transform.position + targetOffset;
        cameraTransform.LookAt(this.transform.position + centerOffset);
    }


    void Cut()
    {
        // cameraOffset.z = -distance;
        // cameraOffset.y = height;


        // cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

        cameraTransform.position = this.transform.position + targetOffset;
        // cameraTransform.LookAt(this.transform.position + centerOffset);
    }
    #endregion
}