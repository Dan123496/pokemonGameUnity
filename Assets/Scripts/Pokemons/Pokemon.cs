using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public PokemonBase Base
    {
        get
        {
            return _base;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }

    public void Init()
    {
       
        HP = MaxHp;
        Moves = new List<Move>();
        foreach (var move in Base.LernableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if(Moves.Count >= 4)
                break;
            
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * 2 * Level) / 100f) + 5; }
    }
    public int Defence
    {
        get { return Mathf.FloorToInt((Base.Defence * 2 * Level) / 100f) + 5; }
    }
    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * 2 * Level) / 100f) + 5; }
    }
    public int SpDefence
    {
        get { return Mathf.FloorToInt((Base.SpDefence * 2 * Level) / 100f) + 5; }
    }
    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * 2 * Level) / 100f) + 5; }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * 2 * Level) / 100f)+ Level  + 10; }
    }
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float crit = 1f;
        if (Random.value *100f <= 6.25f)
        {
            crit = 1.5f;
        }
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        
        float stab = 1;
        if (move.Base.Type == attacker.Base.Type1 || move.Base.Type == attacker.Base.Type2)
        {
            stab = 1.5f;
           
            Debug.Log("stab");
        }
        var damageDetails = new DamageDetails()
        {
            TypeEffect = type,
            Crit = crit,
            Fainted = false

        };
        float attackStat =1;
        float defenceStat = 1;
        if (move.Base.MoveCategory == Category.Physical)
        {
            attackStat = (float)attacker.Attack;
            defenceStat = Defence;
            
        }
        else if (move.Base.MoveCategory == Category.Special)
        {
            attackStat = (float)attacker.SpAttack;
            defenceStat = SpDefence;
            
        }
        Debug.Log(attackStat + " taken in stat");
        Debug.Log(MaxHp +" hp");

        float modifiers = Random.Range(0.85f, 1f) * type * crit;
        float a = ((2 * attacker.Level) / 5) + 2;
        float d = a * move.Base.Power + (attackStat / defenceStat) ;
        float f = (d / 50) + 2; 
        int damage = Mathf.FloorToInt(f * modifiers * stab);
        
        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
             damageDetails.Fainted = true;
        }
        return damageDetails;
    }
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];

    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Crit { get; set; }
    public float TypeEffect { get; set; }

}
