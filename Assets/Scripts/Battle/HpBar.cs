using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{

    [SerializeField] GameObject health;

    bool isUpdating;


    // Start is called before the first frame update
    public bool IsUpdating => isUpdating;

    // Update is called once per frame
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }
    public IEnumerator SetHPSmothly(float newHp)
    {
        isUpdating = true;
        Debug.Log(newHp);
        float currentHp = health.transform.localScale.x;
        float changeAmount = currentHp - newHp;
        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            
            health.transform.localScale = new Vector3(currentHp, 1f);

            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f);
        isUpdating = false;
    }
}
