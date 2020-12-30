using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mouseController : MonoBehaviourPun
{
    private Vector3 moveInput;
    private bool stuncheck;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<mouseController>());
        }
        stuncheck = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!stuncheck)
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.cyan);

                transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "StunnerBullet")
        {
            StartCoroutine(Stunned(1f, 5));
        }
    }

    IEnumerator Stunned(float t, int times)
    {
        stuncheck = true;
        for(int i=0;i<times;i++)
        {
            yield return new WaitForSeconds(t);
        }
        stuncheck = false;
    }
}
