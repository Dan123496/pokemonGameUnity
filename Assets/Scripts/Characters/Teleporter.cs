using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Vector3 spawnLocation;
    [SerializeField] bool Upstairs;
    [SerializeField] bool left;
    bool running;
    
    PlayerController playerScript;
    Character characterScript;
    public void OnPlayerTriggered(PlayerController player)
    {
        if(GameController.Instance.GetSate() == GameState.OverWorld)
        {
            playerScript = player;
            characterScript = player.GetComponentInParent<Character>();
            teleport();

        }
        
    }
    void teleport()
    {
        Debug.Log("hi");
        GameController.Instance.SetSate(GameState.busy);
        characterScript.followerIsMoving = false;
        characterScript.SetNotOnSlope();
        characterScript.Animator.MoveX = characterScript.Animator.MoveX * -1;
        
        Debug.Log(characterScript.FollowerAnimator.MoveX);
        playerScript.transform.parent.transform.position = spawnLocation;
        int prevMove = -1;
        if (left)
        {
            prevMove = 1;
        }
        characterScript.PrevPositionChange(new Vector3(prevMove, 0, 0));
        int offset = 1;
        if (left)
        {
            offset = -1;
        }
        var pika = GameObject.FindGameObjectWithTag("pikachu");
        pika.transform.position = (spawnLocation + new Vector3(offset, 0,0));
       

        StartCoroutine(StairAnim(characterScript, pika));  
    }
    IEnumerator StairAnim(Character characterScript, GameObject pikachu)
    {
        running = true;
        var a = -1;
        var dir = MoveDirection.Left;
        if (left) { a = 1; dir = MoveDirection.Right; }
        pikachu.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        yield return characterScript.Move(new Vector3(a, 0, 0 ), 4, null, true);
        pikachu.GetComponentInChildren<CharacterAnimator>().SetFacingDirection(dir);
        if (Upstairs)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
        pikachu.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        yield return characterScript.Move(new Vector3(a, 0, 0), 4, null, true);
        GameController.Instance.SetSate(GameState.OverWorld);
        running = false;
    }




    void Update()
    {
        if (running)
        {
            characterScript.HandleUpdate();
        }
      
    }
}
