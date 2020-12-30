﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PreparePool : MonoBehaviour
{
    public List<GameObject> Prefabs;
    // Start is called before the first frame update
    void Start()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && this.Prefabs != null)
        {
            foreach (GameObject prefab in this.Prefabs)
            {
                try
                {
                    pool.ResourceCache.Add(prefab.name, prefab);
                }
                catch(System.Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }
}
