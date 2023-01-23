using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PC : MonoBehaviour
{
    // Start is called before the first frame update
     [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
    }
    public void Start()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }
    
    public void AddPokemon(Pokemon newPokemon)
    {
        pokemons.Add(newPokemon);
    }
}
 