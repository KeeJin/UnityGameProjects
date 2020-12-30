using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureRespawn : MonoBehaviour
{
    private Vector3 respawnPos;
    private int respawnCount;
    
    void Start()
    {
        respawnCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15f)
        {
            if (respawnCount >= 2)
            {
                Destroy(this.gameObject);
            }
            else
            {
                respawnPos = new Vector3(Random.Range(-3, 3), 10f, Random.Range(-3, 3));
                transform.position = respawnPos;
                respawnCount+=1;    
            }
            transform.position = respawnPos;
            respawnCount+=1;
        }
    }
}
