using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector3> movePattern;
    [SerializeField] float timeBetwenPattern;
    [SerializeField] bool moveOnSpot;


    NPCState state;
    float idleTimer;
    int currentPattern;
   
    Character character;
    public NPCState GetState()
    {
        return state;
    }
    public void SetState(NPCState newState)
    {
        state = newState;
    }

    public void Awake()
    {
        
        
            if(GetComponent<Character>() != null)
            {
                character = GetComponent<Character>();
            }
        
    }
    public void Update()
    {
       
        if(movePattern.Count != 0)
        {
            if (state == NPCState.Idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > timeBetwenPattern)
                {
                    idleTimer = 0f;
                    if (movePattern.Count > 0)
                    {
                        StartCoroutine(Walk());
                    }
                }
            }
        }
        if(character)
        {
            character.HandleUpdate();
        }  
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;
        yield return character.Move(movePattern[currentPattern]);
        currentPattern = (currentPattern + 1) % movePattern.Count;
        state = NPCState.Idle;
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle || state == NPCState.Blocked)
        {
            state = NPCState.Dialog;
            if (character)
            {
                character.LookTowards(initiator.position);
                Debug.Log("look");
            }

            yield return DialogManager.Instance.ShowDialog(dialog);
            idleTimer = 0f;
            state = NPCState.Idle;
        }  
    }
}
public enum NPCState { Idle, Walking, Dialog, Blocked }
