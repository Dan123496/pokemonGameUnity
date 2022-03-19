using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.U2D.Sprites;


[CreateAssetMenuAttribute(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{

    [SerializeField] float dexNumber;
    [SerializeField]string name;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite[] frontSprite;
    [SerializeField] Sprite[] backSprite;
 
    

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] List<LernableMove> lernableMoves;


    public float DexNumber
    {
        get { return dexNumber; }
    }
    public string Name
    {
        get { return name; }
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

    [System.Serializable]
    public class LernableMove
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
    

    public List<LernableMove> LernableMoves
    {
        get { return lernableMoves; }
    }



    

   




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
