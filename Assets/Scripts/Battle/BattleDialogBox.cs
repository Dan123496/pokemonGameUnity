using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject selectionArrow;
    [SerializeField] Image catagory;
    public Sprite physical;
    public Sprite special;
    public Sprite status;
    bool isTyping = false;
    bool stop = false;

    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text maxPpText;
    [SerializeField] Text typeText;

    BattleState state;

    public bool IsTyping()
    {
        return isTyping;
    }
    public void Stop()
    {
        stop =true;
    }
    public void Go()
    {
        stop = false;
    }
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        isTyping = true;
        foreach (var letter in dialog.ToCharArray())
        {
            if(stop == true)
            {
                isTyping = false;
                stop = false;
                yield break;
            }
            dialogText.text += letter;
            yield return new WaitForSeconds(1f /letterPerSecond);
        }
        yield return new WaitForSeconds(1f);
        isTyping = false;

    }
    public IEnumerator ShowDialogML(Dialog dialog)
    {
        dialogText.text = "";
        isTyping = true;

        foreach (var line in dialog.Lines)
        {
            yield return (TypeDialogML(line));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
        yield return new WaitForSeconds(1f);
        isTyping = false;

    }
    public IEnumerator TypeDialogML(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
    }
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    public void SetMoveNames(List<Move> moves)
    {
        for(int i=0;i< moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves [i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
    public void UpdateMovesSelection(CurrentMove currentMove, Move move)
    {
        if (move == null)
        {
            ppText.text = "";
            maxPpText.text = "";
            typeText.text = "";
        }
        else
        {
            ppText.text = move.PP.ToString();
            maxPpText.text = move.Base.PP.ToString();
            typeText.text = move.Base.Type.ToString();
            if (move.Base.MoveCategory == Category.Physical) {
                catagory.sprite = physical;
            }else if(move.Base.MoveCategory == Category.Special)
            {
                catagory.sprite = special;
            }
            else if (move.Base.MoveCategory == Category.Status)
            {
                catagory.sprite = status;
            }
            if (move.PP <= 0)
            {
                ppText.color = Color.red;
                maxPpText.color = Color.red;
            }
            else
            {
                ppText.color = Color.black;
                maxPpText.color = Color.black;
            }
        }
    }

}
