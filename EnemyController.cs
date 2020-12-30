using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    private Vector3 destpos;

    void Update()
    {
        destpos = GameObject.FindGameObjectWithTag("Player").transform.position;
        navMeshAgent.SetDestination(destpos);
    }
}
