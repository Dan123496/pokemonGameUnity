using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsAnim : MonoBehaviour
{
    // Start is called before the first frame update
    Character characterScript;
    bool playerInside = false;
    AudioSource doorSound;
    PlayerController playerController;
    [SerializeField] MoveDirection stairsdirection;
    [SerializeField] bool down;
    bool running = false;
    private void Awake()
    {
        doorSound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponentInChildren<PlayerController>().InStairsTrigger(true, stairsdirection);
            playerController = other.GetComponentInChildren<PlayerController>();
            Debug.Log(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponentInChildren<PlayerController>().InDoorTrigger(false);
        }
    }
    public IEnumerator TriggerStairs(Character characterScript)
    {
        this.characterScript = characterScript;
        if (!running)
        {
            yield return TriggerStairs2(characterScript);
        }

    }
    public IEnumerator TriggerStairs2(Character characterScript)
    {
        running = true;
        
        yield return characterScript.Move(new Vector3(0, 0,2), 4, null, true);
       
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
