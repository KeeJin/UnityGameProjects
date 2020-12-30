using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace DigitalRuby.PyroParticles
{
    public class CollisionHandler_Player : MonoBehaviourPun
    {
        public Text massStat;
        public Text livesLeft;
        
        private float myMassMultiplier;
        private float massBuff;
        private Vector3 contactVec;
        private bool massBuffed;
        private float massDecay;
        private float massBase;
        private float buffTimer;
        private float countdownTimer;
        private Vector3 playerRespawnPos;
        private string localCharacter;
        private int localLivesLeft;
        private float myMass;
        private bool gameover;
        private bool singlePlayer;
        private int ranking;
        private GameObject[] players;
        private float timer;
        public Text timerText;
        public Button spectateBtn;
        private float newScale;
        private int ammoStunner;
        private int ammoFirebolt;
        private bool fireBoltCheck;
        public Text stunnerStat;
        public AudioSource popPowerup;
        private bool stuncheck;
        private bool invulnerable;

        [SerializeField]
        GameObject gameOverCanvasGroup;

        [SerializeField]
        Text RankText;

        [SerializeField]
        Text spectateBtnTxt;

        [SerializeField]
        Text fireBoltText;

        [SerializeField]
        Text respawn;

        // [SerializeField]
        // Text forceMultiplierText;

        [SerializeField]
        float impulseMultiplier;
        
        [SerializeField]
        GameObject XZBShield;

        [SerializeField]
        GameObject Co0kyShield;

        void Start()
        {
            gameOverCanvasGroup.SetActive(false);
            if (!photonView.IsMine)
            {
                Destroy(GetComponent<CollisionHandler_Player>());
            }
            ammoStunner = 1;
            ammoFirebolt = 0;
            countdownTimer = 0.2f;
            timer = 3f;
            newScale = 15f;
            fireBoltCheck = false;
            stuncheck = false;
            invulnerable = false;

            if (PhotonNetwork.CurrentRoom.MaxPlayers == 1)
            {
                singlePlayer = true;
            }
            else
            {
                singlePlayer = false;
            }
            if (!singlePlayer)
            {
                gameover = false;
                localLivesLeft = (int) PhotonNetwork.LocalPlayer.CustomProperties["LivesLeft"];
            }

            localCharacter = PhotonNetwork.LocalPlayer.CustomProperties["CharacterSelected"].ToString();
            if (localCharacter == "XiaoZharBor")
            {
                this.GetComponent<Rigidbody>().mass = 6.0f;
                impulseMultiplier = 158.5f;
                myMassMultiplier = 1f;
                massBase = 5.0f;
                massBuff = 8.0f;
                massDecay = 0.5f;
            }
            else if (localCharacter == "Co0ky")
            {
                impulseMultiplier = 50f;
                myMassMultiplier = 1f;
                this.GetComponent<Rigidbody>().mass = 10.0f;
                massBase = 4.0f;
                massBuff = 10.0f;
                massDecay = 1.1f;
            }
            StartCoroutine(ShrinkSpawn(1f, 30));
        }

        // Update is called once per frame
        void Update()
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            myMass = this.GetComponent<Rigidbody>().mass;
            bool sprintPressed = Input.GetKey(KeyCode.LeftShift);
            bool forwardPressed = Input.GetKey("w");
            bool spacePressed = Input.GetKey(KeyCode.Space);
            bool ePressed = Input.GetKeyDown("e");
            bool qPressed = Input.GetKeyDown("q");
            Vector3 currentScaleTemp = this.transform.localScale;
            currentScaleTemp.x = (myMass - massBase) / 150.0f + 1f;
            currentScaleTemp.z = (myMass - massBase) / 150.0f + 1f;
            currentScaleTemp.y = (myMass - massBase) / 180.0f + 1f;
            this.transform.localScale = currentScaleTemp;

            if (ammoFirebolt == 0 && !fireBoltCheck)
            {
                Debug.Log("Creating new FireBolt...");
                StartCoroutine(FireBolt(1f, 3));
                // CountdownTimerFirebolt();
            }

            if (ammoFirebolt > 0 && !fireBoltCheck)
            {
                fireBoltText.text = "FIREBOLT: READY";
            }

            if (ammoFirebolt > 0 && !stuncheck)
            {
                // Checks for 'e' press
                if (qPressed)
                {
                    // Instantiate stunner bullet
                    GameObject  bulletobj = PhotonNetwork.Instantiate("Firebolt", transform.position + new Vector3(0, 1f, 0) + transform.forward/2, transform.rotation) as GameObject;
                    Physics.IgnoreCollision(this.GetComponent<CapsuleCollider>(), bulletobj.GetComponentInChildren<SphereCollider>());
                    FireProjectileScript projectileScript = bulletobj.GetComponent<FireProjectileScript>();
                    if (projectileScript != null)
                    {
                        // make sure we don't collide with other fire layers
                        projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
                    }
                    ammoFirebolt -= 1;
                }
            }

            if (ammoStunner > 0 && !stuncheck)
            {
                // Checks for 'e' press
                if (ePressed)
                {
                    // stunnerFire.Play();
                    // Instantiate stunner bullet
                    GameObject  bulletobj = PhotonNetwork.Instantiate("stunnerBullet", transform.position + new Vector3(0, 1f, 0), transform.rotation) as GameObject;
                    Physics.IgnoreCollision(this.GetComponent<CapsuleCollider>(), bulletobj.GetComponent<CapsuleCollider>());
                    ammoStunner -= 1;
                }
            }

            stunnerStat.text = "STUNNERS: " + ammoStunner.ToString();

            if(!spacePressed && sprintPressed && forwardPressed && myMass > massBase)
            {
                this.GetComponent<Rigidbody>().mass -= Time.deltaTime;
            }
            
            if(spacePressed && sprintPressed && forwardPressed && myMass > massBase)
            {
                this.GetComponent<Rigidbody>().mass -= 0.01f;
            }

            if(!spacePressed && !sprintPressed && forwardPressed && myMass > massBase)
            {
                this.GetComponent<Rigidbody>().mass -= Time.deltaTime / 2;
            }

            if(massBuffed)
            {
                countdownTimer -= Time.deltaTime;
                if (countdownTimer<=0)
                {
                    countdownTimer = 1.0f;
                    this.GetComponent<Rigidbody>().mass -= massDecay;
                }
                if (this.GetComponent<Rigidbody>().mass <= massBase)
                {
                    this.GetComponent<Rigidbody>().mass = massBase;
                    massBuffed = false;
                }
            }

            massStat.text = "MASS: " + (Mathf.Round(this.GetComponent<Rigidbody>().mass * 10) / 10.0).ToString();
            if (!singlePlayer)
            {
                livesLeft.text = "LIVES LEFT: " + localLivesLeft.ToString();
            }
            
            foreach (GameObject player in players)
            {
                if (player.transform.position.y < -14.73f)
                {
                    StartCoroutine(InvulnerabilityShield(1f, 5, player));
                }
            }
            
            // Check for death
            if(this.transform.position.y < -15f)
            {
                Debug.Log("Died!");
                if (singlePlayer)
                {
                    Respawn(players);
                }
                else
                {
                    localLivesLeft -= 1;
                    livesLeft.text = "Lives left: " + localLivesLeft.ToString();
                    if (!gameover)
                    {
                        if(localLivesLeft <= 0)
                        {
                            livesLeft.text = "";
                            massStat.text = "";
                            GameOver();
                            gameover = true;
                        }
                        else
                        {
                            Respawn(players);
                        }
                    }
                }
            }
        }

        void Respawn(GameObject[] players)
        {
            bool spawnable = true;
            float halfNewScale = 4f;
            playerRespawnPos = new Vector3(Random.Range(-halfNewScale, halfNewScale), 5f, Random.Range(-(halfNewScale), halfNewScale));
            float distancethreshold = 1f;
            Vector3 playerpos;
            float playerspawndist;

            foreach (GameObject player in players)
            {
                playerpos = player.transform.position;
                playerspawndist = Vector3.Distance(playerRespawnPos, playerpos);
                if (playerspawndist < distancethreshold)
                {
                    spawnable = false;
                }
            }
            
            while (!spawnable || Vector3.Distance(playerRespawnPos, new Vector3(0, 5f, 0)) >= halfNewScale)
            {
                playerRespawnPos = new Vector3(Random.Range(-halfNewScale, halfNewScale), 5f, Random.Range(-halfNewScale, halfNewScale));
                spawnable = true;
                foreach (GameObject player in players)
                {
                    playerpos = player.transform.position;
                    playerspawndist = Vector3.Distance(playerRespawnPos, playerpos);
                    if (playerspawndist < distancethreshold)
                    {
                        spawnable = false;
                    }
                }
            }
            this.transform.position = playerRespawnPos;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().mass = massBase;
            StartCoroutine(Invulnerability(1f, 5));
        }

        void GameOver()
        {
            // Calculate ranking
            int ranking = 1;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.transform.position.y > -15f)
                {
                    ranking += 1;
                }
            }
            Debug.Log("Final Rank: " + ranking.ToString());
            gameOverCanvasGroup.SetActive(true);
            if (ranking == 1)
            {
                RankText.text = "You outlasted everyone! Ha!";
                spectateBtnTxt.text = "MATCH RESULTS";
            }
            else if (ranking == 2)
            {
                RankText.text = "You finished in 2nd place! Wasted...";
                spectateBtnTxt.text = "MATCH RESULTS";
            }
            else if (ranking == 3)
            {
                RankText.text = "You finished in 3rd place! Wasted...";
            }
            else if (ranking >3)
            {
                RankText.text = "You finished in last place. Pathetic.";
            }
            Cursor.lockState = CursorLockMode.Confined;
            // ranking -= 1;
            Debug.Log("Rank: " + ranking.ToString());
            StartCoroutine(SpectateTimer(1f, 5));
        }

        IEnumerator ShrinkSpawn(float t, int times)
        {
            for(int i=times;i>0;i--)
            {
                yield return new WaitForSeconds(t);
            }
            float timerShrink = 0f;
            while (timerShrink <= 5f)
            {
                newScale = 20f - timerShrink;
                yield return new WaitForSeconds(Time.deltaTime);
                timerShrink += Time.deltaTime;
            }
            newScale = 20f - timerShrink;
            float newBaseSize = newScale;
            for(int i=times;i>0;i--)
            {
                yield return new WaitForSeconds(t);
            }
            timerShrink = 0f;
            while (timerShrink <= 5f)
            {
                newScale = newBaseSize - timerShrink;
                yield return new WaitForSeconds(Time.deltaTime);
                timerShrink += Time.deltaTime;
            }
            newScale = newBaseSize - timerShrink;
        }

        IEnumerator SpectateTimer(float t, int times)
        {
            for(int i=times;i>0;i--)
                {
                    timerText.text = "Entering spectator mode in " + i.ToString() + "...";
                    yield return new WaitForSeconds(t);
                }
            timerText.text = "Entering spectator mode in 0...";
            //Invoke press btn
            spectateBtn.onClick.Invoke();
        }

        IEnumerator Invulnerability(float t, int times)
        {
            invulnerable = true;
            for(int i=times;i>0;i--)
            {
                respawn.text = "INVULNERABLE... (" + i.ToString() + "s)";
                yield return new WaitForSeconds(t);
            }
            respawn.text = "";
            yield return new WaitForSeconds(0.1f);
            invulnerable = false;
        }

        IEnumerator InvulnerabilityShield(float t, int times, GameObject player)
        {
            string playerCharacter = player.GetComponent<PhotonView>().Owner.CustomProperties["CharacterSelected"].ToString();
            
            if (playerCharacter == "XiaoZharBor")
            {
                GameObject forcefield = Instantiate(XZBShield, player.transform.position, Quaternion.identity);
                Physics.IgnoreCollision(player.GetComponent<CapsuleCollider>(), forcefield.GetComponent<SphereCollider>());
                forcefield.transform.parent = player.transform;
                for(int i=times;i>0;i--)
                {
                    yield return new WaitForSeconds(t);
                }
                yield return new WaitForSeconds(0.1f);

                Destroy(forcefield);
            }
            else if (playerCharacter == "Co0ky")
            {
                GameObject forcefield = Instantiate(Co0kyShield, player.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
                Physics.IgnoreCollision(player.GetComponent<CapsuleCollider>(), forcefield.GetComponent<SphereCollider>());
                forcefield.transform.parent = player.transform;
                for(int i=times;i>0;i--)
                {
                    yield return new WaitForSeconds(t);
                }
                yield return new WaitForSeconds(0.1f);

                Destroy(forcefield);
            }   
        }

        IEnumerator FireBolt(float t, int times)
        {
            fireBoltCheck = true;
            for(int i=times;i>0;i--)
            {
                fireBoltText.text = "FIREBOLT: Available in " + i.ToString() + "...";
                yield return new WaitForSeconds(t);
            }
            ammoFirebolt += 1;
            fireBoltCheck = false;
        }

        public void Spectate()
        {
            Camera.main.GetComponent<cameraSpectate>().ActivateSpectatorCam();
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
        }

        public void ExitGame()
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel(0);
        }

        private void OnCollisionEnter(Collision collision) 
        {
            contactVec = collision.contacts[0].normal;
            if(collision.collider.tag == "Player" && !invulnerable) 
            {
                float myMass = this.GetComponent<Rigidbody>().mass;
                float otherMass = collision.rigidbody.mass;
                Debug.Log("My Mass: " + myMass.ToString());
                // Debug.Log("Other Mass: " + otherMass.ToString());
                float impulsemagnitude = collision.impulse.magnitude;
                impulsemagnitude = Mathf.Clamp(impulsemagnitude, 11f, 15f);
                // forceMultiplierText.text = "Force multiplier: " + impulsemagnitude.ToString();
                Debug.Log("Impulse: " + impulsemagnitude.ToString());
                // float bias = 1;
                float totalForceMultiplier = ((impulseMultiplier * impulsemagnitude) / (myMass * myMassMultiplier));
                Debug.Log("Total Force Multiplier: " + totalForceMultiplier.ToString());
                // collision.collider.GetComponent<Rigidbody>().AddForce(contactVec * (impulseMultiplier + ((myMass - otherMass) * 10 / Mathf.Min(myMass, otherMass))), ForceMode.Impulse);
                collision.collider.GetComponent<Rigidbody>().AddForce(contactVec * totalForceMultiplier, ForceMode.Impulse);
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "PowerUpMass")
            {
                popPowerup.Play();
                // Debug.Log("Collided into MASS power-up.");
                MassBuff();
            }

            if (other.tag == "PowerUpStunner")
            {
                popPowerup.Play();
                if (ammoStunner<3)
                {
                    ammoStunner += 1;
                }
                // Debug.Log("Collided into STUNNER power-up.");
            }

            if (other.tag == "StunnerBullet")
            {
                // Debug.Log("Hit by stunner!");
                StartCoroutine(Stunned(1f, 5));
            }
        }

        IEnumerator Stunned(float t, int times)
        {
            stuncheck = true;
            for(int i=0;i<times;i++)
            {
                yield return new WaitForSeconds(t);
            }
            yield return new WaitForSeconds(0.8f);
            stuncheck = false;
        }

        private void MassBuff()
        {
            massBuffed = true;
            if (this.GetComponent<Rigidbody>().mass < 250f)
            {
                this.GetComponent<Rigidbody>().mass += massBuff;
            }
        }
    }
}