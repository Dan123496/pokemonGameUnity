using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum MoveDirection
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    None = 4
}
public class PlayerController : MonoBehaviour, ISavable
{
    // Start is called before the first frame update
    
     float moveSpeed =4;
    public float walkSpeed =4;
    public float runSpeed =8;
    private bool isMoving;
    private Vector3 input;
    private Vector3 prevInput;
    public GameObject pikachu;
    public GameObject playerSprite;
    public GameObject pikaSprite;
    MoveDirection stairsDirection;
    GameObject doorObject;
    public void DoorObject(GameObject door)
    {
        doorObject = door;
    }
    bool inDoorTrigger = false;
    bool inStairsTrigger = false;
    bool inLedgeTrigger = false;
    bool justJumped = false;
    Ledge ledge;
    public void InDoorTrigger(bool inTrigger)
    {
        inDoorTrigger = inTrigger;
    }
    public void Jumped(bool jumped)
    {
        justJumped = jumped;
    }
    public void InLedgeTrigger(bool inledge)
    {
        inLedgeTrigger = inledge;
    }
    public void SetLedge(Ledge ledgeScript)
    {
        ledge = ledgeScript;
    }
    public Ledge GetLedge()
    {
        return ledge;
    }
    public void InStairsTrigger(bool inTrigger, MoveDirection stairsDir)
    {
        inStairsTrigger = inTrigger;
        stairsDirection = stairsDir;
    }
    public AudioSource bumbSound;
    private AudioSource wildBattleTheme;
    public AudioSource GetWildBattleTheme()
    {
        return wildBattleTheme;

    }
    public void SetWildBattleThemeClip(AudioClip clip)
    {
        clip = wildBattleTheme.clip;
    }

    public GameObject world;
    public GameObject battle;
    float delay = 0.1f;
    float remainingDelay;
    bool incTimer = false;
    UnityEngine.Object collidedObject;

    bool pikaIsMoving;
    bool isRunning;
    bool m_Started;
    [SerializeField] Character character;
    [SerializeField] Sprite[] sprites;
    [SerializeField] string trainerName;
    public bool canMove { get; set; } = true;
    public string TrainerName
    {
        get => trainerName;
    }
    public Sprite[] Sprites
    {
        get => sprites;
    }

    public event Action onEncounter;
    public event Action<Collider[]> onEnterTrainnerView;

    Vector3 positionChange;
    Vector3 prevPositionChange;

    PokemonBase wildPokemon;

    MoveDirection playerFaceing = MoveDirection.Down;
    MoveDirection prevPlayerFaceing;

    public MoveDirection GetPlayerFaceing()
    {
        return playerFaceing;
    }
    private void Awake()
    {
        //animator = playerSprite.GetComponent<Animator>();
        //pikaAnimator = pikaSprite.GetComponent<Animator>();
        
        character = GetComponentInParent<Character>();
        wildBattleTheme = GetComponent<AudioSource>();
    }
    void Start()
    {
        
        m_Started = true;
        pikachu.GetComponent<AudioSource>().Stop();
        character.FollowerAnimator.IsMoving = true;
    }
    

    public Vector3 GetMovementdiraction  (MoveDirection moveDir)
    {
        if(playerFaceing == MoveDirection.Down)
        {
            return new Vector3(0, 0, -1);
        }
        else if(playerFaceing == MoveDirection.Left)
        {
            return new Vector3(-1, 0, 0);
        }
        else if (playerFaceing == MoveDirection.Right)
        {
            return new Vector3(1, 0, 0);
        }
        else
        {
            return new Vector3(0, 0, 1);
        }
       

    }

