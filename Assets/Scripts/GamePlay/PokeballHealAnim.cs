using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokeballHealAnim : MonoBehaviour
{
    [SerializeField] AudioClip ballSound;
    [SerializeField] AudioClip HealSound;
    [SerializeField] List<GameObject> pokeballs;
    SpriteRenderer pokeballSprite;
    [SerializeField] Color flashColor = new Color(255, 255, 255); 

    private void Awake()
    {
        
        if (pokeballs.Count == 0)
        {
            pokeballs = GetComponentsInChildren<GameObject>().ToList();
        }
    }
    public IEnumerator PokeballHeal(NurseController nurseScipt)
    {
        var partyPokemon = GameController.Instance.PlayerControllerScript.GetComponent<PartyPokemon>().Pokemons;
        Debug.Log(partyPokemon.Count);
        AudioSource audioSource = nurseScipt.GetComponent<AudioSource>();
        audioSource.clip = ballSound;
        for (int i = 0; i < partyPokemon.Count; i++)
        {
            pokeballs[i].SetActive(true);
            
            audioSource.Play();
            yield return new WaitForSeconds(0.5f);


        }
        audioSource.clip = HealSound;
        audioSource.Play();
        for (int i = 0; i < partyPokemon.Count; i++)
        {
            pokeballSprite = pokeballs[i].transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
            Debug.Log(pokeballSprite.name);
            var sequence = DOTween.Sequence();
            sequence.Append(pokeballSprite.DOFade(0f, 0.3f));
            sequence.Append(pokeballSprite.DOFade(1, 0.3f));
            sequence.Append(pokeballSprite.DOFade(0f, 0.3f));
            sequence.Append(pokeballSprite.DOFade(1, 0.3f));
            sequence.Append(pokeballSprite.DOFade(0f, 0.3f));
            sequence.Append(pokeballSprite.DOFade(1, 0.3f));
            sequence.Append(pokeballSprite.DOFade(0f, 0.3f));
            sequence.Append(pokeballSprite.DOFade(1, 0.3f));
            

        }
        yield return new WaitForSeconds(2.5f);
        for(int i = 0; i < partyPokemon.Count ; i++)
        {
            pokeballs[i].SetActive(false);
        }


    }


}
