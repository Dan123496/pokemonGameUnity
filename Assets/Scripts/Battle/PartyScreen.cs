using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public PartyMemberUI[] memberlots;
    List<Pokemon> pokemons;
    public AudioSource SEAudio;
    [SerializeField] int maxPartySlots;
    [SerializeField] Text messageText;
    [SerializeField] AudioClip cursor;
    PartyPokemon party;
   
    int currentMember;

    public Pokemon SelectedMember => pokemons[currentMember];

    public Text MessageText => messageText;

    public int CurrentMember => currentMember; 
    public void Initi()
    {
        currentMember = 0;
        SEAudio = GetComponent<AudioSource>();
        memberlots = GetComponentsInChildren<PartyMemberUI>(true);
        party = PartyPokemon.GetPlayerParty();
        SetPartyData();
        party.OnUpdated += SetPartyData;
        messageText.text = "Select a Pokemon";
    }
   
   public void SetPartyData()
    {
        currentMember = 0;
        pokemons = party.Pokemons;
        for(int i= 0; i< memberlots.Length; i++)
        {
            if(i < pokemons.Count)
            {
                memberlots[i].gameObject.SetActive(true);
                memberlots[i].InitData(pokemons[i]);
            }
            else
            {
                memberlots[i].gameObject.SetActive(false);
            }
            
            
        }
        getSelectedSprite();
    }
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevMember = currentMember;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ++currentMember;
            SEAudio.PlayOneShot(cursor);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            --currentMember;
            SEAudio.PlayOneShot(cursor);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentMember += 2;
            SEAudio.PlayOneShot(cursor);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentMember -= 2;
            SEAudio.PlayOneShot(cursor);
        }
        if (currentMember == pokemons.Count)
        {
            currentMember = 0;
        }
        if (currentMember == pokemons.Count + 1)
        {
            if (pokemons.Count % 2 == 0)
            {
                currentMember = 1;
            }
            else
            {
                currentMember = 0;
            }
        }
        if (currentMember == -1)
        {

            currentMember = pokemons.Count - 1;

        }
        if (currentMember == -2)
        {
            if (pokemons.Count % 2 == 0)
            {
                currentMember = pokemons.Count - 2;
            }
            else
            {
                currentMember = pokemons.Count - 1;
            }
        }
        if(prevMember != currentMember)
        {
            getSelectedSprite();
        }       
        if (Input.GetKeyDown(KeyCode.X) )
        {
            onBack?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
    }
    public void getSelectedSprite()
    {
        if (currentMember == 0)
        {
            Sprite selectSprit = Resources.Load<Sprite>("pokemonTabFirstSelected");

            var partyImage = memberlots[0].gameObject;
            partyImage.GetComponent<Image>().sprite = selectSprit;
        }
        else
        {
            Sprite selectSprit = Resources.Load<Sprite>("pokemonTabFirstUnselected") as Sprite;
            var partyImage = memberlots[0].gameObject;
            partyImage.GetComponent<Image>().sprite = selectSprit;

        }
        for (int i = 1; i < pokemons.Count; i++)
        {
            if (i == currentMember)
            {
                Sprite selectSprit = Resources.Load<Sprite>("pokemonTabSelected");
                memberlots[i].gameObject.GetComponent<Image>().sprite = selectSprit;
            }
            else
            {
                Sprite selectSprit = Resources.Load<Sprite>("pokemonTabUnselected") as Sprite;
                memberlots[i].gameObject.GetComponent<Image>().sprite = selectSprit;
            }
        }
    }
    public void ShowIfUseable(TMsHMsItem tm)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberlots[i].MessageText.gameObject.SetActive(true);
            string message = tm.CanBeTaught(pokemons[i]) ? "ABLE!" : "NOT ABLE";
            memberlots[i].SetMessage(message);
        }
    }
    public void ClearPartySlotMessage()
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberlots[i].SetMessage("");
            memberlots[i].MessageText.gameObject.SetActive(false);
        }
    }
}/*
var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0 || selectedMember == playerUnit.Pokemon)
            {
                errorSound.Play();
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {

                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
            prevState = null;

  

prevState = null;
            partyScreen.gameObject.SetActive(false);

            ActionSelection();
  
  */
