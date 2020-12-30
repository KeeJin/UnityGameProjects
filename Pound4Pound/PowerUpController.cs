using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerUpController : MonoBehaviourPun
{
    void Start()
    {
        Destroy(this.gameObject, 6);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            if(this.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
            }
            Debug.Log("Collided into a Player.");
            
        }
    }
}
