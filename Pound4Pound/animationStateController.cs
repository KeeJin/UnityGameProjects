using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class animationStateController : MonoBehaviourPun
{
    private Animator animator;
    private Rigidbody m_Rigidbody;
    int isWalkingHash;
    int isSprintingHash;
    int isTwerkingHash;
    int isJumpingForwardHash;
    int isJumpingStandHash;
    int isFallingHash;
    int isStunnedHash;
    float m_Speed;
    Vector3 directionInput;
    private bool stuncheck;
    private float newScale;
    private float maxJumpHeight;
    private float minJumpHeight;

    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<animationStateController>());
        }
        stuncheck = false;
        animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Speed = 1.0f;
        isWalkingHash = Animator.StringToHash("isWalking");
        isSprintingHash = Animator.StringToHash("isSprinting");
        isTwerkingHash = Animator.StringToHash("isTwerking");
        isJumpingForwardHash = Animator.StringToHash("isJumpingForward");
        isJumpingStandHash = Animator.StringToHash("isJumpingStanding");
        isFallingHash = Animator.StringToHash("isFalling");
        isStunnedHash = Animator.StringToHash("isStunned");
        directionInput = new Vector3(0, 0, 1);
        newScale = 15f;
        maxJumpHeight = 4f;
        minJumpHeight = -2f;
        StartCoroutine(ShrinkSpawn(1f, 30));
    }

    bool ForwardJumpAnimatorIsPlaying(string stateName = "ForwardJump")
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    void Update()
    {
        if (!stuncheck)
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.forward * 1 + transform.position);

            bool forwardPressed = Input.GetKey("w");
            bool spacePressed = Input.GetKey(KeyCode.Space);
            bool forwardReleased = Input.GetKeyUp("w");
            bool sprintPressed = Input.GetKey(KeyCode.LeftShift);
            bool isWalking = animator.GetBool(isWalkingHash);
            bool isSprinting = animator.GetBool(isSprintingHash);
            bool isTwerking = animator.GetBool(isTwerkingHash);
            bool isJumpingForward = animator.GetBool(isJumpingForwardHash);
            bool isJumpingStand = animator.GetBool(isJumpingStandHash);
            bool isFalling = animator.GetBool(isFallingHash);
            
            if (ForwardJumpAnimatorIsPlaying())
            {
                Debug.Log("In Forward Jump state.");
                animator.speed = 1.0f;
            }
            else
            {
                Debug.Log("Not in Forward Jump state.");
                animator.speed = 1.0f / transform.localScale.x;
            }

            if(this.GetComponent<Rigidbody>().velocity.y <= -4f)
            {
                animator.speed = 1.0f;
                // Debug.Log("Ah!! Falling!!");
                animator.SetBool(isFallingHash, true);
            }
            else
            {
                animator.SetBool(isFallingHash, false);
            }
            float currentHeight = this.transform.position.y;
            float distFromCenter = Vector3.Distance(Vector3.zero, new Vector3(this.transform.position.x, 0, this.transform.position.z));
            if(!isJumpingStand && spacePressed)
            {
                if(currentHeight < maxJumpHeight && (distFromCenter < (newScale/2 + 7f)) && currentHeight > minJumpHeight)
                {
                    float myMass = this.GetComponent<Rigidbody>().mass;
                    animator.SetBool(isJumpingStandHash, true);
                    this.GetComponent<Rigidbody>().AddForce(transform.up * 10f * (myMass/5.0f), ForceMode.Impulse);
                }
            }

            if((isJumpingForward || isJumpingStand) && !spacePressed)
            {
                animator.SetBool(isJumpingStandHash, false);
                animator.SetBool(isJumpingForwardHash, false);
            }

            if(forwardPressed && !isJumpingForward && spacePressed)
            {
                if(currentHeight < maxJumpHeight && (distFromCenter < (newScale/2 + 7f)) && currentHeight > minJumpHeight)
                {
                    float myMass = this.GetComponent<Rigidbody>().mass;
                    animator.SetBool(isJumpingForwardHash, true);
                    this.GetComponent<Rigidbody>().AddForce(transform.forward * 15f * (myMass/5.0f) + transform.up * 20f * (myMass/5.0f), ForceMode.Impulse);
                }
            }

            if(isWalking && !isSprinting)
            {
                m_Speed = 1.0f / transform.localScale.x;
                transform.Translate(directionInput * m_Speed * Time.deltaTime);
            }

            if(isWalking && isSprinting)
            {
                m_Speed = 3.0f / transform.localScale.x;
                transform.Translate(directionInput * m_Speed * Time.deltaTime);
            }

            // if "w" key is pressed
            if(forwardPressed && !isWalking)
            {
                animator.SetBool(isTwerkingHash, false);
                animator.SetBool(isWalkingHash, true);
            }

            // if "w" key is not pressed
            if(forwardReleased && isWalking)
            {
                // Idle
                Debug.Log("Idle");
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isSprintingHash, false);
            }

            // Walking to Sprinting (currently not sprinting, is walking and left shift is held)
            if(!isSprinting && (isWalking && sprintPressed))
            {
                // Sprinting
                Debug.Log("Sprinting");
                m_Speed = 6.0f;
                animator.SetBool(isSprintingHash, true);
            }

            // Sprinting to Walking (currently sprinting, "w" is held but left shift is not held)
            if(isSprinting && (forwardPressed && !sprintPressed))
            {
                animator.SetBool(isSprintingHash, false);
            }

            if(Input.GetKeyDown("t"))
            {
                // Twerk
                Debug.Log("Twerking");
                animator.SetBool(isTwerkingHash, true);
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "StunnerBullet")
        {
            Debug.Log("Hit by stunner!");
            StartCoroutine(Stunned(1f, 5));
        }
    }

    IEnumerator Stunned(float t, int times)
    {
        stuncheck = true;
        animator.speed = 1.0f;
        animator.SetBool(isStunnedHash, true);
        for(int i=0;i<times;i++)
        {
            yield return new WaitForSeconds(t);
        }
        animator.SetBool(isStunnedHash, false);
        yield return new WaitForSeconds(0.8f);
        stuncheck = false;
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
}
