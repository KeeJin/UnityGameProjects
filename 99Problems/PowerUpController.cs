using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 10);   // Ensures that "Solution" is destroyed after 10.0s
    }
}
