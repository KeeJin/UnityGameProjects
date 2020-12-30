using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisplayScore : MonoBehaviour
{
    public Text scoretext;
    public Text highscoretext;
    private int score;
    private int highscore = 0;
    private int kill_count;

    void Start()
    {
        score = PlayerPrefs.GetInt("Player Score");
        highscore = PlayerPrefs.GetInt("Highscore");
        kill_count = PlayerPrefs.GetInt("Kill Count");

        // Updates highscore if current achieved score is greater than previously recorded highscore
        if(score>highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("Highscore", highscore);
            scoretext.text = "You solved " + kill_count.ToString() + " problems. TOTAL SCORE: "+ score.ToString() + " (BEST SCORE)";
        }
        // If achieved score is not greater than previous highscore, do not update highscore
        else
        {
            scoretext.text = "You solved " + kill_count.ToString() + " problems. TOTAL SCORE: "+ score.ToString();
        }
        highscoretext.text = "BEST SCORE: " + highscore.ToString();
    }
}
