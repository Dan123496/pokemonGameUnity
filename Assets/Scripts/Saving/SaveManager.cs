using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager saveGame;
    
    public Dictionary<int, bool> Trainners = new Dictionary<int, bool>();

    private void Awake()
    {
        if (saveGame == null)
        {
            saveGame = this;
        }
        else
        {
            Destroy(this);
        }
    }

}
