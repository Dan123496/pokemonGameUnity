using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

public class PartyPokemon : MonoBehaviour
{
    // Start is called before the first frame update
     [SerializeField] List<Pokemon> pokemons;

    public event Action OnUpdated;
    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
        set
        {
            pokemons = value;
            OnUpdated?.Invoke();

        }
    }
    public void Awake()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }
    public Pokemon GetHealthyPokemon()
    {
       return  pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }
    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
            OnUpdated?.Invoke();
           
        }
        else
        {
            var pc = GetComponent<PC>();
            pc.AddPokemon(newPokemon);
                
        }
    }
    public void HealParty()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.CureStatus();
            pokemon.HP = pokemon.MaxHp;
            foreach (var move in pokemon.Moves)
            {
                move.PP = move.Base.PP;
            }
            OnUpdated?.Invoke();


        }
    }
    public static PartyPokemon GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PartyPokemon>();
    }
    public void SwitchPartyOrder(int firstPokemonIndex, int secondPokemonIndex)
    {
        if(Pokemons != null)
        {
            if (firstPokemonIndex >= 0 && firstPokemonIndex < Pokemons.Count)
            {
                if (secondPokemonIndex >= 0 && secondPokemonIndex < Pokemons.Count)
                {
                    Pokemon temp = Pokemons[firstPokemonIndex];
                    Pokemons[firstPokemonIndex] = Pokemons[secondPokemonIndex];
                    Pokemons[secondPokemonIndex] = temp;
                    OnUpdated?.Invoke();
                    Debug.Log("moved  " + Pokemons[secondPokemonIndex].Base.name + " to position " + secondPokemonIndex);
                    Debug.Log("moved  " + Pokemons[firstPokemonIndex].Base.name + " to position " + firstPokemonIndex);
                    return;
                }
            }
        }
        Debug.Log("error");



    }
}
 