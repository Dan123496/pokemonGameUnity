using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int xDir = 0;
    [SerializeField] int zDir = 0;

    PlayerController playerController;
    bool InTrigger = false;

    public bool TryToJump(PlayerController player, Vector3 moveDir)
    {
        if (moveDir.x == xDir && moveDir.z == zDir)
        {
            player.canMove = false;
            StartCoroutine(Jump(player));
            return true;
        }
        return false;
    }
    IEnumerator Jump(PlayerController player)
    {
        player.canMove = false;
        var jumpDist = player.transform.position + new Vector3(xDir, 0, zDir) * 2;
        yield return player.transform.parent.transform.gameObject.DOJump(jumpDist, 0.3f, 1.5f, 0.5f).WaitForCompletion();
        player.canMove = true;
    }
    public IEnumerator JumpFolower(GameObject follower)
    {
        
        var jumpDist = follower.transform.position + new Vector3(xDir, 0, zDir) * 2;
        yield return follower.DOJump(jumpDist, 0.3f, 1.5f, 0.5f).WaitForCompletion();
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            InTrigger = true;
            playerController = collider.GetComponentInChildren<PlayerController>();
            playerController.SetLedge(this);
            playerController.InLedgeTrigger(true);
        }
        
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            InTrigger = false;
            playerController.InLedgeTrigger(false);
            playerController = null;
        }
            
    }
}
