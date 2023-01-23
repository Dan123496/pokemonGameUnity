using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Vector3 spawnLocation;
    [SerializeField] Image SceneTransition;
    [SerializeField] string sceneName;
    DayNightCycle sun;
    float timeOfDay;
    [SerializeField] bool isExit;
    [SerializeField] AudioClip exitClip;
    AudioSource portalAudio;
    
    void Awake()
    {
        portalAudio = GetComponent<AudioSource>();
    }
    public void OnPlayerTriggered(PlayerController player)
    {
        SceneTransition = GameObject.FindWithTag("SceneTransition").GetComponent<Image>();
        sun = GameObject.FindWithTag("Sun").GetComponent<DayNightCycle>();
        if(sun != null)
        {
            timeOfDay = sun.TimeOfDay;
        }
        
       
        StartCoroutine(SwitchScene(player));
    }
    IEnumerator SwitchScene(PlayerController player)
    {
        
        DontDestroyOnLoad(gameObject);
        if(portalAudio != null && exitClip != null && !portalAudio.isPlaying)
        {
            portalAudio.clip = exitClip;
            portalAudio.Play();
        }

        GameController.Instance.SetSate(GameState.busy);
        SceneTransition.CrossFadeAlpha(0, 0.00001f, true);
        SceneTransition.enabled = true;
        SceneTransition.CrossFadeAlpha(1, 0.6f, false);
        yield return new WaitForSeconds(0.6f);
        /*List<SavableEntity> saveables = new List<SavableEntity>();
        foreach (var  savable in FindObjectsOfType<SavableEntity>())
        {
            saveables.Add(savable);
        }
        SavingSystem.i.CaptureEntityStates(saveables);*/
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        sun = GameObject.FindWithTag("Sun").GetComponent<DayNightCycle>();
        sun.SetTimeOfDay(timeOfDay);
        var lightArray = GameObject.FindGameObjectsWithTag("InDoorLighting");
        if (lightArray.Length  <= 0)
        {
            sun.GetComponent<Light>().enabled = true;
        }
        else
        {
            sun.GetComponent<Light>().enabled = false;
        }
        
       
        player.transform.parent.transform.position = spawnLocation;
        Debug.Log("playerLoc : " + player.transform.parent.transform.position);
        Debug.Log("spawn  :" + spawnLocation);
        player.GetComponentInParent<Character>().PrevPositionChange(new Vector3(-1, 0, 0));
        var pika = GameObject.FindGameObjectWithTag("pikachu");
        pika.transform.position = (spawnLocation + new Vector3(1, 0, 0));
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return null;
        }
        //SavingSystem.i.RestoreEntityStates(saveables);
        SceneTransition.CrossFadeAlpha(0, 0.8f, false);
        yield return new WaitForSeconds(0.7f);
        GameController.Instance.SetSate(GameState.OverWorld);
        Destroy(gameObject);
        



        
    }
}
