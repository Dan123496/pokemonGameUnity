using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonCounter : MonoBehaviour
{
    [SerializeField] GameObject[] pokeBalls;
    // Start is called before the first frame update
    

    public void SetPokeBalls(PartyPokemon party)
    {
        for(int i = 0; i < party.Pokemons.Count; i++)
        {
            pokeBalls[i].SetActive(true);
            if(party.Pokemons[i].HP <= 0)
            {
                pokeBalls[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                pokeBalls[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    public void ResetPokeBalls()
    {
        for (int i = 0; i < pokeBalls.Length; i++)
        {
            pokeBalls[i].transform.GetChild(0).gameObject.SetActive(true);
            pokeBalls[i].SetActive(false);
        }
    }
}
