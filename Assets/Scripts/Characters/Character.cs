
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public AudioSource bumbSound;
    Vector3 positionChange;
    Vector3 prevPositionChange;
    Vector3 prevVect;
    bool onSouthSlope = false;
    bool onNorthSlope = false;
    bool onWestSlope = false;
    bool onEastSlope = false;
    [SerializeField] CharacterAnimator AnimScript;
    [SerializeField] CharacterAnimator followerAnimScript;
    [SerializeField] NpcController npcController;
    [SerializeField] GameObject follower;
    MoveDirection Faceing = MoveDirection.Down;
    MoveDirection prevFaceing;
    float delay = 0.1f;
    float remainingDelay;
    float xMove;
    float zMove;
    int increment;
    float remainingMovement;
    [SerializeField] bool moveOnSpot;

    public void PrevPositionChange(Vector3 newPos)
    {
        prevPositionChange = newPos;
    }

    private void Awake()
    {
        if(AnimScript == null)
        {
            AnimScript = transform.GetChild(0).GetComponent<CharacterAnimator>();
        }
        if (followerAnimScript == null)
        {
            if (follower != null)
            {
                followerAnimScript = follower.transform.GetChild(0).GetComponent<CharacterAnimator>();
            }       
        }
        
        if (followerAnimScript)
        {
            FollowerAnimator.IsMoving = true;
        }
        
        
        npcController = GetComponent<NpcController>();
    }
    public CharacterAnimator Animator
    {
        get => AnimScript;
    }
    public CharacterAnimator FollowerAnimator
    {
        get => followerAnimScript;
    }
    public bool IsMoving { get;  set; }
    public bool followerIsMoving { get; set; }


    // Start is called before the first frame update
    public IEnumerator Move(Vector3 moveVec, float moveSpeed=4, Action OnMoveOver=null, bool ignoreColl = false, bool ignoreSlopes = false, bool justJumped = false)
    {
        

        AnimScript.MoveX =Mathf.Clamp(moveVec.x, -1f,1f);
        AnimScript.MoveY = Mathf.Clamp(moveVec.z, -1f, 1f);
        xMove = 0;
        zMove = 0;
        if (moveVec.x != 0)
        {
            if (moveVec.x > 0)
            {
                remainingMovement = moveVec.x;
                increment = -1;
                xMove = 1;
            }
            else if(moveVec.x < 0)
            {
                remainingMovement = moveVec.x;
                increment = 1;
                xMove = -1;
            }
        }
        else if (moveVec.z != 0)
        {
            if (moveVec.z > 0)
            {
                remainingMovement = moveVec.z;
                increment = -1;
                zMove = 1;
            }
            else if (moveVec.z < 0)
            {
                remainingMovement = moveVec.z;
                increment = 1;
                zMove = -1;
            }
        }

        while (remainingMovement != 0)
        {
            if (IsMoving == true || followerIsMoving == true)
            {
                yield return null;
            }
            else
            {
               
                var targetPos = transform.position;
                var followerTargetPos = Vector3.zero;

                if (follower == null)
                {
                    followerTargetPos = Vector3.zero;
                }
                else
                {
                    followerTargetPos = follower.transform.position;
                }



                positionChange = Vector3.zero;
                positionChange.x += xMove;
                positionChange.z += zMove;
                var test = targetPos + positionChange;
                

                if(AnimScript.tag == "Player")
                {
                    if(ignoreColl == false)
                    {
                        if (!IsWalkerble(test))
                        {
                            yield break;
                        }  
                    }
                }
                else if(!IsWalkerble(test) && !ignoreColl)
                {
                    Debug.Log("Collider");
                    if (npcController)
                    {
                        npcController.SetState(NPCState.Blocked);
                    }
                    yield return new WaitUntil(() => IsWalkerble(test));
                }
                if (npcController)
                {
                    npcController.SetState(NPCState.Walking);
                }
                remainingMovement += increment;

                if (!ignoreSlopes)
                {
                    if (onSouthSlope)
                    {
                        if (zMove > 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onSouthSlope = false;
                        }
                        else if (zMove < 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onSouthSlope = false;
                        }
                    }
                    else if (onNorthSlope)
                    {
                        if (zMove > 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onNorthSlope = false;
                        }
                        else if (zMove < 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onNorthSlope = false;
                        }
                    }
                    else if (onWestSlope)
                    {
                        if (xMove > 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onWestSlope = false;
                        }
                        else if (xMove < 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onWestSlope = false;
                        }

                    }
                    else if (onEastSlope)
                    {
                        if (xMove > 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onEastSlope = false;
                        }
                        else if (xMove < 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onEastSlope = false;
                        }
                    }
                    if (isSouthSlope(test))
                    {
                        if (zMove > 0) positionChange += new Vector3(0, 0.5f, 0);
                        else if (zMove < 0) positionChange += new Vector3(0, -0.5f, 0);
                        onSouthSlope = true;
                    }
                    else if (isNorthSlope(test))
                    {
                        if (zMove > 0) positionChange += new Vector3(0, -0.5f, 0);
                        else if (zMove < 0) positionChange += new Vector3(0, 0.5f, 0);
                        onNorthSlope = true;
                    }
                    else if (isWestSlope(test))
                    {
                        if (xMove > 0) positionChange += new Vector3(0, 0.5f, 0);
                        else if (xMove < 0) positionChange += new Vector3(0, -0.5f, 0);
                        onWestSlope = true;
                    }
                    else if (isEastSlope(test))
                    {
                        if (xMove > 0) positionChange += new Vector3(0, -0.5f, 0);
                        else if (xMove < 0) positionChange += new Vector3(0, 0.5f, 0);
                        onEastSlope = true;
                    }
                }
                
                if (follower)
                {
                    followerAnimScript.MoveX = prevVect.x;
                    followerAnimScript.MoveY = prevVect.z;
                }
                targetPos += positionChange;
                if (prevPositionChange == Vector3.zero)
                {
                    prevPositionChange = Vector3.left;
                }
                if (follower)
                {
                    followerTargetPos += prevPositionChange;
                }



                StartCoroutine(MoveUnit(targetPos, moveSpeed, OnMoveOver));
                if (follower && !justJumped)
                {
                    StartCoroutine(MoveFollower(followerTargetPos, moveSpeed));
                }else if (follower && justJumped)
                {
                    Ledge ledge = GetComponentInChildren<PlayerController>().GetLedge();
                    StartCoroutine(ledge.JumpFolower(gameObject));
                }

                prevPositionChange = positionChange;
                prevVect = new Vector3(xMove, 0, zMove);
                

            }
        }
    }


    public void HandleUpdate()
    {
        if (AnimScript)
        {
            AnimScript.IsMoving = IsMoving;
            if(moveOnSpot && AnimScript.IsMoving == false)
            {
                AnimScript.IsMoving = true;
            }
        }
    }
    public void LookTowards(Vector3 targetPos)
    {
        var xDiff =Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.z) - Mathf.Floor(transform.position.z);

        if(xDiff == 0 || yDiff == 0)
        {
            AnimScript.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            AnimScript.MoveY= Mathf.Clamp(yDiff, -1f, 1f);
        }
        else
        {
            Debug.Log(targetPos + "  = targetPos    " + transform.position + "  transform.position");
            Debug.LogError("CHARECTER CANT TURN DIAGONLY");
        }
    }
    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        if (Physics.BoxCast(transform.position + dir, new Vector3(0.3f, 0.3f, 0.3f), dir, Quaternion.identity, diff.magnitude - 1, Gamelayers.i.SolidLayer | Gamelayers.i.InteractableLayer | Gamelayers.i.PlayerLayer))
        {
            return false;
        }
        return true;

    }
    IEnumerator MoveUnit(Vector3 targetPos,float moveSpeed=4, Action OnMoveOver=null)
    {
        IsMoving = true;
        AnimScript.IsMoving = IsMoving;
        while ((targetPos - transform.position).sqrMagnitude > 0.0001)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
      
        transform.position = targetPos;
        
        OnMoveOver?.Invoke();
        IsMoving = false;

    }
    IEnumerator MoveFollower(Vector3 followerTargetPos, float moveSpeed = 4)
    {
        followerIsMoving = true;
        while ((followerTargetPos - follower.transform.position).sqrMagnitude > 0.0001)
        {
            if (!followerIsMoving) { yield break; }
            follower.transform.position = Vector3.MoveTowards(follower.transform.position, followerTargetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        follower.transform.position = followerTargetPos;
        followerIsMoving=false;

    }
    private bool IsWalkerble(Vector3 targetPos)
    {
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.SolidLayer | Gamelayers.i.InteractableLayer | Gamelayers.i.PlayerLayer);
        if (hitCollider.Length > 0)
        {
            foreach (var collider in hitCollider)
            {
                if (collider.gameObject == follower)
                {
                    return true;
                }
            }
            if (bumbSound)
            {
                bumbSound.Play();
            }
           
            return false;
        }
        return true;
    }
    
    private bool isSouthSlope(Vector3 targetPos)
    {

        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.SouthSlopeLayer);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool isNorthSlope(Vector3 targetPos)
    {

        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.NorthSlopeLayer);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool isWestSlope(Vector3 targetPos)
    {

        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.WestSlopeLayer);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool isEastSlope(Vector3 targetPos)
    {
 
        Collider[] hitCollider = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.EastSlopeLayer);
        if (hitCollider.Length > 0)
        {
            return true;
        }
        return false;
    }
    public void SetNotOnSlope()
    {
        onEastSlope = false;
        onNorthSlope = false;
        onSouthSlope = false;
        onWestSlope = false;
    }

    

}
