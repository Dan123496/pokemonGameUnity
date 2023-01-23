using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingPortal : MonoBehaviour
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Vector3 spawnLocation;
    [SerializeField] Image SceneTransition;
    [SerializeField] string sceneName;
    DayNightCycle sun;
    float timeOfDay;

     void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            SceneTransition = GameObject.FindWithTag("SceneTransition").GetComponent<Image>();
            sun = GameObject.FindWithTag("Sun").GetComponent<DayNightCycle>();
            if (sun != null)
            {
                timeOfDay = sun.TimeOfDay;
            }
            var player = collider.gameObject.GetComponentInChildren<PlayerController>();

            StartCoroutine(SwitchScene(player));
        }
            
    }
    IEnumerator SwitchScene(PlayerController player)
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.SetSate(GameState.busy);
        
        SceneTransition.enabled = true;
        /*List<SavableEntity> saveables = new List<SavableEntity>();
        foreach (var savable in FindObjectsOfType<SavableEntity>())
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
        player.GetComponentInParent<Character>().PrevPositionChange(new Vector3(-1, 0, 0));
        var pika = GameObject.FindGameObjectWithTag("pikachu");
        pika.transform.position = (spawnLocation + new Vector3(1, 0, 0));
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return null;
        }
        //SavingSystem.i.RestoreEntityStates(saveables);
        //Debug.Log(saveables[0]);
        SceneTransition.CrossFadeAlpha(0, 0.8f, false);
        yield return new WaitForSeconds(0.7f);
        GameController.Instance.SetSate(GameState.OverWorld);
        



        Destroy(gameObject);
    }
}
