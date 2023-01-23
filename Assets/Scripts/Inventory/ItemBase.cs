using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    // Start is called before the first frame update
    [SerializeField] string itemName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] string usedMessage;

    public virtual string Name => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public string UsedMessage => usedMessage;

    public virtual bool canUseInBattle => true;

    public virtual bool IsReuseable => false;

    public virtual bool canUseOutOfBattle => true;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }


}
