using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text typeText;
    [SerializeField] Text ppText;
    [SerializeField] Image catImage;
    [SerializeField] Text powerText;
    [SerializeField] Text accText;


    MoveBase _move;
    [SerializeField] Sprite physical;
    [SerializeField] Sprite special;
    [SerializeField] Sprite status;

    public void InitData(MoveBase move)
    {
        _move = move;
        if(move != null)
        {
            UpdateData();
        }
        else
        {
            SetMoveNull();
        }
     
    }
    void UpdateData()
    {
        nameText.text = _move.Name;
        ppText.text = "" + _move.PP;
        powerText.text = _move.Power.ToString();
        typeText.text = _move.Type.ToString();
        accText.text = _move.Accuary.ToString();


        switch (_move.MoveCategory)
        {
            case Category.Physical:
                catImage.sprite = physical;
                break;
            case Category.Special:
                catImage.sprite = special;
                break;
            case Category.Status:
                catImage.sprite = status;
                break;
        }
        
        
       
    }
    void SetMoveNull()
    {
        nameText.text = "-";
        ppText.text = "-";
        powerText.text = "-";
        typeText.text = "-";
        accText.text = "-";
        catImage.sprite = status;
    }
}
