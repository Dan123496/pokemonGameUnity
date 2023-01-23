using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition 
{
    public string Name { get; set; }
    public string Description { get; set; }

    public string StartMessage { get; set; }
    public ConditionID ID { get; set; }
    public Sprite Icon { get; set; }
    public Func<Pokemon, bool> OnBeforeMove { get; set; }
    public Action<Pokemon> OnStart { get; set; }
    public Action<Pokemon> OnAfterTurn { get; set; }
}


