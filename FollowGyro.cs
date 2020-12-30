using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FollowGyro : MonoBehaviour
{
    public float mSpeed;
    public float rSpeed;
    float rollInput = 0.0f;
    float pitchInput = 0.0f;
    private Vector3 directionInput;
    private Vector3 accmagnitude;
    private Vector3 SpeedVec;
    float currentDegrees = 0f;
    private int score = 0;
    public Text scoretext;
    private Vector3 solpos;
    private float killradius;
    public GameObject explosion;
    private float timer;
    private float countdowntimer;
    public AudioSource splat_powerup1;
    private bool countdowncheck;
    private int kill_count;

    void Start()
    {
        // Stop pre-game background music
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicController>().StopMusic();

        GyroManager.Instance.EnableGyro();
        mSpeed = 20f;
        rSpeed = 20f;
        scoretext.text = "SCORE: 0";
        killradius = 5f;
        timer = 0f;
        countdowntimer = 4.5f;
        countdowncheck = true;
        kill_count = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Counts down from 4.5s
        if(countdowntimer<=0)
        {
            countdowncheck=false;
        }
        else
        {
            countdowntimer -= Time.deltaTime;
        }

        // Only start counting score when 4.5s has elapsed
        if (timer>1f && countdowncheck==false)
        {
            score += 1;
            scoretext.text = "SCORE: " + score.ToString();
            timer = 0f;
        }
        
        rollInput = GyroManager.Instance.GetGyroRoll();
        pitchInput = GyroManager.Instance.GetGyroPitch();

        if(rollInput>90f)
        {
            rollInput = 90f;
        }
        else if(rollInput<-90f)
        {
            rollInput = -90f;
        }

        if(pitchInput>90f)
        {
            pitchInput = 90f;
        }
        else if(pitchInput<-90f)
        {
            pitchInput = -90f;
        }
        
        float degrees = Mathf.Atan2(rollInput, pitchInput) * Mathf.Rad2Deg;
        currentDegrees = Mathf.LerpAngle(currentDegrees, degrees, rSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0,currentDegrees,0);
        directionInput = Vector3.back;
        accmagnitude = new Vector3(rollInput, pitchInput, 0);
        if (accmagnitude.sqrMagnitude > 8100)
        {
            accmagnitude.Normalize();   // normalise magnitudes to ensure player's speed is independant of direction.
        }
        if (accmagnitude.magnitude>30f) // Limits player speed
        {
            SpeedVec = directionInput * mSpeed * 30f / 50;
        }
        else
        {
            SpeedVec = directionInput * mSpeed * accmagnitude.magnitude / 50;
        }
        if (accmagnitude.sqrMagnitude > 0.1)
        {
            transform.Translate(SpeedVec * Time.deltaTime);
        }
    }

    // Handles any collisions with Player
    private void OnTriggerEnter(Collider other) 
    {

        if (other.tag == "Enemy")
        {
            Debug.Log("Collided with enemy");
            PlayerPrefs.SetInt("Player Score", score);
            PlayerPrefs.SetInt("Kill Count", kill_count);
            SceneManager.LoadScene("GameOver");
        }

        if (other.tag == "Solution")
        {
            Debug.Log("Hit a solution");
            // Play splat sound
            splat_powerup1.Play();

            solpos = other.transform.position;
            Destroy(other.gameObject);
            Instantiate(explosion, solpos, Quaternion.identity); // Play explosion animation
            Collider[] nearObjects = Physics.OverlapSphere(solpos, killradius); // Creates an array of objects near the "solution"'s position.
            foreach (Collider obj in nearObjects)
            {
                if(obj.tag == "Enemy")
                {
                    kill_count += 1;
                    Destroy(obj.gameObject);
                    score += 2;
                    scoretext.text = "SCORE: " + score.ToString();
                }
            }
        }

    }
}
