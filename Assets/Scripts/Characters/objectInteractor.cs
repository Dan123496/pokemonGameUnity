using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectInteractor : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    ObjectState state;
   
    public ObjectState GetState()
    {
        return state;
    }
    public void SetState(ObjectState newState)
    {
        state = newState;
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (state == ObjectState.Idle )
        {

            yield return DialogManager.Instance.ShowDialog(dialog);
            state = ObjectState.Idle;
        }
    }
}
public enum ObjectState { Idle, Dialog }
