using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject LobbyUI;

    [SerializeField]
    GameObject searchPanel;

    [SerializeField]
    GameObject loadingText;

    [SerializeField]
    InputField inputField;

    [SerializeField]
    GameObject warningText;

    [SerializeField]
    Text searchingText;

    [SerializeField]
    Text serverName;

    [SerializeField]
    CanvasGroup findMatchCanvasGroup;

    [SerializeField]
    Dropdown dropdown;

    [SerializeField]
    GameObject XZB;

    [SerializeField]
    GameObject Co0ky;

    [SerializeField]
    Button charSelectionBtn;

    [SerializeField]
    GameObject stopFindingBtn;

    public static string NameTag;
    private string playerSearchFrac;
    private int playerCount = 1;
    private string characterSelected;
    private int totalLives = 5;

    void Start()
    {
        LobbyUI.SetActive(false);
        loadingText.SetActive(true);
        warningText.SetActive(false);
        characterSelected = "XiaoZharBor";
        Co0ky.GetComponent<Animator>().enabled = false;

        // Connect to Photon Server
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("asia");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Photon Server on " + PhotonNetwork.CloudRegion + "Server!");
        serverName.text = "Successfully connected to Photon Server on " + PhotonNetwork.CloudRegion + "Server!";
        PhotonNetwork.AutomaticallySyncScene = true;
        loadingText.SetActive(false);
        LobbyUI.SetActive(true);
        findMatchCanvasGroup.interactable = false;
        findMatchCanvasGroup.blocksRaycasts = false;
        inputField.ActivateInputField();
    }

    public void NumberOfPlayersOption()
    {
        int dropdownIndex = dropdown.value;
        playerCount = dropdownIndex + 1;
        // Debug.Log("Dropdown Index: " + dropdownIndex);
        Debug.Log("New Player Count: " + playerCount);
    }

    public void changeCharacter()
    {
        if (characterSelected == "XiaoZharBor")
        {
            charSelectionBtn.GetComponentInChildren<Text>().text = "DUMDUM JOE";
            characterSelected = "Co0ky";
            XZB.GetComponent<Animator>().enabled = false;
            Co0ky.GetComponent<Animator>().enabled = true;
        }
        else
        {
            charSelectionBtn.GetComponentInChildren<Text>().text = "XIAO ZHARBOR";
            characterSelected = "XiaoZharBor";
            XZB.GetComponent<Animator>().enabled = true;
            Co0ky.GetComponent<Animator>().enabled = false;
        }
    }

    public void FindMatch()
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            warningText.SetActive(true);
            findMatchCanvasGroup.interactable = false;
            findMatchCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            warningText.SetActive(false);
            searchPanel.SetActive(true);
            LobbyUI.SetActive(false);
            NameTag = inputField.text;
            PhotonNetwork.LocalPlayer.NickName = NameTag;
            Hashtable setPlayerProperties = new Hashtable();
            setPlayerProperties.Add("CharacterSelected", (string)characterSelected);
            setPlayerProperties.Add("LivesLeft", (int)totalLives);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setPlayerProperties);

            // Try to join existing room - if it fails, create a new room.
            PhotonNetwork.JoinRandomRoom(null, System.Convert.ToByte(playerCount));
            Debug.Log("Searching for room...");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Debug.Log("Couldn't find a room. Creating a new room...");
        MakeRoom();
    }

    void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = System.Convert.ToByte(playerCount)
        };
        PhotonNetwork.CreateRoom("Room No. " + randomRoomName, roomOptions);
        // Debug.Log("Room created, waiting for other players...");
        searchingText.text = "SEARCHING FOR MATCH: 1/" + playerCount.ToString() + " PLAYERS...";
    }

    public override void OnCreatedRoom()
    {
        Hashtable setRoomProperties = new Hashtable();
        setRoomProperties.Add("Rank", (int)playerCount);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setRoomProperties);
        if (PhotonNetwork.CurrentRoom.PlayerCount == playerCount && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Single Player Mode selected. Starting game...");
            stopFindingBtn.SetActive(false);
            // Switch to Main Game scene
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void StopSearch()
    {
        searchPanel.SetActive(false);
        LobbyUI.SetActive(true);
        PhotonNetwork.LeaveRoom();
        // Debug.Log("Stopped search, back to Game Lobby.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerSearchFrac = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + playerCount.ToString();
        searchingText.text = "SEARCHING FOR MATCH: " + playerSearchFrac + " PLAYERS...";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerSearchFrac = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + playerCount.ToString();
        searchingText.text = "SEARCHING FOR MATCH: " + playerSearchFrac + " PLAYERS...";
        if (PhotonNetwork.CurrentRoom.PlayerCount == playerCount && PhotonNetwork.IsMasterClient)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + " players. Starting game...");
            stopFindingBtn.SetActive(false);
            // Switch to Main Game scene
            PhotonNetwork.LoadLevel(1);
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            findMatchCanvasGroup.interactable = false;
            findMatchCanvasGroup.blocksRaycasts = false;
            warningText.SetActive(true);
        }
        else
        {
            if(Input.GetKeyUp(KeyCode.Return))
            {
                FindMatch();
            }
            findMatchCanvasGroup.interactable = true;
            findMatchCanvasGroup.blocksRaycasts = true;
            warningText.SetActive(false);
        }
    }
}
