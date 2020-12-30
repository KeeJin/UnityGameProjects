using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPun
{
    [SerializeField]
    GameObject[] SpawnPoints;

    [SerializeField]
    GameObject victoryScreenCanvasGroup;

    [SerializeField]
    GameObject gameResultsCanvasGroup;

    [SerializeField]
    GameObject circularPlatform;

    [SerializeField]
    Text timerShrinkText;

    [SerializeField]
    Text textOfShame;

    private Vector3 playerpos;
    private Vector3 spawnpoint;
    private float playerspawndist;
    private float distancethreshold;
    private float timerPUMass;
    private float timerPUStunner;
    public GameObject[] players;
    private float xpos;
    private float ypos;
    private int numberPlayers;
    private string localCharacter;
    private bool singlePlayer;
    private bool instantiate_check;
    private bool drawcheck;
    private bool fledgamecheck;
    private int CurrentRoomMaxPlayers;
    private float newScale;


    // Start is called before the first frame update
    void Start()
    {
        timerPUMass = 1.5f;
        timerPUStunner = 5f;
        victoryScreenCanvasGroup.SetActive(false);
        gameResultsCanvasGroup.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                SpawnStructures();
            }
            SpawnPlayer();
        }
        newScale = 15f;
        distancethreshold = 3f;
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 1)
        {
            Debug.Log("This is a Single Player mode.");
            singlePlayer = true;
        }
        else
        {
            singlePlayer = false;
        }
        instantiate_check = false;
        drawcheck = true;
        CurrentRoomMaxPlayers = (int) PhotonNetwork.CurrentRoom.MaxPlayers;
        StartCoroutine(SmallerPlatform(1f, 30));
    }

    IEnumerator SmallerPlatform(float t, int times)
    {
        for(int i=times;i>0;i--)
        {
            timerShrinkText.text = "Shrinking Platform in " + i.ToString() + "...";
            yield return new WaitForSeconds(t);
        }
        timerShrinkText.text = "Shrinking Platform...";
        float timerShrink = 0f;
        while (timerShrink <= 5f)
        {
            Vector3 currentScaleTemp = circularPlatform.transform.localScale;
            newScale = 20f - timerShrink;
            currentScaleTemp.x = newScale;
            currentScaleTemp.z = newScale;
            circularPlatform.transform.localScale = currentScaleTemp;
            yield return new WaitForSeconds(Time.deltaTime);
            timerShrink += Time.deltaTime;
        }
        timerShrinkText.text = "";
        newScale = circularPlatform.transform.localScale.x;
        float newBaseSize = newScale;
        for(int i=times;i>0;i--)
        {
            timerShrinkText.text = "Shrinking Platform in " + i.ToString() + "...";
            yield return new WaitForSeconds(t);
        }
        timerShrinkText.text = "Shrinking Platform...";
        timerShrink = 0f;
        while (timerShrink <= 5f)
        {
            Vector3 currentScaleTemp = circularPlatform.transform.localScale;
            newScale = newBaseSize - timerShrink;
            currentScaleTemp.x = newScale;
            currentScaleTemp.z = newScale;
            circularPlatform.transform.localScale = currentScaleTemp;
            yield return new WaitForSeconds(Time.deltaTime);
            timerShrink += Time.deltaTime;
        }
        timerShrinkText.text = "";
        newScale = circularPlatform.transform.localScale.x;
    }

    void SpawnStructures()
    {
        PhotonNetwork.Instantiate("CubeWallTall", new Vector3(0, 2f, 0), Quaternion.identity);
        PhotonNetwork.Instantiate("CubeWallNormal", new Vector3(7f, 2f, 0), Quaternion.identity);
        PhotonNetwork.Instantiate("CubeWallNormal", new Vector3(-7f, 2f, 0), Quaternion.identity);

        PhotonNetwork.Instantiate("CubeWallNormal", new Vector3(0, 2f, 3f), Quaternion.identity);
        PhotonNetwork.Instantiate("CubeWallNormal", new Vector3(0, 2f, -3f), Quaternion.identity);
        PhotonNetwork.Instantiate("CubeWallNormal", new Vector3(0, 3f, 3f), Quaternion.identity);
        PhotonNetwork.Instantiate("CubeWallNormal", new Vector3(0, 3f, -3f), Quaternion.identity);
    }

    void SpawnPlayer()
    {
        int player = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        if (player > 4)
        {
            player -= 5;
        }
        localCharacter = PhotonNetwork.LocalPlayer.CustomProperties["CharacterSelected"].ToString();
        GameObject Player = PhotonNetwork.Instantiate(localCharacter, SpawnPoints[player].transform.position, Quaternion.identity);
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void VictoryScreen(GameObject player)
    {
        Camera.main.GetComponent<cameraSpectate>().DeactivateSpectatorCam();
        victoryScreenCanvasGroup.SetActive(true);
        if (player.GetComponent<PhotonView>().IsMine)
        {
            Destroy(player.GetComponent<animationStateController>());
            Destroy(player.GetComponent<CameraWork>());
            Destroy(player.GetComponent<mouseController>());
        }
        // PhotonNetwork.Destroy(player);
    }

    void EndGameScreen(int CurrentRoomMaxPlayers, string winner = "")
    {
        Camera.main.GetComponent<cameraSpectate>().DeactivateSpectatorCam();
        gameResultsCanvasGroup.SetActive(true);
        if (winner == "")
        {
            Debug.Log("Draw!!");
            textOfShame.text = "Ended without a winner. What a waste of time...";
        }
        else
        {
            if (CurrentRoomMaxPlayers <= 2)
            {
                textOfShame.text = winner + " pwned yo weak ass.";
            }
            else
            {
                textOfShame.text = winner + " pwned all yo weak asses.";
            }
        }
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void FledGameCheck()
    {
        fledgamecheck = true;
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if(CurrentRoomMaxPlayers == players.Length)
        {
            instantiate_check = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        if (!singlePlayer && players.Length == 1 && instantiate_check)
        {
            GameObject player = players[0];
            if (player.transform.position.y > -15f)
            {
                drawcheck = false;
                string winner = player.GetComponent<PhotonView>().Owner.NickName;
                Debug.Log("We have a winner! Victory goes to " + winner);
                if(player.GetComponent<PhotonView>().IsMine)
                {
                    Debug.Log("Flash screen for the champ.");
                    VictoryScreen(player);
                }
                else
                {
                    Debug.Log("Flash screen for the losers.");
                    EndGameScreen(CurrentRoomMaxPlayers, winner);    
                }
            }
        }

        if (!singlePlayer && players.Length == 0 && instantiate_check && drawcheck && !fledgamecheck)
        {
            EndGameScreen(CurrentRoomMaxPlayers);
        }

        // ####################### Every 1.5 seconds, 1 new mass power-up will spawn. ############################
        if (timerPUMass > 0)
        {
            timerPUMass -= Time.deltaTime;
        }
        else
        {
            bool spawnable = true;

            // Create vector with a random spawn position
            float halfNewScale = newScale/2;
            xpos = Random.Range(-halfNewScale, halfNewScale);
            ypos = Random.Range(-halfNewScale, halfNewScale);
            spawnpoint = new Vector3(xpos, 0.5f, ypos);

            foreach (GameObject player in players)
            {
                // Debug.Log("There is someone named: " + player.GetComponent<PhotonView>().Owner.NickName + " in the game!");
                playerpos = player.transform.position;
                playerspawndist = Vector3.Distance(spawnpoint, playerpos);
                if (playerspawndist<distancethreshold)
                {
                    spawnable = false;
                }
            }
            if (Vector3.Distance(spawnpoint, new Vector3(0,0.5f,0)) >= halfNewScale)
            {
                spawnable = false;
            }
            
            if (spawnable && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("powerUpMass", spawnpoint, Quaternion.identity);
            }
            timerPUMass = 1.5f; // Resets countdown to 1.5s
        }



        // ############################ Every 5 seconds, 1 new strong power-up will spawn. ##############################
        if (timerPUStunner > 0)
        {
            timerPUStunner -= Time.deltaTime;
        }
        else
        {
            bool spawnable = true;

            // Create vector with a random spawn position
            float halfNewScale = newScale/2;
            xpos = Random.Range(-halfNewScale, halfNewScale);
            ypos = Random.Range(-halfNewScale, halfNewScale);
            spawnpoint = new Vector3(xpos, 0.5f, ypos);

            foreach (GameObject player in players)
            {
                playerpos = player.transform.position;
                playerspawndist = Vector3.Distance(spawnpoint, playerpos);
                if (playerspawndist < distancethreshold)
                {
                    spawnable = false;
                }
            }
            if (Vector3.Distance(spawnpoint, new Vector3(0,0.5f,0)) >= halfNewScale)
            {
                spawnable = false;
            }
            
            if (spawnable && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("powerUpStunner", spawnpoint, Quaternion.identity);
            }
            timerPUStunner = 5f; // Resets countdown to 1.5s
        }
    }
}
