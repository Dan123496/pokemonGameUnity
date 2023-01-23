using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;
    [SerializeField] bool Inside;
    [SerializeField] GameObject debugSpawner;
    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            if(debugSpawner == null)
            {
                debugSpawner = GameObject.FindGameObjectWithTag("DebugSpawner");
            }
             
            Instantiate(essentialObjectsPrefab, debugSpawner.transform.position, Quaternion.identity);
            if (Inside)
            {
                GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>().enabled = false;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>().enabled = true;
            }
        }
    }
}
