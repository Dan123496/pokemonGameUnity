using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NurseController : MonoBehaviour, Interactable
{
    

    [SerializeField] Dialog dialog;
    [SerializeField] Dialog endingDialog;
    [SerializeField] List<Vector3> movePattern;
    [SerializeField] float timeBetwenPattern;
    [SerializeField] bool moveOnSpot;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip HealSound;
    [SerializeField] bool inPokemonCenter;
    [SerializeField] Image blackCanvas;


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

    public void Start()
    {

        
        if (GetComponent<Character>() != null)
        {
            character = GetComponent<Character>();
        }
        var transitionObj = GameObject.FindGameObjectWithTag("SceneTransition");
        Debug.Log(transitionObj);
        blackCanvas = transitionObj.GetComponent<Image>();

    }
    public void Update()
    {

        if (movePattern.Count != 0)
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
        if (character)
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
            PokemonCenterHeal();
           
        }
    }

    public void PokemonCenterHeal()
    {
        GameController.Instance.SetSate(GameState.cutscene);
        Debug.Log("here");
        var player = GameController.Instance.PlayerControllerScript;
        var party = player.GetComponentInChildren<PartyPokemon>();
        Debug.Log(party);
        party.HealParty();
        if (inPokemonCenter)
        {
            StartCoroutine(HealAnim());
        }
        else
        {
            StartCoroutine(OutsideHealAnim());
        }
        

        
    }
    IEnumerator HealAnim()
    {
        character.Animator.MoveX = -1;
        yield return new WaitForSeconds(0.2f);
        var a = GetComponentInChildren<PokeballHealAnim>();
        Debug.Log(a);
        yield return StartCoroutine(a.PokeballHeal(this));
        state = NPCState.Idle;
        character.Animator.MoveX = 0;
        character.Animator.MoveY = -1;
        state = NPCState.Dialog;
        yield return DialogManager.Instance.ShowDialog(endingDialog);
        GameController.Instance.SetSate(GameState.OverWorld);
        state = NPCState.Idle;

    }
    IEnumerator OutsideHealAnim()
    {
        yield return new WaitForSeconds(0.2f);
        blackCanvas.enabled = true;
        var old = audioSource.clip;
        audioSource.clip = HealSound;
        audioSource.Play();
        var sequence = DOTween.Sequence();
        sequence.Append(blackCanvas.DOFade(255f, 0.2f));
        sequence.AppendInterval(1.5f);
        yield return sequence.Append(blackCanvas.DOFade(0f, 0.2f)).WaitForCompletion();
        state = NPCState.Dialog;
        yield return DialogManager.Instance.ShowDialog(endingDialog);
        GameController.Instance.SetSate(GameState.OverWorld);
        state = NPCState.Idle;
        audioSource.clip = old;
    }
  
}

