using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PikachuInteracter : MonoBehaviour, Interactable
{
    

    [SerializeField] Dialog dialog;
    [SerializeField] AudioSource pikaSound;
    [SerializeField] CharacterAnimator AnimScript;

    PikaState state;
   
    public PikaState GetState()
    {
        return state;
    }
    public void SetState(PikaState newState)
    {
        state = newState;
    }

    public void Awake()
    {
        state = PikaState.Idle;
        AnimScript = transform.GetChild(0).GetComponent<CharacterAnimator>();
    }
    public void Update()
    {
    }
    
    public IEnumerator Interact(Transform initiator)
    {
        if (state == PikaState.Idle)
        {
            LookTowards(initiator.position);
            state = PikaState.Dialog;
            pikaSound.Play();

            yield return DialogManager.Instance.ShowDialog(dialog);
            state = PikaState.Idle;
        }
    }
    public void LookTowards(Vector3 initiator)
    {
        var xDiff = Mathf.Floor(initiator.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(initiator.z) - Mathf.Floor(transform.position.z);

        if (xDiff == 0 || yDiff == 0)
        {
            AnimScript.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            AnimScript.MoveY = Mathf.Clamp(yDiff, -1f, 1f);

        }
        else
        {
            Debug.LogError("CHARECTER CANT TURN DIAGONLY");
        }
        Debug.Log(xDiff + "  = xDiff   " + yDiff + "  yDiff");
    }
}


public enum PikaState { Idle, Dialog }


