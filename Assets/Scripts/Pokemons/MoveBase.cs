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
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuary;
    [SerializeField] int pp;
    [SerializeField] Category moveCategory;

    public string Name
    {
        get { return name; }
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
    public int PP
    {
        get { return pp; }
    }
    public Category MoveCategory
    {
        get { return moveCategory; }
    }

}

