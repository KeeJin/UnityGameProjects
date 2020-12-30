using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UpdateNameTag : MonoBehaviourPun
{
    [SerializeField]
    Text NameTag;

    public void Awake()
    {
        NameTag.text = GetComponent<PhotonView>().Owner.NickName;
    }
}
