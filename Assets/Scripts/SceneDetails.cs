using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool isLoaded { get; private set; }
    [SerializeField] bool inDoor;
    private void OnTriggerEnter(Collider other)
    {
        if (inDoor)
        {
            GameController.Instance.SetCurrentScene(this);

            return;
        }
        if(other.gameObject.name == "Player")
        {
            StartCoroutine(LoadSceneAysnc());
            GameController.Instance.SetCurrentScene(this);
            foreach ( var scene in connectedScenes)
            {
                StartCoroutine(scene.LoadSceneAysnc());
            }
            if (GameController.Instance.PreviousScene != null)
            {
                var PrevScenes = GameController.Instance.PreviousScene.connectedScenes;
                foreach (var scene in PrevScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this && scene)
                    {
                        Debug.Log(scene + " Unloaded");
                        Debug.Log("by  " + this);
                        StartCoroutine(scene.UnLoadSceneAysnc());
                    }
                }
            }
        }
        
       
    }
    public IEnumerator LoadSceneAysnc()
    {
        if (!isLoaded)
        {
            isLoaded = true;
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            while (!loadScene.isDone)
            {
                yield return null;
            }
            Debug.Log(gameObject.name + "  Loaded");
           
            
        }
    }
    public IEnumerator UnLoadSceneAysnc()
    {
        if (isLoaded)
        {
            isLoaded = false;
            AsyncOperation unLoadScene = SceneManager.UnloadSceneAsync(gameObject.name);
            while (!unLoadScene.isDone)
            {
                yield return null;
            }
            Debug.Log(gameObject.name + "  Unloaded");
            
        }
            
    }
    public bool IsIndoor
    {
        get => inDoor;
    }
}
