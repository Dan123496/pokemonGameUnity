using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    [SerializeField] GameObject expBar;
    [SerializeField] Text currentHP;
    [SerializeField] Text maxHP;
    Pokemon _pokemon;
    [SerializeField] Image icon;

    public void SetData(Pokemon pokemon)
    {

        if (_pokemon != null)
        {
            _pokemon.OnHpChanged -= UpdateHp;
        }
        
        _pokemon = pokemon;
        
        nameText.text = pokemon.Base.Name;
        SetLevel();
        maxHP.text = pokemon.MaxHp.ToString();
        currentHP.text = pokemon.HP.ToString();
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        SetExp();
        UpdateStatus(pokemon);
       
        _pokemon.OnHpChanged += UpdateHp;



    }
    public void ClearData()
    {
        nameText.text = "";
        levelText.text = "";
        maxHP.text = "";
        currentHP.text = "";
        hpBar.SetHP(1f / 1f);
        UpdateStatus(null);
    }
    public void SetExp()
    {
        if(expBar == null) return;
        float normalisedExp = GetNormalisedExp();
        Debug.Log(normalisedExp);
        expBar.transform.localScale = new Vector3(normalisedExp, 1, 1);
    }
    public IEnumerator SetExpSmoothly(bool reset = false)
    {
        if (expBar == null) yield break;
        if (reset== true)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        float normalisedExp = GetNormalisedExp();
        yield return expBar.transform.DOScaleX(normalisedExp, 1f).WaitForCompletion();
        
    }
    float GetNormalisedExp()
    {
        int currLevelExp = _pokemon.Base.ExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.ExpForLevel(_pokemon.Level+1);
        float normalisedExp = (float)(_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalisedExp);
    }
    // Start is called before the first frame update
    public void UpdateHp()
    {
        StartCoroutine(UpdateHpAsync());
    }
    public IEnumerator UpdateHpAsync()
    {
        yield return hpBar.SetHPSmothly((float)_pokemon.HP / _pokemon.MaxHp);
        currentHP.text = _pokemon.HP.ToString();  
    }
    public void SetLevel()
    {
        levelText.text = "" + _pokemon.Level;
    }
    public void UpdateStatus(Pokemon pokemon)
    {
        if (pokemon == null)
        {
            icon.gameObject.SetActive(false);
        }
        else if (pokemon.Status != null)
        {
            if (pokemon.Status.Name == "none")
            {
                icon.gameObject.SetActive(false);
                icon.sprite = null;
            }
            else
            {
                icon.gameObject.SetActive(true);
                icon.sprite = pokemon.Status.Icon;
                Debug.Log(pokemon.Status.Name);
            }
        }
        else
        {
            icon.gameObject.SetActive(false);
            
        }
       

    }
    public IEnumerator WaitForHpUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }
    public void ClearEvents()
    {
        if (_pokemon != null)
        {
            _pokemon.OnHpChanged -= UpdateHp;
        }
    }
}