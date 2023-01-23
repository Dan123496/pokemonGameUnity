using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Text currentHP;
    [SerializeField] Text maxHP;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Image icon;
    [SerializeField] Image statusIcon;
    Pokemon _pokemon;

    public void InitData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        _pokemon.OnHpChanged += UpdateData;
        _pokemon.OnStatusChanged += UpdateData;
        UpdateData();
        SetMessage("");




    }
    void UpdateData()
    {
        nameText.text = _pokemon.Base.Name;
        levelText.text = "" + _pokemon.Level;
        maxHP.text = _pokemon.MaxHp.ToString();
        currentHP.text = _pokemon.HP.ToString();
        icon.sprite = _pokemon.Base.Icon;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
        
        
        if (_pokemon.Status != null)
        {
            if (_pokemon.Status.Name == "none")
            {
                statusIcon.gameObject.SetActive(false);
                statusIcon.sprite = null;
            }
            else
            {
                statusIcon.gameObject.SetActive(true);
                statusIcon.sprite = _pokemon.Status.Icon;
            }
        }
        else
        {
            statusIcon.gameObject.SetActive(false);
            statusIcon.sprite = null;
        }
    }
    public TextMeshProUGUI MessageText => messageText;
    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
