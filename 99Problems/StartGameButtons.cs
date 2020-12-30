using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButtons : MonoBehaviour
{
    public Button startgame;
    public Button settings;

    void Awake()
    {
        // Start playing pre-game background music
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicController>().PlayMusic(); 
    }
    
    void Start()
    {
        startgame.onClick.AddListener(StartGame);
        settings.onClick.AddListener(Settings);
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
    void Settings()
    {
        SceneManager.LoadScene("Settings");
    }
}
