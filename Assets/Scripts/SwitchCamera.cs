using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] bool isPerspective;
    GameObject currentCamera;
    // Start is called before the first frame update
    void Start()
    {
        if (isPerspective)
        {
            currentCamera = GameObject.FindWithTag("MainCamera");
            currentCamera.GetComponent<Camera>().orthographic = false;
        }
        else if (!isPerspective)
        {
            currentCamera = GameObject.FindWithTag("MainCamera");
            currentCamera.GetComponent<Camera>().orthographic = true;
        }
        
    }

   
}
