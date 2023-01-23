using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class doorAnim : MonoBehaviour
{
    Character characterScript;
    bool playerInside = false;
    AudioSource doorSound;
    PlayerController playerController;
    bool running = false;
    [SerializeField] bool sideDoor;
    
    private void Awake()
    {
        doorSound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponentInChildren<PlayerController>().InDoorTrigger(true);
            other.GetComponentInChildren<PlayerController>().DoorObject(this.gameObject);
            playerController = other.GetComponentInChildren<PlayerController>();
            Debug.Log(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponentInChildren<PlayerController>().InDoorTrigger(false);
            other.GetComponentInChildren<PlayerController>().DoorObject(null);
        }
    }
    public IEnumerator OpenDoor(Character characterScript)
    {
        this.characterScript = characterScript;
        if (!running)
        {
            if (sideDoor == true)
            {
                yield return OpenSideDoor(characterScript);
            }
            else
            {
                yield return OpenDoor1(characterScript);
            }
            
        }
        
    }
    public IEnumerator OpenDoor1(Character characterScript)
    {
        running = true;
        doorSound.Play();
        yield return this.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).WaitForCompletion();
        yield return characterScript.Move(new Vector3(0, 0, 2), 4, playerController.OnMoveOver, true);
        running = false;
    }
    public IEnumerator OpenSideDoor(Character characterScript)
    {
        running = true;
        doorSound.Play();
        yield return this.gameObject.transform.DOScaleZ(0, 0.5f).WaitForCompletion();
        yield return characterScript.Move(new Vector3(0, 0, 2), 4, playerController.OnMoveOver, true);
        running = false;
    }
    private void Update()
    {
        if (running)
        {
            characterScript.HandleUpdate();
        }
    }
}
