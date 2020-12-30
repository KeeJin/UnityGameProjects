using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StunnerBullet : MonoBehaviourPun
{
    private float forceMultiplier = 20f;
    
    private float distanceFromCenter;
    private Rigidbody m_Rigidbody;
    public AudioClip stunnerHitClip;
    public AudioClip stunnerFireClip;

    void Start()
    {
        AudioSource.PlayClipAtPoint(stunnerFireClip, this.transform.position);
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.AddForce(transform.forward * forceMultiplier, ForceMode.Impulse);
    }

    void Update()
    {
        distanceFromCenter = Vector3.Distance(transform.position, new Vector3(0, transform.position.y, 0));
        if (distanceFromCenter > 50)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player" || other.tag == "Wall" || other.tag == "Respawn")
        {
            AudioSource.PlayClipAtPoint(stunnerHitClip, this.transform.position);
            if(this.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
            }
            Debug.Log("Collided into a Player.");
            
        }
    }
}
