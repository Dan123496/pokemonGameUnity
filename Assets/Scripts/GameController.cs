using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { OverWorld, Battle, Busy}
public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] GameObject worldZoneTriggers;
    [SerializeField] Camera playerCam;
    public AudioSource battleTheme;
    public Image transition1;

    bool start = false;
    bool done1 = false;
    bool done2 = false;

    

    GameState state;

    public void Start()
    {
        
        playerController.onEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }
    void StartBattle()
    {
        if (!battleTheme.isPlaying)
        {
            battleTheme.Play();
        }
        worldZoneTriggers.SetActive(false);
        state = GameState.Busy;
       



    }
    void EndBattle(bool won)
    {
        if (battleTheme.isPlaying)
        {
            battleTheme.Stop();
        }
        worldZoneTriggers.SetActive(true);
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
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Busy)
        {
            Transition1();
        }
       

    }
    public void Transition1()
    {

        if (!battleTheme.isPlaying)
        {
            battleTheme.Play();
        }
        
        if ((transition1.fillAmount + Mathf.Epsilon) < 1f)
        {
            
            
            transition1.fillAmount += 0.002f;
            
            
        }
        else
        {
            transition1.fillAmount = 1f;

            battleSystem.gameObject.SetActive(true);
            playerCam.gameObject.SetActive(false);
            transition1.fillAmount = 0f;
            state = GameState.Battle;

            var playerParty = playerController.GetComponent<PartyPokemon>();
            var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomPokemon();
            Debug.Log(playerParty);
            Debug.Log(wildPokemon);
            battleSystem.StartBattle(playerParty, wildPokemon);



        }
        Debug.Log(state);

    }
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
