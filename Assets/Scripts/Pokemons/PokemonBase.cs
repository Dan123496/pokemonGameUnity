using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.U2D.Sprites;


[CreateAssetMenuAttribute(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{

    [SerializeField] float dexNumber;
    [SerializeField]string pName;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite[] frontSprite;
    [SerializeField] Sprite[] backSprite;
    [SerializeField] Sprite icon;


    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] int catchRate=255;

    [SerializeField] GrowthRate growth;

    [SerializeField] AudioClip cry;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItem;

    [SerializeField] public static int MaxMoves { get; set; } = 4;

    public int ExpForLevel(int level)
    {
        
        if (growth == GrowthRate.fast)
        {
            return 4 *(level* level * level)/5;
        }
        else if (growth == GrowthRate.mediumFast)
        {
            return (level * level * level);
        }
        else if (growth == GrowthRate.mediumSlow)
        {
            return ((6 / 5) * (level * level * level)) - (15 * (level * level)) + (100 * level) - 140;
        }
        else if (growth == GrowthRate.slow)
        {
            return (5*(level*level*level))/4;
        }
        else
        {
            return -1;
        }
    }
    public float DexNumber
    {
        get { return dexNumber; }
    }
    public string Name
    {
        get { return pName; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite[] FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite[] BackSprite
    {
        get { return backSprite; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public PokemonType Type1
    {
        get { return type1; }
    }
    public PokemonType Type2
    {
        get { return type2; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defence
    {
        get { return defence; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }
    public int SpDefence
    {
        get { return spDefence; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public int CatchRate
    {
        get { return catchRate; }
    }
    public int ExpYield
    {
        get { return expYield; }
    }
    public AudioClip Cry
    {
        get { return cry; }
    }
    public GrowthRate Growth
    {
        get { return growth; }
    }

    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] MoveBase moveBase;
        [SerializeField] int level;

        public MoveBase Base
        {
            get { return moveBase; }
        }
        public int Level
        {
            get { return level; }
        }
    }
    

    public List<LearnableMove> LernableMoves
    {
        get { return learnableMoves; }
    }

    public List<MoveBase> LearnableByItem => learnableByItem;

}
public enum Stat
{
    Attack,
    Defence,
    SpAttack,
    SpDefence,
    Speed,

    //accuracy Modifiers, not stats
    Accuracy,
    Evasion
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poision,
    Ground,
    Flying,
    Phychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}
public enum GrowthRate
{
    eratic, 
    fast,
    mediumFast, 
    mediumSlow, 
    slow, 
    fluctating
}
public class TypeChart
{
    static float[][] chart = 
    {          
        
                                                                              //DEFENDING
                 //           NOR   FIR     WAT     ELE     GRA     ICE     FIG     POI     GRO     FLY     PHY     BUG     ROC     GHO     DRA     DAR     STE     FAI
        /*NOR*/ new float[] { 1f,   1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     0.5f,   0f,     1f,     1f,     0.5f,   1f },

        /*FIR*/ new float[] { 1f,   0.5f,   0.5f,   1f,     2f,     2f,     1f,     1f,     1f,     1f,     1f,     2f,     0.5f,   1f,     0.5f,   1f,     2f,     1f },

        /*WAT*/ new float[] { 1f,   2f,     0.5f,   1f,     0.5f,   1f,     1f,     1f,     2f,     1f,     1f,     1f,     2f,     1f,     0.5f,   1f,     1f,     1f },

       /*ELE*/  new float[] { 1f,   1f,     2f,     0.5f,   0.5f,   2f,     1f,     1f,     0f,     2f,     1f,     1f,     1f,     1f,     0.5f,   1f,     1f,     1f},

        /*GRA*/ new float[] { 1f,   0.5f,   2f,     1f,     0.5f,   1f,     1f,     0.5f,   2f,     0.5f,   1f,     0.5f,   2f,     1f,     0.5f,   1f,     0.5f,   1f},

        /*ICE*/ new float[] { 1f,   0.5f,   0.5f,   1f,     2f,     0.5f,   1f,     1f,     2f,     2f,     1f,     1f,     1f,     1f,     2f,     1f,     0.5f,   1f },

        /*FIG*/ new float[] { 2f,   1f,     1f,     1f,     1f,     2f,     1f,     0.5f,   1f,     0.5f,   0.5f,   0.5f,   2f,     0f,     1f,     2f,     2f,     0.5f},

        /*POI*/ new float[] { 1f,   1f,     1f,     1f,     2f,     1f,     1f,     0.5f,   0.5f,   1f,     1f,     1f,     0.5f,   0.5f,   1f,     1f,     0f,     2f},
//ATTACKING
        /*GRO*/ new float[] { 1f,   2f,     1f,     2f,     0.5f,   1f,     1f,     2f,     1f,     0f,     1f,     0.5f,   2f,     1f,     1f,     1f,     2f,     1f},

        /*FLY*/ new float[] { 1f,   1f,     1f,     0.5f,   2f,     1f,     2f,     1f,     1f,     1f,     1f,     2f,     0.5f,   1f,     1f,     1f,     0.5f,   1f },

        /*PHY*/ new float[] { 1f,   1f,     1f,     1f,     1f,     1f,     2f,     2f,     1f,     1f,     0.5f,   1f,     1f,     1f,     1f,     0f,     0.5f,   1f},

       /*BUG*/  new float[] { 1f,   0.5f,   1f,     1f,     2f,     1f,     0.5f,   0.5f,   1f,     0.5f,   2f,     1f,     1f,     0.5f,   1f,     2f,     0.5f,   0.5f },

        /*ROC*/ new float[] { 1f,   2f,     1f,     1f,     1f,     2f,     0.5f,   1f,     0.5f,   2f,     1f,     2f,     1f,     1f,     1f,     1f,     0.5f,   1f},

        /*GHO*/ new float[] { 0f,   1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     2f,     1f,     1f,     2f,     1f,     0.5f,   0.5f,   1f },

        /*DRA*/ new float[] { 1f,   1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     1f,     2f,     1f,     0.5f,   0f },

        /*DAR*/ new float[] { 1f,   1f,     1f,     1f,     1f,     1f,     0.5f,   1f,     1f,     1f,     2f,     1f,     1f,     2f,     1f,     0.5f,   0.5f,   0.5f },

        /*STE*/ new float[] { 1f,   0.5f,   0.5f,   1f,   1f,     2f,     1f,     1f,     1f,     1f,     1f,     1f,     2f,     1f,     1f,     1f,     0.5f,   2f },
               
        /*FAI*/ new float[] { 1f,   0.5f,   0f,     1f,   1f,     2f,     1f,     1f,     1f,     1f,     1f,     1f,     2f,     1f,     1f,     1f,     0.5f,   1f}


    };
    public static float GetEffectiveness(PokemonType attackType, PokemonType defendingType)
    {
       if (attackType == PokemonType.None || defendingType == PokemonType.None)
        {
            return 1f;
        }
        int row = (int)attackType - 1;
        int col = (int)defendingType - 1;



        return chart[row][col];
    }

}


