using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    
     float moveSpeed =4;
    public float walkSpeed =4;
    public float runSpeed =8;
    private bool isMoving;
    private Vector3 input;
    private Vector3 prevInput;
    private Animator animator;
    private Animator pikaAnimator;
    public GameObject pikachu;
    public GameObject playerSprite;
    public GameObject pikaSprite;
    public AudioSource bumbSound;
    public LayerMask solidObjectsLayer;
    public LayerMask longGrassLayer;
    public LayerMask southSlope;
    public LayerMask westSlope;
    public LayerMask northSlope;
    public LayerMask eastSlope;
    public LayerMask pikachuLayer;
    private AudioSource audioSource;
    public GameObject route1;
    public GameObject world;
    public GameObject battle;
    float delay = 0.1f;
    float remainingDelay;
    bool incTimer = false;
    UnityEngine.Object collidedObject;
    bool onSouthSlope = false;
    bool onNorthSlope = false;
    bool onWestSlope = false;
    bool onEastSlope = false;
    bool pikaIsMoving;
    bool isRunning;
    bool m_Started;

    public event Action onEncounter;

    Vector3 positionChange;
    Vector3 prevPositionChange;



     PokemonBase wildPokemon;



    public enum MoveDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        None = 4
    }
    MoveDirection playerFaceing = MoveDirection.Down;
    MoveDirection prevPlayerFaceing;




    void Start()
    {
        route1.SetActive(true);
        animator = playerSprite.GetComponent<Animator>();
        pikaAnimator = pikaSprite.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        m_Started = true;
        pikachu.GetComponent<AudioSource>().Stop();
    }

    // Update is called once per frame
    public void  HandleUpdate()
    {

       

        if (!isMoving && !pikaIsMoving)
        {
            PlayPikaCry();
            CheckRunning();


            input.x = Input.GetAxisRaw("Horizontal");
            input.z = Input.GetAxisRaw("Vertical");
            

            if (input.z != 0)
            {
                input.x = 0;
            }
            if (prevPositionChange == Vector3.zero)
            {
                prevPositionChange = Vector3.back;
            }

            if (input != Vector3.zero)
            {
                prevPlayerFaceing = playerFaceing;
                SetFaceingDirection();
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.z);
                if (playerFaceing != prevPlayerFaceing)
                {
                    remainingDelay = delay;
                }
                if (remainingDelay > 0f)
                {
                    remainingDelay -= Time.deltaTime;
                    return;
                }

                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.z);
                var targetPos = transform.parent.position;
                var pikaTargetPos = pikachu.transform.position;
                positionChange = Vector3.zero;
                positionChange.x += input.x;
                positionChange.z += input.z;
                var test = targetPos + positionChange;
                if (isWalkerble(test))
                {
                    if (onSouthSlope)
                    {
                        if (input.z > 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onSouthSlope = false;
                        }
                        else if (input.z < 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onSouthSlope = false;
                        }
                    }
                    else if (onNorthSlope)
                    {
                        if (input.z > 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onNorthSlope = false;
                        }
                        else if (input.z < 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onNorthSlope = false;
                        }
                    }
                    else if (onWestSlope)
                    {
                        if (input.x > 0) {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onWestSlope = false;
                        }
                        else if (input.x < 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onWestSlope = false;
                        }

                    }
                    else if (onEastSlope)
                    {
                        if (input.x > 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onEastSlope = false;
                        }
                        else if (input.x < 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onEastSlope = false;
                        }
                    }
                    else if (isSouthSlope(test))
                    {
                        if (input.z > 0) positionChange += new Vector3(0, 0.5f, 0);
                        else if (input.z < 0) positionChange += new Vector3(0, -0.5f, 0);
                        onSouthSlope = true;
                    }
                    else if (isNorthSlope(test))
                    {
                        if (input.z > 0) positionChange += new Vector3(0, -0.5f, 0);
                        else if (input.z < 0) positionChange += new Vector3(0, 0.5f, 0);
                        onNorthSlope = true;
                    }
                    else if (isWestSlope(test))
                    {
                        if (input.x > 0) positionChange += new Vector3(0, 0.5f, 0);
                        else if (input.x < 0) positionChange += new Vector3(0, -0.5f, 0);
                        onWestSlope = true;
                    }
                    else if (isEastSlope(test))
                    {
                        if (input.x > 0) positionChange += new Vector3(0, -0.5f, 0);
                        else if (input.x < 0) positionChange += new Vector3(0, 0.5f, 0);
                        onEastSlope = true;
                    }

                    pikaAnimator.SetFloat("moveX", prevInput.x);
                    pikaAnimator.SetFloat("moveY", prevInput.z);
                    // Debug.Log(positionChange);
                    //Debug.Log(prevPositionChange);

                    targetPos += positionChange;
                    pikaTargetPos += prevPositionChange;

                    
                    StartCoroutine(Move(targetPos));
                    StartCoroutine(PikaMove(pikaTargetPos));
                    prevPositionChange = positionChange;
                    prevInput = input;


                }

            }
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isMoving", isMoving);
            


        }
    }
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.parent.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.parent.position = targetPos;
        isMoving = false;
        CheckForEncounters(targetPos);
    }
    IEnumerator PikaMove(Vector3 pikaTargetPos)
    {
        pikaIsMoving = true;
        while ((pikaTargetPos - pikachu.transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            pikachu.transform.position = Vector3.MoveTowards(pikachu.transform.position, pikaTargetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        pikachu.transform.position = pikaTargetPos;
        pikaIsMoving = false;

    }
    private bool isWalkerble(Vector3 targetPos)
    {
        targetPos.z = targetPos.z - 0.5f;
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, solidObjectsLayer);
        if (hitCollider.Length > 0) {
            bumbSound.Play();
            return false;
        }
        return true;
    }
    private bool isSouthSlope(Vector3 targetPos)
    {
        targetPos.z = targetPos.z - 0.5f;
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, southSlope);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool isNorthSlope(Vector3 targetPos)
    {
        targetPos.z = targetPos.z - 0.5f;
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, northSlope);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool isWestSlope(Vector3 targetPos)
    {
        targetPos.z = targetPos.z - 0.5f;
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, westSlope);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool isEastSlope(Vector3 targetPos)
    {
        targetPos.z = targetPos.z - 0.5f;
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, eastSlope);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }


    private void CheckForEncounters(Vector3 targetPos)
    {
        targetPos.z = targetPos.z - 0.5f;
        Collider[] grassCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, longGrassLayer);
        if (grassCollider.Length > 0)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                Debug.Log("triggerBattle");
                pikaIsMoving = false;
                animator.SetBool("isMoving", false);
                onEncounter();
                








            }

        }

    }
    void SetFaceingDirection()
    {

        if (input.x == 1f)
        {
            playerFaceing = MoveDirection.Right;
        }
        if (input.x == -1f)
        {
            playerFaceing = MoveDirection.Left;
        }
        if (input.z == 1f)
        {
            playerFaceing = MoveDirection.Up;
        }
        if (input.z == -1f)
        {
            playerFaceing = MoveDirection.Down;
        }

    }
    void CheckRunning()
    {
        if (Input.GetKey(KeyCode.C)){
            moveSpeed = runSpeed;
            isRunning = true;

            
        }
        else
        {
            moveSpeed = walkSpeed;
            isRunning = false;
            
        }
        

    }
    void PlayPikaCry()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("pressed Z");
            Vector3 collideCheck = transform.parent.position;
            if (playerFaceing == MoveDirection.Up)
            {
                collideCheck.z += 1;
            }
            else if (playerFaceing == MoveDirection.Down)
            {
                collideCheck.z -= 1;
            }
            else if (playerFaceing == MoveDirection.Left)
            {
                collideCheck.x -= 1;
            }
            else if (playerFaceing == MoveDirection.Right)
            {
                collideCheck.x += 1;
            }

             
            
            Collider[] hitCollider = Physics.OverlapBox(collideCheck, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, pikachuLayer);
            if (hitCollider.Length > 0)
            {

                pikachu.GetComponent<AudioSource>().Stop();
                if(playerFaceing == MoveDirection.Up)
                {
                    pikaAnimator.SetFloat("moveY", -1);
                }
                else if (playerFaceing == MoveDirection.Down)
                {
                    pikaAnimator.SetFloat("moveY", 1);
                }
                else if (playerFaceing == MoveDirection.Left)
                {
                    pikaAnimator.SetFloat("moveY", 0);
                    pikaAnimator.SetFloat("moveX", 1);
                }
                else if (playerFaceing == MoveDirection.Right)
                {
                    pikaAnimator.SetFloat("moveY", 0);
                    pikaAnimator.SetFloat("moveX", -1);
                }
                pikachu.GetComponent<AudioSource>().Play();
                
            }

        }
    }
        
    
    /* void DirectionChange()
     {
         if (PreviousPlayerFaceing == MoveDirection.Down )
         {
             pikaTargetPos.z += -1;
         }
         if (PreviousPlayerFaceing == MoveDirection.Up)
         {
             pikaTargetPos.z += 1;
         }
         if (PreviousPlayerFaceing == MoveDirection.Left)
         {
             pikaTargetPos.x += -1;
         }
         if (PreviousPlayerFaceing == MoveDirection.Right)
         {
             pikaTargetPos.x += 1;
         }
         if (PreviousPlayerFaceing == MoveDirection.None)
         {
             pikaTargetPos.x += input.x;
             pikaTargetPos.z += input.z;
         }

     }*/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started)
        {
            Vector3 test = transform.parent.position;
            if (playerFaceing == MoveDirection.Up)
            {
                test.z += 1;
            }
            else if (playerFaceing == MoveDirection.Down)
            {
                test.z -= 1;
            }
            else if (playerFaceing == MoveDirection.Left)
            {
                test.x -= 1;
            }
            else if (playerFaceing == MoveDirection.Right)
            {
                test.x += 1;
            }
            
            test.z -= .5f;
            Gizmos.DrawWireCube(test, new Vector3(0.3f, 0.3f, 0.3f));
        }
    }
     public void OnEnable()
    {
        pikachu.GetComponent<AudioSource>().Stop();
    }


}
