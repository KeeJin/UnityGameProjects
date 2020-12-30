using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class inGameOptions : MonoBehaviour
{
    public CanvasGroup confirmQuitCanvasGroup;
    public CanvasGroup controlsCanvasGroup;
    public CanvasGroup controlsPromptCanvasGroup;

    void Awake()
    {
        DoConfirmQuitNo(); // Disable the quit confirmation dialog box
        DoControlsOff();
        StartCoroutine(ShowControlsPrompt(1.2f, 2));
    }

    IEnumerator ShowControlsPrompt(float t, int times)
    {
        controlsPromptCanvasGroup.alpha = 1;
        controlsPromptCanvasGroup.interactable = true;
        controlsPromptCanvasGroup.blocksRaycasts = true;

        for(int i=0;i<times;i++)
        {
            yield return new WaitForSeconds(t);
        }
        float alpha = 1f;
        while (alpha >= 0)
        {
            controlsPromptCanvasGroup.alpha = alpha;
            yield return new WaitForSeconds(Time.deltaTime * 2);
            alpha -= Time.deltaTime;
        }
        controlsPromptCanvasGroup.alpha = 0;
        controlsPromptCanvasGroup.interactable = false;
        controlsPromptCanvasGroup.blocksRaycasts = false;
    }


    public void DoConfirmQuitNo()
    {
        Debug.Log("Back to the game");

        //disable the confirmation quit UI
        confirmQuitCanvasGroup.alpha = 0;
        confirmQuitCanvasGroup.interactable = false;
        confirmQuitCanvasGroup.blocksRaycasts = false;
    }

    public void DoConfirmQuitYes()
    {
        Debug.Log("Ok bye bye");
        // Application.Quit();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }

    public void DoQuit()
    {
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Check form quit confirmation");

        //enable interraction with confirmation gui and make visible
        confirmQuitCanvasGroup.alpha = 1;
        confirmQuitCanvasGroup.interactable = true;
        confirmQuitCanvasGroup.blocksRaycasts = true;
    }

    public void DoControlsOn()
    {
        Debug.Log("Checking controls...");
        controlsCanvasGroup.alpha = 1;
        controlsCanvasGroup.interactable = true;
        controlsCanvasGroup.blocksRaycasts = true;
    }

    public void DoControlsOff()
    {
        controlsCanvasGroup.alpha = 0;
        controlsCanvasGroup.interactable = false;
        controlsCanvasGroup.blocksRaycasts = false;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DoQuit();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            DoControlsOn();
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            DoControlsOff();
        }
    }
}
