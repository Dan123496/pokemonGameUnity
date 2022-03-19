using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public GameObject sun;
    float secondsPerDay;
    float secondsPerHour;
    float secondsPerMin = 60;
    public float timeMuitipler =1;
    float seconds;
    float mins=0;
    float hour =0;
    float timer;
    float angleTimer;
    [Range (0,24)]
    public float timeOfDay = 12;
    float sTimer;
    // Start is called before the first frame update
    void Start()
    {
        secondsPerHour = secondsPerMin * 60;
        secondsPerDay = secondsPerHour * 24;
        hour = (int)timeOfDay;

    }

    // Update is called once per frame
    void Update()
    {
        SunUpdate();
        float incriment = Mathf.Abs((Time.deltaTime / secondsPerDay) * timeMuitipler);
        
        
        timeOfDay += incriment;
        


        if (timeOfDay >= 24)
        {
            timeOfDay = 0;
        }

        sTimer += Time.deltaTime;
        timer += Time.deltaTime;
        if (sTimer >= 1 * timeMuitipler)
        {
            sTimer = 0;
            seconds +=1;
            Debug.Log(timeOfDay);
        }
        if(timer >= 60 * timeMuitipler)
        {
            mins += 1;
            timer = 0;
            seconds = 0;
        }
        if (mins >= 60)
        {
            
            mins = 0;

        }
        
        


    }
    void SunUpdate()
    {
        sun.transform.localRotation = Quaternion.Euler(((timeOfDay / 24) * 360f) - 90, 70, 0);
    }
}
