using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Create New TM or HM")]
public class TMsHMsItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string Name => base.Name + $": {move.Name}" ;

    public override bool canUseInBattle => false;

    public MoveBase Move => move;
    public bool IsHM => isHM;

    public override bool IsReuseable => isHM;
    public override bool Use(Pokemon pokemon)
    {
        return pokemon.HasMove(move);
    }
    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItem.Contains(move);
    }
}
