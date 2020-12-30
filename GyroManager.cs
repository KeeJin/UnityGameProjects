// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    #region Instance
    private static GyroManager instance;
    public static GyroManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GyroManager>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned GyroManager", typeof(GyroManager)).GetComponent<GyroManager>();
                }
            }
            return instance;
        }
        set
        {
            instance = value;
        }

    }
    #endregion

    private Gyroscope gyro;
    private bool gyroActive;
    private float rollDegrees;
    private float pitchDegrees;

    public void EnableGyro()
    {
        // Already activated
        if (gyroActive)
        {
            return;
        }

        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroActive = gyro.enabled;
        }
        else
        {
            Debug.Log("Gyro is not supported on this device.");
            Application.Quit();
        }
        
    }


    private void Update()
    {
        if (gyroActive)
        {
            // Get the vector representing global up (away from gravity)
            // within the device's coordinate system.
            // Vector3 localDown = (Quaternion.Inverse(Input.gyro.attitude) * Vector3.down);
            Vector3 localDown = Input.gyro.gravity * -1;
            
            // Extract our roll rotation - how much gravity points to our left or right.
            rollDegrees = Mathf.Asin(localDown.x) * Mathf.Rad2Deg;

            // Extract our pitch rotation - how much gravity points forward or back.
            pitchDegrees = Mathf.Atan2(localDown.y, localDown.z) * Mathf.Rad2Deg;
        }
    }

    public float GetGyroRoll()
    {
        return rollDegrees;
    }
    public float GetGyroPitch()
    {
        return pitchDegrees;
    }
}
