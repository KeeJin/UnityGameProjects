using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller class for handling "Quit Game" option.
public class QuitHandler : MonoBehaviour
{
    public CanvasGroup uiCanvasGroup;
    public CanvasGroup confirmQuitCanvasGroup;
    
    private void Awake()
    {
        DoConfirmQuitNo(); // Disable the quit confirmation dialog box
    }

    public void DoConfirmQuitNo()
    {
        Debug.Log("Back to the game");

        //enable the normal UI
        uiCanvasGroup.alpha = 1;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

        //disable the confirmation quit UI
        confirmQuitCanvasGroup.alpha = 0;
        confirmQuitCanvasGroup.interactable = false;
        confirmQuitCanvasGroup.blocksRaycasts = false;
    }

    public void DoConfirmQuitYes()
    {
        Debug.Log("Ok bye bye");
        Application.Quit();
    }

    public void DoQuit()
    {
        Debug.Log("Check form quit confirmation");

        //reduce the visibility of normal UI, and disable all interraction
        uiCanvasGroup.alpha = 0.08f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;

        //enable interraction with confirmation gui and make visible
        confirmQuitCanvasGroup.alpha = 1;
        confirmQuitCanvasGroup.interactable = true;
        confirmQuitCanvasGroup.blocksRaycasts = true;
    }
}
