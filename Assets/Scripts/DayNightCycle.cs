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
    public float timeOfDay;
    float sTimer;
    public bool outSide;
    // Start is called before the first frame update
    void Start()
    {
        sun = gameObject;
        if(timeOfDay == 0)
        {
            timeOfDay = 8;
        }
        secondsPerHour = secondsPerMin * 60;
        secondsPerDay = secondsPerHour * 24;
        hour = (int)timeOfDay;
        /*
        var Suns = GameObject.FindGameObjectsWithTag("Sun");
        foreach(var sun in Suns)
        {
            if(sun != this.gameObject)
            {
                this.gameObject.SetActive(false);
            }
        }*/
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
        if (outSide)
        {
            sun.transform.localRotation = Quaternion.Euler(((timeOfDay / 24) * 360f) - 90, 70, 0);
        }
        
    }
    public void SetTimeOfDay(float newTime)
    {
        timeOfDay = newTime;
    }
    public float TimeOfDay
    {
        get { return timeOfDay; }
    }
}
