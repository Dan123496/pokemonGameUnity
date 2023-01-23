using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Category{
    Physical = 1,
    Special =2,
    Status =3
}
[CreateAssetMenuAttribute(fileName = "Move", menuName = "Pokemon/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuary;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] Category moveCategory;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;
    public string Name
    {
        get { return moveName; }
    }
    public string Description
    {
        get { return description; }
    }
    public PokemonType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuary
    {
        get { return accuary; }
    }
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int PP
    {
        get { return pp; }
    }
    public int Priority
    {
        get { return priority; }
    }
    public Category MoveCategory
    {
        get { return moveCategory; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public List<SecondaryEffects> SecondaryEffects
    {
        get { return secondaryEffects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
}
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID staus;
    [SerializeField] ConditionID volatileStaus;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public ConditionID Staus
    {
        get { return staus; }
    }
    public ConditionID VolatileStaus
    {
        get { return volatileStaus; }
    }
}
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;
    public int Chance
    {
        get { return chance;  }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
}
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}
public enum MoveTarget
{
    foe, self
}
