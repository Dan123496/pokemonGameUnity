using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] string nickname;
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        nickname = pBase.Name;
        Init();
    }
    public PokemonBase Base
    {
        get
        {
            return _base;
        }
    }
    public string Nickname
    {
        get
        {
            return nickname;
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
    public int Exp { get; set; }

    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }


    public event System.Action OnHpChanged;
    public event System.Action OnStatusChanged;

    public void Init()
    {
        Debug.Log(_base.Name);
        
        Moves = new List<Move>();

        foreach (var move in Base.LernableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if(Moves.Count >= PokemonBase.MaxMoves)
                break;
            
        }
        Exp = Base.ExpForLevel(Level);
        CalculateStats();
        HP = MaxHp;
        StatusChanges = new Queue<string>();
        ClearStatBoosts();
        SetStatus(ConditionID.none);
        SetVolatileStatus(ConditionID.none);
    }
    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetPokemonByName(saveData.baseName);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;
        if(saveData.statusID != null)
        {
            Status = ConditionsDB.Conditions[saveData.statusID.Value];
        }
        else
        {
            Status = null;
        }

        Moves = saveData.moves.Select(m => new Move(m)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ClearStatBoosts();
        SetVolatileStatus(ConditionID.none);
        
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            baseName = Base.Name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusID = Status?.ID,
            moves = Moves.Select(m => m.GetSaveData()).ToList()

        };
        return saveData;
    }

    void ClearStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defence, 0 },
            {Stat.SpAttack, 0 },
            {Stat.SpDefence, 0 },
            {Stat.Speed, 0 },
            {Stat.Accuracy,0 },
            {Stat.Evasion,0 },
        };
    }
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * 2 * Level) / 100f) + 5);
        Stats.Add(Stat.Defence, Mathf.FloorToInt((Base.Defence * 2 * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * 2 * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefence, Mathf.FloorToInt((Base.SpDefence * 2 * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * 2 * Level) / 100f) + 5);


        MaxHp = Mathf.FloorToInt((Base.MaxHp * 2 * Level) / 100f) + Level + 10;

    }
    
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostVals = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost  >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostVals[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostVals[-boost]);
        }
        return statVal;
    }
    public PokemonBase.LearnableMove GetLearnableMoveAtLevel()
    {
       return Base.LernableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if(Moves.Count > PokemonBase.MaxMoves)
        {
            return;
        }
        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;

    }
    

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if(boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name} {stat} rose!");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            }

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }
   public bool CheckForLevelUp()
    {
        if(Exp > Base.ExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defence
    {
        get { return GetStat(Stat.Defence); }
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefence
    {
        get { return GetStat(Stat.SpDefence); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int MaxHp { get; private set; }

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

        DecreaseHP(damage);
        return damageDetails;
    }
    public int ConfusionDamage()
    {
       float attackStat = (float)this.Attack;
       float defenceStat = this.Defence;
       float modifiers = Random.Range(0.85f, 1f);
       float a = ((2 * this.Level) / 5) + 2;
       float d = a * 40 + (attackStat / defenceStat);
       float f = (d / 50) + 2;
       int damage = Mathf.FloorToInt(f * modifiers );
        Debug.Log(attackStat);
        return damage;

    }
    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHpChanged?.Invoke();
    }
    public void IncreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP + damage, 0, MaxHp);
        OnHpChanged?.Invoke();
    }
    public void SetStatus(ConditionID conditionId)
    {
        if(Status == null)
        {
            Status = ConditionsDB.Conditions[conditionId];
            OnStatusChanged?.Invoke();
        }
        else if(Status.Name == "none" || conditionId == ConditionID.none)
        {
            Status = ConditionsDB.Conditions[conditionId];
            Status.OnStart?.Invoke(this);
            if (Status.StartMessage != "")
            {
                StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
            }
            OnStatusChanged?.Invoke();
        }
        else
        {
            StatusChanges.Enqueue($"{Base.Name} already has the status  {Status.Name} ");
        }
        
    }  
    public void CureStatus()
    {
        SetStatus(ConditionID.none);
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus == null)
        {
            VolatileStatus = ConditionsDB.Conditions[conditionId];
        }
        else if (VolatileStatus.Name == "none" || conditionId == ConditionID.none)
        {
            VolatileStatus = ConditionsDB.Conditions[conditionId];
            VolatileStatus.OnStart?.Invoke(this);
            if (VolatileStatus.StartMessage != "")
            {
                StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
            }
        }
        else
        {
            StatusChanges.Enqueue($"{Base.Name} already has the status  {VolatileStatus.Name} ");
        }

    }
    public void CureVolatileStatus()
    {
        SetVolatileStatus(ConditionID.none);
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];

    }
    public bool OnBeforeMove()
    {
        bool canPreformMove = true;
        if (Status.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPreformMove = false;
            }
        }
        if (VolatileStatus.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPreformMove = false;
            }
        }

        return canPreformMove;
    }
    public void OnAfterTurn()
    {
        Status.OnAfterTurn?.Invoke(this);
        VolatileStatus.OnAfterTurn?.Invoke(this);
    }
    public void OnBattleOver()
    {
        SetVolatileStatus(ConditionID.none);
        ClearStatBoosts();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Crit { get; set; }
    public float TypeEffect { get; set; }

}

[System.Serializable]
public class PokemonSaveData
{
    public string baseName;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusID;
    public List<MoveSaveData> moves;

} 
