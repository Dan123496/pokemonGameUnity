using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentAction { Fight, Pokemon, Bag, Run }
public enum CurrentMove { TopLeftMove, TopRightMove, BottomLeftMove, BottomRightMove }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enermyUnit;
    [SerializeField] BattleHud enermyHud;
    [SerializeField] BattleDialogBox dialogBox;
	[SerializeField] GameObject selectionArrow;
    [SerializeField] GameObject movesSelectionArrow;
    public GameObject world;
    public GameObject battle;
    
    public Image transition2;

    public event Action<bool> OnBattleOver;

    private PartyPokemon playerParty;
    private Pokemon wildPokemon;

    BattleState state;
	int currentActionX;
	int currentActionY;
    int currentMoveX;
    int currentMoveY;
    int setMove;

    CurrentAction action = CurrentAction.Fight;
    CurrentMove currentMove = CurrentMove.TopLeftMove; 



    public void StartBattle(PartyPokemon playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        state = BattleState.Start;
        StartCoroutine(SetUpBattle());
        dialogBox.EnableActionSelector(false);
    }
    public IEnumerator SetUpBattle()
    {
        
        playerUnit.SetUp(playerParty.GetHealthyPokemon());
        playerHud.SetData(playerUnit.Pokemon);
        enermyUnit.SetUp(wildPokemon);
        enermyHud.SetData(enermyUnit.Pokemon);
        StartCoroutine(Transition2());
        yield return new WaitForSeconds(1.5f);
        enermyUnit.PlayerEnterAnimation();
        yield return dialogBox.TypeDialog($"A wild {enermyUnit.Pokemon.Base.Name} appered !");
        playerUnit.PlayerEnterAnimation();
        yield return dialogBox.TypeDialog($"Go {playerUnit.Pokemon.Base.Name} !");



        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        
        PlayerAction();
    }
    public IEnumerator Transition2()
    {
    
        transition2.fillAmount = 1f;

        while ((transition2.fillAmount - 0.002f) > 0)
        {
            transition2.fillAmount -= Time.deltaTime;
            Debug.Log(transition2.fillAmount);
            yield return null;
        }
       
        transition2.fillAmount = 0f;

        
        

    }
    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.EnableMoveSelector(false);
        StartCoroutine(dialogBox.TypeDialog($"What will {playerUnit.Pokemon.Base.Name} do ?"));
        dialogBox.EnableActionSelector(true);
      

    }
    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
       

    }
    IEnumerator PreformPlayerMove()
    {
        state = BattleState.Busy;
        Debug.Log((int)currentMove);
       var move = playerUnit.Pokemon.Moves[(int)currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        enermyUnit.PlayHitAnimation();
        yield return new WaitForSeconds(1f);
        var damageDetails = enermyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enermyHud.UpdateHP();
        yield return ShowDamageDetalis(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enermyUnit.Pokemon.Base.Name} Fainted ");
            enermyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnermyMove());
        }


    }
    IEnumerator EnermyMove()
    {
        state = BattleState.EnemyMove;
        var move = enermyUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{enermyUnit.Pokemon.Base.Name} used {move.Base.Name}");

        enermyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerUnit.PlayHitAnimation();
        yield return new WaitForSeconds(1f);
        var damageDetails  = playerUnit.Pokemon.TakeDamage(move, enermyUnit.Pokemon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetalis(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted ");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            var nextpokemon = playerParty.GetHealthyPokemon();
            if(nextpokemon == null)
            {
                OnBattleOver(false);
            }else
            {
                playerUnit.SetUp(nextpokemon);
                playerHud.SetData(nextpokemon);
                

                playerUnit.PlayerEnterSecondAnimation();
                yield return dialogBox.TypeDialog($"Go {nextpokemon.Base.Name} !");
                PlayerAction();
                dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            }
            
        }   
        else
        {
            PlayerAction();
        }

    }
    IEnumerator ShowDamageDetalis(DamageDetails damageDetails)
    {
        if (damageDetails.Crit > 1f)
        {
            yield return dialogBox.TypeDialog("A Critical Hit!");

        }
       
        if (damageDetails.TypeEffect ==4)
        {
            yield return dialogBox.TypeDialog("its HYPER-Effective!");
        }
        else if (damageDetails.TypeEffect ==2)
        {
            yield return dialogBox.TypeDialog("its Super-Effective!");
        }else if (damageDetails.TypeEffect == 0.5f)
        {
            yield return dialogBox.TypeDialog("its not very effective");
        }else if (damageDetails.TypeEffect ==0.25f)
        {
            yield return dialogBox.TypeDialog("its Really not very effective!");
        }
    }
    public void HandleUpdate()
    {
        if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
    }
	void HandleActionSelection()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
		{
			if (currentActionY == 1)
			{
				currentActionY= 0;

			}
			else if (currentActionY == 0)
			{
				currentActionY = 1;
			}
            MoveActionSelectionArrow();

        }
		else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
		{
			if (currentActionX == 1)
			{
				currentActionX = 0;
			}
			else if (currentActionX == 0)
			{
                currentActionX = 1;
			}
            MoveActionSelectionArrow(); ;

        }
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        {
           if(action == CurrentAction.Fight)
            {

                PlayerMove();


            }
           else if(action == CurrentAction.Run)
            {
                world.SetActive(true);
                battle.SetActive(false);
            }
        }
 
    }
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentMoveY == 1)
            {
                currentMoveY = 0;

            }
            else if (currentMoveY == 0)
            {
                currentMoveY = 1;
            }
            MoveMovesSelectionArrow();

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentMoveX == 1)
            {
                currentMoveX = 0;
            }
            else if (currentMoveX == 0)
            {
                currentMoveX = 1;
            }
            MoveMovesSelectionArrow();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerAction();
        }
          

        if ((int)currentMove <= playerUnit.Pokemon.Moves.Count -1)
        {
            dialogBox.UpdateMovesSelection(currentMove, playerUnit.Pokemon.Moves[(int)currentMove]);
        }
        else
        {
            
            dialogBox.UpdateMovesSelection(currentMove, null);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(Input.GetKeyDown(KeyCode.Z));
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PreformPlayerMove());
        }



    }


     void MoveActionSelectionArrow()
    {
        if(currentActionX == 0 && currentActionY == 0)
        {
            selectionArrow.transform.localPosition = new Vector3(-135, 18, -10);
            action = CurrentAction.Fight; 
        }
        else if (currentActionX == 0 && currentActionY == 1)
        {
            selectionArrow.transform.localPosition = new Vector3(-135, -24, -10);
            action = CurrentAction.Pokemon;
        }
        else if (currentActionX == 1 && currentActionY == 0)
        {
            selectionArrow.transform.localPosition = new Vector3( 22, 18, -10);
            action = CurrentAction.Bag;
        }
        else if (currentActionX == 1 && currentActionY == 1)
        {
            selectionArrow.transform.localPosition = new Vector3(22, -24, -10);
            action = CurrentAction.Run;
        }
    }
    void MoveMovesSelectionArrow()
    {
        if (currentMoveX == 0 && currentMoveY == 0)
        {
            movesSelectionArrow.transform.localPosition = new Vector3(-360, 25, -10);
            currentMove = CurrentMove.TopLeftMove;
        }
        else if (currentMoveX == 0 && currentMoveY == 1)
        {
            movesSelectionArrow.transform.localPosition = new Vector3(-360, -25, -10);
            currentMove = CurrentMove.BottomLeftMove;
        }
        else if (currentMoveX == 1 && currentMoveY == 0)
        {
            movesSelectionArrow.transform.localPosition = new Vector3(-140, 25, -10);
            currentMove = CurrentMove.TopRightMove;
        }
        else if (currentMoveX == 1 && currentMoveY == 1)
        {
            movesSelectionArrow.transform.localPosition = new Vector3(-140, -25, -10);
            currentMove = CurrentMove.BottomRightMove;
        }
    }
    private void OnEnable()
    {
        
    }
    
}

