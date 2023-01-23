using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =  "Item/Create new Pokeball item")]
public class PokeballItem : ItemBase
{
    [SerializeField] GameObject pokeballPrefab;
    [SerializeField] float catchRate;
    [SerializeField] bool AlwaysSucced;
    [SerializeField] bool fristTurn;
    [SerializeField] bool timer;
    [SerializeField] bool Repeat;

    public GameObject PokeballPrefab => pokeballPrefab;

    public override bool canUseOutOfBattle => false;
    public override bool Use(Pokemon pokemon)
    {    
        return true;      
    }
    public float CalculateCatchRate(float turnCount)
    {
        float finalCatchRate = catchRate;
        if (AlwaysSucced)
        {
            return 255;
        }
        else if(fristTurn)
        {
            if(turnCount > 1)
            {
                return 1;
            }
        }
        else if (timer)
        {
            if(turnCount >= 10)
            {
                return 4;
            }
            else
            {
                finalCatchRate = Mathf.Clamp(1 + turnCount * (1229 / 4096) , 1 , 4);
                Debug.Log(finalCatchRate);
                return finalCatchRate;
            }
        }
        return finalCatchRate;
    }
}
