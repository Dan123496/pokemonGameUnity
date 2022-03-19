using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayZoneMusic : MonoBehaviour
{
    public GameObject theZone;
    public AudioSource zoneTheme;
 
    int count;

    // Start is called before the first frame update
    void Start()
    {

        zoneTheme.Stop();



    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            zoneTheme.Play();
            Debug.Log("Triggered " + theZone.name);
        }
           
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {

            zoneTheme.Stop();
        }
    }
    private void OnEnable()
    {
        Debug.Log(zoneTheme);
        zoneTheme.Stop();

    }
}
