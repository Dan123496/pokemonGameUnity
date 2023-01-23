using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog 
{
    [TextArea(1, 2)]
    [SerializeField] List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }

    
}