    // Update is called once per frame
    public void  HandleUpdate()
    {

       

        //if (!isMoving && !pikaIsMoving)
        if(!character.IsMoving && !character.followerIsMoving && canMove)
        {
          
            CheckRunning();


            input.x = Input.GetAxisRaw("Horizontal");
            input.z = Input.GetAxisRaw("Vertical");
            

            if (input.z != 0)
            {
                input.x = 0;
            }
            if (prevPositionChange == Vector3.zero)
            {
                prevPositionChange = Vector3.left;
            }

            if (input != Vector3.zero)
            {

                
                prevPlayerFaceing = playerFaceing;
                SetFaceingDirection();
                //animator.SetFloat("moveX", input.x);
                //animator.SetFloat("moveY", input.z);
                character.Animator.MoveX =input.x;
                character.Animator.MoveY = input.z;
                if (playerFaceing != prevPlayerFaceing)
                {
                    remainingDelay = delay;
                }
                if (remainingDelay > 0f)
                {
                    remainingDelay -= Time.deltaTime;
                    return;
                }
                if (ledge != null && inLedgeTrigger)
                {
                    ledge.TryToJump(this, input);
                }
                if(doorObject != null && inDoorTrigger == true && playerFaceing == MoveDirection.Up && canMove)
                {
                    Debug.Log(doorObject.name);
                    Debug.Log(doorObject.GetComponent<doorAnim>());
                    StartCoroutine(doorObject.GetComponent<doorAnim>().OpenDoor(character));
                }
                /*else if (inDoorTrigger == true && playerFaceing == stairsDirection)
                {
                    StartCoroutine(doorObject.GetComponent<StairsAnim>().TriggerStairs(character));
                }*/
                else if(canMove)
                {
                    StartCoroutine(character.Move(input, moveSpeed, OnMoveOver));
                }
                
                /*
                //animator.SetFloat("moveX", input.x);
                //animator.SetFloat("moveY", input.z);
                PlayerAnimScript.MoveX = input.x;
                PlayerAnimScript.MoveY = input.z;
                var targetPos = transform.parent.position;
                var pikaTargetPos = pikachu.transform.position;
                positionChange = Vector3.zero;
                positionChange.x += input.x;
                positionChange.z += input.z;
                var test = targetPos + positionChange;
                if (isWalkerble(test))
                {
                    if (onSouthSlope)
                    {
                        if (input.z > 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onSouthSlope = false;
                        }
                        else if (input.z < 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onSouthSlope = false;
                        }
                    }
                    else if (onNorthSlope)
                    {
                        if (input.z > 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onNorthSlope = false;
                        }
                        else if (input.z < 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onNorthSlope = false;
                        }
                    }
                    else if (onWestSlope)
                    {
                        if (input.x > 0) {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onWestSlope = false;
                        }
                        else if (input.x < 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onWestSlope = false;
                        }

                    }
                    else if (onEastSlope)
                    {
                        if (input.x > 0)
                        {
                            positionChange += new Vector3(0, -0.5f, 0);
                            onEastSlope = false;
                        }
                        else if (input.x < 0)
                        {
                            positionChange += new Vector3(0, 0.5f, 0);
                            onEastSlope = false;
                        }
                    }
                    else if (isSouthSlope(test))
                    {
                        if (input.z > 0) positionChange += new Vector3(0, 0.5f, 0);
                        else if (input.z < 0) positionChange += new Vector3(0, -0.5f, 0);
                        onSouthSlope = true;
                    }
                    else if (isNorthSlope(test))
                    {
                        if (input.z > 0) positionChange += new Vector3(0, -0.5f, 0);
                        else if (input.z < 0) positionChange += new Vector3(0, 0.5f, 0);
                        onNorthSlope = true;
                    }
                    else if (isWestSlope(test))
                    {
                        if (input.x > 0) positionChange += new Vector3(0, 0.5f, 0);
                        else if (input.x < 0) positionChange += new Vector3(0, -0.5f, 0);
                        onWestSlope = true;
                    }
                    else if (isEastSlope(test))
                    {
                        if (input.x > 0) positionChange += new Vector3(0, -0.5f, 0);
                        else if (input.x < 0) positionChange += new Vector3(0, 0.5f, 0);
                        onEastSlope = true;
                    }

                    //pikaAnimator.SetFloat("moveX", prevInput.x);
                    //pikaAnimator.SetFloat("moveY", prevInput.z);
                    PikaAnimScript.MoveX = prevInput.x;
                    PikaAnimScript.MoveY = prevInput.z;
                   

                    targetPos += positionChange;
                    pikaTargetPos += prevPositionChange;

                    
                    StartCoroutine(Move(targetPos));
                    StartCoroutine(PikaMove(pikaTargetPos));
                    prevPositionChange = positionChange;
                    prevInput = input;
               


                } */

            }
            //animator.SetBool("isRunning", isRunning);
            //animator.SetBool("isMoving", isMoving);
            character.HandleUpdate();
            character.Animator.IsRunning = isRunning;
            //PlayerAnimScript.IsMoving = isMoving;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(Interact());
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                character.FollowerAnimator.MoveX = character.FollowerAnimator.MoveX * -1;
            }

        }
    }
    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX,0, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        var interactCollider = Physics.OverlapBox(interactPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.InteractableLayer| Gamelayers.i.PikachuLayer);
        if (interactCollider.Length > 0)
        {
            /*for(int i=0; i<interactCollider.Length; i++)
            {
                interactCollider[i].GetComponent<Interactable>()?.Interact();
            }*/
            foreach(Collider col in interactCollider)
            {
                yield return col.GetComponent<Interactable>()?.Interact(transform);

            }
        }

    }
    public void OnMoveOver()
    {
        CheckForEncounters();
        var colliders = Physics.OverlapBox(transform.parent.position, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.TriggerableLayers);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }   
    }
    public void CheckForEncounters()
    {

        var check = transform.parent.position;
        Collider[] grassCollider = Physics.OverlapBox(check, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, Gamelayers.i.LongGrassLayer);
        if (grassCollider.Length > 0)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                Debug.Log("triggerBattle");
                
                character.Animator.IsMoving = false;
                onEncounter.Invoke();
            }
        }
    }
    void SetFaceingDirection()
    {

        if (input.x == 1f)
        {
            playerFaceing = MoveDirection.Right;
        }
        if (input.x == -1f)
        {
            playerFaceing = MoveDirection.Left;
        }
        if (input.z == 1f)
        {
            playerFaceing = MoveDirection.Up;
        }
        if (input.z == -1f)
        {
            playerFaceing = MoveDirection.Down;
        }

    }
    public void Restart()
    {
        character.FollowerAnimator.IsMoving = true;
    }
    void CheckRunning()
    {
        if (Input.GetKey(KeyCode.C)){
            moveSpeed = runSpeed;
            isRunning = true;  
        } 
        else
        {
            moveSpeed = walkSpeed;
            isRunning = false;      
        } 
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started)
        {
            Vector3 test = transform.parent.position;
            if (playerFaceing == MoveDirection.Up)
            {
                test.z += 1;
            }
            else if (playerFaceing == MoveDirection.Down)
            {
                test.z -= 1;
            }
            else if (playerFaceing == MoveDirection.Left)
            {
                test.x -= 1;
            }
            else if (playerFaceing == MoveDirection.Right)
            {
                test.x += 1;
            }

            
            Gizmos.DrawWireCube(test, new Vector3(0.3f, 0.3f, 0.3f));
        }
       
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.parent.transform.position.x, transform.parent.transform.position.y, transform.parent.transform.position.z },
            pokemons = GetComponent<PartyPokemon>().Pokemons.Select(p => p.GetSaveData()).ToList()
        };
        Debug.Log(saveData);
        return saveData;
    }

    public void RestoreState(object state)
    {
        var SaveData = (PlayerSaveData)state;
        var pos = SaveData.position;
        transform.parent.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        pikachu.transform.position = new Vector3(pos[0] +1, pos[1], pos[2]);
        character.PrevPositionChange(Vector3.left);

        GetComponent<PartyPokemon>().Pokemons = SaveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PokemonSaveData> pokemons;
}