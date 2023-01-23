using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.none,
            new Condition()
            {
                Name = "none",
                StartMessage = "",
                Icon  = null,
                ID = ConditionID.none,
            }
        },
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisond!",
                Icon  = Resources.Load<Sprite>("Pois"),
                ID = ConditionID.psn,
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    var theName =pokemon.Base.Name;
                    
                    if(pokemon.MaxHp/8 < 1f)
                    {
                        pokemon.DecreaseHP(1);
                    }
                    else
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp/8);
                    }
                    pokemon.StatusChanges.Enqueue($"{theName} was hurt by poison! ");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been Burned!",
                Icon  = Resources.Load<Sprite>("Burned"),
                ID = ConditionID.brn,
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    var theName =pokemon.Base.Name;
                    if(pokemon.MaxHp/16< 1f)
                    {
                        pokemon.DecreaseHP(1);
                    }
                    else
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp/16);
                    }
                    
                    pokemon.StatusChanges.Enqueue($"{theName} was hurt by burns! ");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralysis",
                StartMessage = "has been Paralyzed!",
                Icon  = Resources.Load<Sprite>("Para"),
                ID = ConditionID.par,
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    var theName =pokemon.Base.Name;
                    if(Random.Range(1,5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{theName} is fully Paralyzed! ");
                        return false;
                        
                    }
                   return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been Frozen!",
                Icon  = Resources.Load<Sprite>("Frozen"),
                ID = ConditionID.frz,
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    var theName =pokemon.Base.Name;
                    if(Random.Range(1,6) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{theName} thawed out!");
                        return true;

                    }
                    pokemon.StatusChanges.Enqueue($"{theName} is Frozen!");
                   return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "fell Asleep!",
                Icon  = Resources.Load<Sprite>("Sleep"),
                ID = ConditionID.slp,
                OnStart = (Pokemon pokemon) => 
                {

                    pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    var theName =pokemon.Base.Name;
                    
                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{theName} woke up!");
                        return true;
                    }
                    else
                    {
                        pokemon.StatusTime--;
                        pokemon.StatusChanges.Enqueue($"{theName} is fast Asleep!");
                        return false;
                    }
                   
                }
            }
        },
        {
            ConditionID.con,
            new Condition()
            {

                Name = "Confusion",
                StartMessage = "has benn Confused!",
                Icon  = Resources.Load<Sprite>("Sleep"),
                ID = ConditionID.con,
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = Random.Range(2,5);
                    Debug.Log($"Will be Confused for {pokemon.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    var theName =pokemon.Base.Name;
                    

                    if(pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        
                        pokemon.StatusChanges.Enqueue($"{theName} Shuck off its Confusion!");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    if(Random.Range(1,3) ==1)
                    {
                     return true;
                    }
                    
                    pokemon.StatusChanges.Enqueue($"{theName} is confused");
                    int damageCal = pokemon.ConfusionDamage();
                    pokemon.DecreaseHP(damageCal);
                    pokemon.StatusChanges.Enqueue($"it hurt itself in confusion");
                    return false;
                }
            }
        },
        {
            ConditionID.ftn,
            new Condition()
            {
                Name = "Fainted",
                StartMessage = "",
                Icon  = Resources.Load<Sprite>("Faint"),
                ID = ConditionID.ftn,
            }
        }

    };
    public static float GetStatusBounus(Condition condition)
    {
        float stausBonus = 1f;
        if(condition == null )
        {
            stausBonus = 1f;
        }
        else if(condition.ID == ConditionID.frz || condition.ID == ConditionID.slp)
        {
            stausBonus = 2.5f;
        }
        else if (condition.ID == ConditionID.par || condition.ID == ConditionID.psn || condition.ID == ConditionID.brn)
        {
            stausBonus = 1.5f;
        }
        Debug.Log(stausBonus + " bonus");
        return stausBonus;
    }
}
public enum ConditionID
{
    none,psn,brn,slp,par,frz,ftn,con
}
