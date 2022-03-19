using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleTransition : MonoBehaviour
{
    public AudioSource battleTheme;
    public Image transition1;
    public Image transition2;
    // Start is called before the first frame update
    public void Start()
    {
        
    }
    
    public IEnumerator Transition1()
    {
        transition1.fillAmount = 0f;
        Debug.Log("started");
        battleTheme.Play();
        while (transition1.fillAmount + Mathf.Epsilon <= 1f)
        {
            transition1.fillAmount += 0.002f;
            Debug.Log(transition1.fillAmount);
            yield return null;

        }
        transition1.fillAmount = 1f;
    }
    public IEnumerator Transition2()
    {
        if (transition2.gameObject.activeSelf == false)           
        {
            transition2.gameObject.SetActive(true);
        }
        transition2.fillAmount = 1f;
        while (transition2.fillAmount - Mathf.Epsilon >= 0)
        {
            transition2.fillAmount -= 0.002f;
            yield return null;
        }
        transition2.fillAmount = 0f;
    }
}



    
