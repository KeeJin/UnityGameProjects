using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public Button resethighscore;
    public Button backtomain;
    public Text highscoretext;
    private int highscore;
    
    void Start()
    {
        resethighscore.onClick.AddListener(ResetHighScore);
        backtomain.onClick.AddListener(BackToMain);
        highscore = PlayerPrefs.GetInt("Highscore");
        highscoretext.text = "Current Highscore: " + highscore.ToString();

    }

    void BackToMain()
    {
        SceneManager.LoadScene("StartGame");
    }
    void ResetHighScore()
    {
        PlayerPrefs.SetInt("Highscore", 0); // Resets highscore to 0 and stores it in PlayerPrefs
        highscoretext.text = "Current Highscore: 0";
    }
}
