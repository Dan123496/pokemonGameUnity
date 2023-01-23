using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Item/Create new Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;
    [SerializeField] bool allMoves;


    [Header("Status Condition")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllSstatus;


    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    

    public override bool Use(Pokemon pokemon)
    {
        // revives and max revives 

        if (revive || maxRevive)
        {
            if (pokemon.HP > 0)
            {
                return false;
            }
            if (maxRevive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }
            else if (revive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp / 2);
            }
            pokemon.CureStatus();
            return true;
        }

        //if hp is 0, the pokemon is fainted and no items, other than revives, can be used 

        if(pokemon.HP <= 0)
        {
            return false;
        }

        //full restore
        if (restoreMaxHP && recoverAllSstatus)
        {
            if(pokemon.HP == pokemon.MaxHp && (pokemon.Status.ID == ConditionID.none || pokemon.Status == null) && pokemon.VolatileStatus.ID != ConditionID.con)
            {
                return false;
            }
            pokemon.IncreaseHP(pokemon.MaxHp);
            pokemon.CureStatus();
            pokemon.CureVolatileStatus();
            return true;
        }

        // hp retoring 

        if (hpAmount > 0 || restoreMaxHP)
        {
            if (pokemon.HP == pokemon.MaxHp)
            {
                return false;
            }
            if (restoreMaxHP)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }
            else
            {
                pokemon.IncreaseHP(hpAmount);
            }
            
            return true;
        }

        //  heal status conditions 

        if (status != ConditionID.none)
        {
            if (status == pokemon.Status.ID)
            {
                pokemon.CureStatus();

            }
            else if (status == pokemon.VolatileStatus.ID)
            {
                pokemon.CureVolatileStatus();
            }
            else
            {
                return false;
            }
            return true;
        }
        // full heall 
        if (recoverAllSstatus) 
        {
            if ((pokemon.Status == null || pokemon.Status.ID == ConditionID.none ) && (pokemon.VolatileStatus.ID != ConditionID.con))
            {
                return false;
            }
            pokemon.CureVolatileStatus();
            pokemon.CureStatus();
            return true;
        }

        // Restore PP 

        //Elixers 
        if (allMoves)
        {
            var count = 0;
            foreach(var move in pokemon.Moves)
            {
                if(move.PP == move.Base.PP)
                {
                    count++;
                } 
                
            }
            if(count == pokemon.Moves.Count)
            {
                return false;
            }
            if (restoreMaxPP)
            {
                pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
                return true;
            }
            else if (ppAmount > 0)
            {
                pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
                return true;
            }
            else
            {
                return false;
            }
        }
        /*Ethers 
         open moves menu
         
         */



        return false;
    }
}
