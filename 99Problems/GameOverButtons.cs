using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverButtons : MonoBehaviour
{
    public Button restart;
    public Button mainmenu;
    public Button settings;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicController>().PlayMusic();
        restart.onClick.AddListener(RestartGame);
        mainmenu.onClick.AddListener(MainMenu);
    }

    // Functions to handle button presses
    void RestartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
    void MainMenu()
    {
        SceneManager.LoadScene("StartGame");
    }
}
