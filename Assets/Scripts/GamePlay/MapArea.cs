using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;
    [SerializeField] AudioClip battleMusic;
    [SerializeField] AudioClip victoryMusic;
    // Start is called before the first frame update
    public Pokemon GetRandomPokemon()
    {

        var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        
        wildPokemon.Init();
        return wildPokemon;

    }
    public AudioClip BattleMusic => battleMusic;
    public AudioClip VictoryMusic => victoryMusic;

}
