using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private int volControl;
    public Sprite volOn;
    public Sprite volOff;
    public Button volBtn;
    private AudioSource lobbyMusic;
    // Start is called before the first frame update
    void Start()
    {
        lobbyMusic = GetComponent<AudioSource>();
        volControl = PlayerPrefs.GetInt("volume", 1);

        if (volControl == 1)
        {
            volBtn.image.sprite = volOn;
        }
        else if (volControl == 0)
        {
            volBtn.image.sprite = volOff;
            lobbyMusic.mute = true;
        }
    }

    public void UpdateVolPrefs()
    {
        lobbyMusic.mute = !lobbyMusic.mute;
        if (volControl == 1)
        {
            volBtn.image.sprite = volOff;
            volControl = 0;
            PlayerPrefs.SetInt("volume", volControl);
            PlayerPrefs.Save();
        }
        else if (volControl == 0)
        {
            volBtn.image.sprite = volOn;
            volControl = 1;
            PlayerPrefs.SetInt("volume", volControl);
            PlayerPrefs.Save();
        }
    }
}
