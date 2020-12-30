using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Instantiate necessary variables
    public Transform[] teleport;
    public GameObject problem;
    public GameObject solution;
    private int enemycount;
    private int enemymax;
    private int solcount;
    private int solmax;
    private Vector3 playerpos;
    private Vector3 spawnpoint;
    private float xpos;
    private float ypos;
    private float playerspawndist;
    private float distancethreshold;
    private int gamestage;
    private float timeRemainingE = 0f;
    private float timeRemainingP = 8f;
    private float gamestarttime = 4.5f;
    public Text countdowntimer;
    public AudioSource countdownhorn;
    public AudioSource gamestartsound;

    void Start()
    {
        gamestage = 1;
        enemycount = 0;
        enemymax = 5;
        solcount = 0;
        solmax = 3;
        distancethreshold = 10f;

        // Start Coroutine for playing countdown timer sound effect (Not doing so will result in time synchronisation issues)
        StartCoroutine(PlayHornEvery(1.5f, 3));
    }

    // Update is called once per frame
    void Update()
    {
        if (gamestarttime > 0)
        {
            CountdownTimerText(); // Displays text according to the time left on countdown timer
        }
        else
        {
            Destroy(countdowntimer); // Destroy countdowntimer object

            // Every 5 seconds, new enemies will spawn.
            if (timeRemainingE > 0)
            {
                timeRemainingE -= Time.deltaTime;
            }
            else
            {
                if (enemymax<50)
                {
                    enemymax = 3 * gamestage;
                }
                enemycount = 0;

                // Spawn enemy
                while (enemycount<enemymax)
                {
                    playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;

                    // Create vector with a random spawn position
                    xpos = Random.Range(-20.0f, 20.0f);
                    ypos = Random.Range(-9.0f, 9.0f);
                    spawnpoint = new Vector3(xpos, 0f, ypos);

                    // Calculates and checks if the enemy is spawning too close to the player. 
                    // If far enough, enemy object will be instantiated.
                    playerspawndist = Vector3.Distance(spawnpoint, playerpos);
                    if (playerspawndist>distancethreshold/2)
                    {
                        Instantiate(problem, spawnpoint, Quaternion.identity);
                        enemycount++;
                    }
                }
                timeRemainingE = 5f; // Resets countdown to 5.0s
                gamestage += 1;
            }

            // Every 5 seconds, 3 new power-ups will spawn.
            if (timeRemainingP > 0)
            {
                timeRemainingP -= Time.deltaTime;
            }
            else
            {
                // Spawn power-ups
                while (solcount<solmax)
                {
                    playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;

                    // Create vector with a random spawn position
                    xpos = Random.Range(-20.0f, 20.0f);
                    ypos = Random.Range(-9.0f, 9.0f);
                    spawnpoint = new Vector3(xpos, 0f, ypos);

                    // Calculates and checks if the power-up is spawning too close to the player. 
                    // If far enough, power-up object will be instantiated.
                    playerspawndist = Vector3.Distance(spawnpoint, playerpos);
                    if (playerspawndist>distancethreshold/2)
                    {
                        Instantiate(solution, spawnpoint, Quaternion.identity);
                        solcount++;
                    }
                }
                solcount = 0;
                timeRemainingP = 8f; // Resets countdown to 8.0s
            }
        }
    }

    // Definition for Coroutine for countdown timer sound effect
    IEnumerator PlayHornEvery(float t, int times)
    {
        for(int i=0;i<times;i++)
            {
                countdownhorn.Play();
                yield return new WaitForSeconds(t);
            }
        gamestartsound.Play();
    }

    // Function to display text for countdown timer
    private void CountdownTimerText()
    {
        if(gamestarttime>3f)
            {
                // Text is already displayed by default
                gamestarttime -= Time.deltaTime;
            }
            else if(gamestarttime<=3f && gamestarttime>1.5f)
            {
                // 2 seconds to game start
                countdowntimer.text = "GAME STARTS IN 2...";
                gamestarttime -= Time.deltaTime;
            }
            else if(gamestarttime<=1.5f && gamestarttime>0f)
            {
                // 1 second to game start
                countdowntimer.text = "GAME STARTS IN 1...";
                gamestarttime -= Time.deltaTime;
            }
    }
}
