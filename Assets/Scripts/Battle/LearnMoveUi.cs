using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnMoveUi : MonoBehaviour
{

    [SerializeField] List<MoveSlotUI> moveSlots;

    int currentSelection;
    [SerializeField] RectTransform arrow;
    [SerializeField] List<int> arrowPos;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip cursor;
    [SerializeField] AudioClip selected;
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        currentSelection = 0;
        moveSlots[0].InitData(newMove);

        for (int i = 1; i < moveSlots.Count; i++)
        {

            if (i <= currentMoves.Count)
            {
                moveSlots[i].InitData(currentMoves[i-1]);
            }
            else
            {
                moveSlots[i].InitData(null);
            }


        }

    }
    public void HandleSelection(Action<int> onSelected)
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSelection++;
            audioSource.PlayOneShot(cursor);


        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentSelection--;
            audioSource.PlayOneShot(cursor);
        }
        if(currentSelection > 4)
        {
            currentSelection = 0;
        }
        else if (currentSelection < 0)
        {
            currentSelection = 4;
        }
        Debug.Log(currentSelection);
        arrow.anchoredPosition = new Vector2(arrow.anchoredPosition.x, arrowPos[currentSelection]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            audioSource.PlayOneShot(selected);
            onSelected?.Invoke(currentSelection);
        }
    }
    private void OnEnable()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

}
