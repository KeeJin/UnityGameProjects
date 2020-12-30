using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestroyer : MonoBehaviour
{
    void Update()
    {
        if(transform.position.y<-10f)
        {
            Destroy(this.gameObject);
        }
    }
}
