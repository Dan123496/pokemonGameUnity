using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class turnOfOutdoorLighting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance.CurrentScene.IsIndoor)
        {
            this.GetComponent<Light>().enabled = false;
        }
        else
        {
            this.GetComponent<Light>().enabled = true;
        }
    }
    
}
