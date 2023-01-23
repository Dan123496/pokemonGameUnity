using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    // Start is called before the first frame update
    [SerializeField] int TrainerID;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] Dialog defeatedDialog;
    [SerializeField] GameObject icon;
    [SerializeField] GameObject fov;
    [SerializeField] Sprite sprite;
    [SerializeField] string trainerName;
    [SerializeField] AudioClip battleTheme;
    [SerializeField] AudioClip victoryTheme;
    [SerializeField] AudioSource encounterTheme;
    [SerializeField] Character character;
    [SerializeField] bool defeated;
    public void Awake()
    {
        character = GetComponent<Character>();

        encounterTheme = fov.GetComponent<AudioSource>();
        
    }
    public void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameControllor");
        SaveManager saveList = obj.GetComponent<SaveManager>();
        if (saveList != null)
        {
            Debug.Log("yes");
            bool value;
            if (saveList.Trainners.TryGetValue(TrainerID, out value))
            {
                defeated = value;
            }
        }
        Debug.Log("defeted:  " + defeated);
        if (defeated)
        {
            fov.gameObject.SetActive(false);
        }
        SetFovRotation(character.Animator.FacingDirection);
        
    }
    public void Update()
    {
        character.HandleUpdate();
    }
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        
        

        icon.SetActive(true);
        if(icon.GetComponent<AudioSource>() != null)
        {
            var SE = icon.GetComponent<AudioSource>();
            SE.Play();
        }
        BackgroundMusic.i.BackgroundMusicSource().Stop();
        encounterTheme.Play();

        yield return new WaitForSeconds(1f);
        icon.SetActive(false);

        
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        

        moveVec = new Vector3(Mathf.Round(moveVec.x), moveVec.y, Mathf.Round(moveVec.z));
        Debug.Log(moveVec);
        yield return character.Move(moveVec, 4, null, true, false);


        yield return DialogManager.Instance.ShowDialog(dialog);
        encounterTheme.Stop();
        GameController.Instance.StartTrainerBattle(battleTheme, victoryTheme, this);

      
    }
    public void BattleLost()
    {
        fov.gameObject.SetActive(false);
        defeated = true;
        GameObject obj = GameObject.FindGameObjectWithTag("GameControllor");
        SaveManager saveList = obj.GetComponent<SaveManager>();
        if (saveList != null)
        {
            if (!saveList.Trainners.ContainsKey(TrainerID))
            {
                saveList.Trainners.Add(TrainerID, true);
                Debug.Log("added key and value");
            }
            else
            {
                saveList.Trainners[TrainerID] = true;
                Debug.Log("changed value");
            }
        }

    }
    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        if (defeated == false)
        {
            BackgroundMusic.i.BackgroundMusicSource().Stop();
            encounterTheme.Play();
            yield return DialogManager.Instance.ShowDialog(dialog);
            encounterTheme.Stop();
            GameController.Instance.StartTrainerBattle(battleTheme, victoryTheme, this);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
    }
    public void SetFovRotation(MoveDirection dir)
    {
        float angle = 0f;
        if(dir == MoveDirection.Right)
        {
            angle = 270f;
        }
        else if (dir == MoveDirection.Up)
        {
            angle = 180;
        }
        else if (dir == MoveDirection.Left)
        {
            angle = 90f;
        }
        fov.transform.eulerAngles = new Vector3(0, angle, 0);
        Debug.Log(angle);
    }

    public object CaptureState()
    {
        Debug.Log(defeated);
        return this.defeated;
    }

    public void RestoreState(object state)
    {
        this.defeated = (bool)state;
        Debug.Log(defeated);
        if (this.defeated)
        {
            fov.SetActive(false);
        }
        else
        {
            fov.SetActive(true);
        }
    }

    public string TrainerName
    {
        get => trainerName;
    }
    public AudioClip VictoryTheme
    {
        get => victoryTheme;
    }
    public Sprite Sprite
    {
        get => sprite;
    }
    public Dialog DefeatedDialog
    {
        get => defeatedDialog;
    }

}
