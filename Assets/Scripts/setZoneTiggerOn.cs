using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class setZoneTiggerOn : MonoBehaviour
{
    public GameObject route1;
    public GameObject palletTown;
    

    // Start is called before the first frame update
    void Start()
    {
        palletTown.GetComponent<BoxCollider>().enabled = true;
        route1.GetComponent<BoxCollider>().enabled = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //sets the camera behind the player. offsets the camera by adding to the players position
    }
}
