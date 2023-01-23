using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { OverWorld, Battle, Dialog, WildBattleTransition, PartyScreen, Bag, TrainerBattleTransition, cutscene, busy, Menu }
public class GameController : MonoBehaviour, ISavable
{
    // Start is called before the first frame update
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] GameObject worldZoneTriggers;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Camera playerCam;
    [SerializeField] AudioSource BattleTheme;
    [SerializeField] AudioSource backgroundMusicSource;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] AudioClip menuOpen;
    [SerializeField] AudioClip select;

    TrainerController tranier;
    MenuControllor menuControllor;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }

    public GameObject WorldZoneTriggers
    {
        get => worldZoneTriggers;
    }

    

   public PlayerController PlayerControllerScript
    {
        get => playerController;
    }
    public PartyScreen Partyscreen => partyScreen;

    public Image transition1;

    GameState state;
    Pokemon wildPokemon;

    bool pokemonSelected;
    int pokemonIndexOne;
    int pokemonIndexTwo;

    public static GameController Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
        menuControllor = GetComponent<MenuControllor>();
        PokemonDB.Init();
        MoveDB.Init();
        pokemonSelected = false;


    }

    public void Start()
    {
        BattleTheme = playerController.GetComponent<AudioSource>();
        playerController.onEncounter += () =>
        {
            StartBattle();
        };
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Initi();

        DialogManager.Instance.OnShowDialog += () =>
         {
             state = GameState.Dialog;
         };
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialog)
            {
                state = GameState.OverWorld;
            }
        };
        menuControllor.onBack += () =>
        {
            state = GameState.OverWorld;
        };
        menuControllor.onMenuSelected += OnMenuSelected;
    }
    public void StartBattle()
    {
        wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomPokemon();
        BattleTheme.clip = CurrentScene.GetComponent<MapArea>().BattleMusic;
        battleSystem.VictoryMusic = CurrentScene.GetComponent<MapArea>().VictoryMusic;
        if (!BattleTheme.isPlaying)
        {
            BattleTheme.Play();
        }
        backgroundMusicSource.Stop();

        
        state = GameState.WildBattleTransition;
    }
    public void StartTrainerBattle(AudioClip battleMusic, AudioClip victoryMusic, TrainerController tranier )
    {
        battleSystem.VictoryMusic = victoryMusic;
        BattleTheme.clip = battleMusic;
        this.tranier = tranier;
        if (!BattleTheme.isPlaying)
        {
            BattleTheme.Play();
        }
        backgroundMusicSource.Stop();
        
        state = GameState.TrainerBattleTransition;
    }
    void EndBattle(bool won)
    {
        if (BattleTheme.isPlaying)
        {
            BattleTheme.Stop();
        }
        BattleTheme.clip = null;
        if(tranier != null && won == true)
        {
            tranier.BattleLost();
        }
        tranier = null;

        
        backgroundMusicSource.Play();
        state = GameState.OverWorld;
        battleSystem.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
        



    }
    // Update is called once per frame
    void Update()
    {
        if (state == GameState.OverWorld)
        {
            playerController.HandleUpdate();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                BattleTheme.PlayOneShot(menuOpen);
                menuControllor.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.WildBattleTransition)
        {
            WildBattleTransition1();
        }
        else if (state == GameState.TrainerBattleTransition)
        {
            TrainerBattleTransition1();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuControllor.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                BattleTheme.PlayOneShot(select);
                if (!pokemonSelected)
                {
                    pokemonIndexOne = partyScreen.CurrentMember;
                    Debug.Log(pokemonIndexOne);
                    partyScreen.MessageText.text = "select second Pokemon";
                    Debug.Log(partyScreen.MessageText.text);
                    pokemonSelected = true;
                }
                else if (pokemonSelected)
                {
                    pokemonIndexTwo = partyScreen.CurrentMember;
                    Debug.Log(pokemonIndexOne);
                    if (pokemonIndexOne != pokemonIndexTwo)
                    {
                        PartyPokemon.GetPlayerParty().SwitchPartyOrder(pokemonIndexOne, pokemonIndexTwo);
                        partyScreen.MessageText.text = "Pokemon switched";
                    }
                    else
                    {
                        partyScreen.MessageText.text = "Select a Pokemon";
                    }
                    pokemonSelected = false;
                    pokemonIndexOne = -1;
                    pokemonIndexTwo = -1;


                }
                Debug.Log(pokemonSelected);
            };
            Action onBack = () =>
            {
                BattleTheme.PlayOneShot(menuOpen);
                menuControllor.OpenMenu();
                partyScreen.gameObject.SetActive(false);
                state = GameState.Menu;
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                BattleTheme.PlayOneShot(menuOpen);
                menuControllor.OpenMenu();
                inventoryUI.gameObject.SetActive(false);
                state = GameState.Menu;
            };
            inventoryUI.HandleUpdate(onBack);
        }



    }
    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }
    public void WildBattleTransition1()
    {
        BattleTransition1();
        if (state == GameState.Battle)
        {
            var playerParty = playerController.GetComponent<PartyPokemon>();
            Debug.Log(CurrentScene);
            
            var wildPokmonClone = new Pokemon(wildPokemon.Base, wildPokemon.Level);
            battleSystem.StartBattle(playerParty, wildPokmonClone);
            wildPokemon = null;
        }
        
        
    }
    public void TrainerBattleTransition1()
    {
        BattleTransition1();
        if (state == GameState.Battle)
        {
            var playerParty = playerController.GetComponent<PartyPokemon>();
            var trainerParty = tranier.GetComponent<PartyPokemon>();

            battleSystem.StartTrainerBattle(playerParty, trainerParty);
        }

    }
    public void BattleTransition1()
    {
        if ((transition1.fillAmount + Mathf.Epsilon) < 1f)
        {
            transition1.fillAmount += Time.deltaTime;

        }
        else
        {
            transition1.fillAmount = 1f;

            battleSystem.gameObject.SetActive(true);
            playerCam.gameObject.SetActive(false);
            transition1.fillAmount = 0f;
            state = GameState.Battle;
        }
        
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        state = GameState.cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }
    public void SetSate(GameState newState) 
    {
        state = newState;
    }
    public GameState GetSate()
    {
        return state;
    }

    public object CaptureState()
    {
        int sceneindex = 0;
        bool gamePlayOpen = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == "GamePlay")
            {
                sceneindex = SceneManager.GetSceneByName("GamePlay").buildIndex;
                gamePlayOpen = true;
            }
        }
        if (!gamePlayOpen)
        {
            sceneindex = SceneManager.GetSceneAt(0).buildIndex;
        }
        return sceneindex;


    }

    public void RestoreState(object state)
    {
        Debug.Log("hi");
        var scene = (int)state;
        StartCoroutine(LoadScene(scene));
        
    }
    IEnumerator LoadScene(int  scene)
    {
        SceneManager.LoadSceneAsync(scene);
        while (!SceneManager.GetSceneByBuildIndex(scene).isLoaded)
        {
            yield return null;
        }
        var sun = GameObject.FindWithTag("Sun").GetComponent<DayNightCycle>();
        var lightArray = GameObject.FindGameObjectsWithTag("InDoorLighting");
        if (lightArray.Length <= 0)
        {
            sun.GetComponent<Light>().enabled = true;
        }
        else
        {
            sun.GetComponent<Light>().enabled = false;
        }
        partyScreen.SetPartyData();

    }
    void OnMenuSelected(int selectedItem)
    {
        
        if (selectedItem == 0)
        {
            Debug.Log("pokemon ");
            partyScreen.gameObject.SetActive(true);
            partyScreen.SetPartyData();
            partyScreen.MessageText.text = "Select a Pokemon";
            partyScreen.getSelectedSprite();
            pokemonSelected = false;
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            SavingSystem.i.Save("saveSlot1");
            state = GameState.OverWorld;
        }
        else if (selectedItem == 3)
        {
            SavingSystem.i.Load("saveSlot1");
            Debug.Log(PlayerPrefs.GetInt("savedScene"));
            state = GameState.OverWorld;
        }
        

    }


    public GameState State => state;


    /*public void Transition2()
    {
        
        if (transition2.gameObject.activeSelf == false)
        {
            transition2.gameObject.SetActive(true);
            transition2.fillAmount = 1f;
           
        }
       
        if ((transition2.fillAmount - Mathf.Epsilon) > 0)
        {
            transition2.fillAmount -= 0.003f;

        }
        else
        {
            transition2.fillAmount = 0f;
            
            done1 = false;
            state = GameState.Battle;


        }
        

    }*/





}
