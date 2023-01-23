using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB : MonoBehaviour
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();
        var moveArray = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveArray)
        {
            Debug.Log(move.name);
            if (moves.ContainsKey(move.Name))
            {
                Debug.LogError($"There are two Moves with the name {move.Name}");
                continue;
            }
            moves[move.Name] = move; 
        }
    }
    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"pokemon with name {name} not found");
            return null;
        }
        return moves[name];
    }
}
