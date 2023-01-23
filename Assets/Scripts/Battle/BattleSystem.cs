using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentAction { Fight, Pokemon, Bag, Run }
public enum CurrentMove { TopLeftMove, TopRightMove, BottomLeftMove, BottomRightMove }

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartySelect, MoveForget, BattleOver, Bag }
public enum BattleAction  { Move, SwitchPokemon, UseItem, Run }
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enermyUnit;
 
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] RectTransform selectionArrow;
    [SerializeField] GameObject movesSelectionArrow;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject playerPokemonCounter;
    [SerializeField] GameObject enermyPokemonCounter;
    [SerializeField] InventoryUI inventoryUI;
    PokemonCounter playerCounterScript;
    PokemonCounter enermyCounterScript;
    [SerializeField] LearnMoveUi learnMoveUI;
    [SerializeField] Text turnCountertext;

    MoveBase moveToLearn;

    public GameObject battle;
  
    public Image transition2;

    [SerializeField] AudioClip victoryMusic;

    public AudioClip VictoryMusic
    {
        get => victoryMusic;

        set => victoryMusic = value;
    }


    [Header("soundeffects")]
    [SerializeField] AudioSource SoundEffectAudio;
    [SerializeField] AudioClip error;
    [SerializeField] AudioClip back;
    [SerializeField] AudioClip cursor;
    [SerializeField] AudioClip next;
    [SerializeField] AudioClip damage;
    [SerializeField] AudioClip pokemonEnter;
    [SerializeField] AudioClip pokemonExit;
    [SerializeField] AudioClip pokeballThrow;
    [SerializeField] AudioClip pokeballCatch;
    [SerializeField] AudioClip pokeballDrop;
    [SerializeField] AudioClip pokeballShake;
    [SerializeField] AudioClip pokeballbreak;
    [SerializeField] AudioClip pokeballCaught;
    [SerializeField] AudioClip LevelUp;

    public event Action<bool> OnBattleOver;

    

    private PartyPokemon playerParty;
    private PartyPokemon trainerParty;
    private Pokemon wildPokemon;

    PlayerController player;
    TrainerController trainer;
    AudioSource musicAudioSource;

    BattleState state;
    BattleState? prevState;
    int currentActionX;
    int currentActionY;
    int currentMoveX;
    int currentMoveY;
    int setMove;
    int turnCounter;
    int escapeAttempts = 0;
    bool fainted;
    float sameSpeedDecider;
    bool isTrainerBattle = false;
    CurrentAction action = CurrentAction.Fight;
    CurrentMove currentMove = CurrentMove.TopLeftMove;

    private void Awake()
    {
        playerCounterScript = playerPokemonCounter.GetComponent<PokemonCounter>();
        enermyCounterScript = enermyPokemonCounter.GetComponent<PokemonCounter>();
       
    }
    public void Start()
    {
        musicAudioSource = player.GetComponent<AudioSource>();
    }

    public void StartBattle(PartyPokemon playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        playerPokemonCounter.SetActive(false);
        enermyPokemonCounter.SetActive(false);
        isTrainerBattle = false;
        playerUnit.gameObject.SetActive(true);
        enermyUnit.gameObject.SetActive(true);
        state = BattleState.Start;
        dialogBox.transform.GetChild(0).GetComponent<Text>().text = "";
        
        StartCoroutine(SetUpBattle());
        dialogBox.EnableActionSelector(false);
        sameSpeedDecider = UnityEngine.Random.value;
    }

    public void StartTrainerBattle(PartyPokemon playerParty, PartyPokemon trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        isTrainerBattle = true;
        playerUnit.gameObject.SetActive(true);
        enermyUnit.gameObject.SetActive(true);
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        dialogBox.transform.GetChild(0).GetComponent<Text>().text = "";
        state = BattleState.Start;
        
        StartCoroutine(SetUpBattle());
        dialogBox.EnableActionSelector(false);
        sameSpeedDecider = UnityEngine.Random.value;
    }
    public IEnumerator SetUpBattle()
    {
        turnCounter = 1;
        turnCountertext.text = turnCounter.ToString();
        playerUnit.Clear();
        enermyUnit.Clear();
        playerUnit.DisableHud();
        enermyUnit.DisableHud();
        escapeAttempts = 0;
        if (!isTrainerBattle)
        {
            //wildBattle
            trainerImage.gameObject.SetActive(false);
            playerUnit.SetUp(playerParty.GetHealthyPokemon());
            enermyUnit.SetUp(wildPokemon);

            playerPokemonCounter.SetActive(false);
            enermyPokemonCounter.SetActive(false);

            
            var enermyCrySource = enermyUnit.GetComponent<AudioSource>();
            enermyCrySource.clip = enermyUnit.Pokemon.Base.Cry;
            var playerCrySource = playerUnit.GetComponent<AudioSource>();
            playerCrySource.clip = playerUnit.Pokemon.Base.Cry;
            
            StartCoroutine(Transition2());
            yield return new WaitForSeconds(1.5f);
            enermyUnit.EnterAnimation();
            yield return new WaitForSeconds(0.2f);
            enermyCrySource.Play();
            yield return dialogBox.TypeDialog($"A wild {enermyUnit.Pokemon.Base.Name} appered !");
            playerUnit.EnterAnimation();
            yield return new WaitForSeconds(0.2f);
            playerCrySource.Play();
            yield return dialogBox.TypeDialog($"Go {playerUnit.Pokemon.Base.Name} !");
            yield return new WaitForSeconds(1f);
        }
        else
        {
            //trainerBattle         
            playerUnit.gameObject.SetActive(false);
            enermyUnit.gameObject.SetActive(false);
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprites[0];
            trainerImage.sprite = trainer.Sprite;
            
            StartCoroutine(Transition2());
            yield return new WaitForSeconds(1.5f);
            yield return dialogBox.TypeDialog($"{trainer.TrainerName} wants to battle!");           
            
            enermyUnit.TrainerEnterAnimation();
            yield return new WaitForSeconds(1f);
            
            enermyUnit.gameObject.SetActive(true); ;
            enermyUnit.SetUp(trainerParty.GetHealthyPokemon());
            
            enermyPokemonCounter.SetActive(true);
            enermyCounterScript.SetPokeBalls(trainerParty);

            var enermyCrySource = enermyUnit.GetComponent<AudioSource>();
            enermyCrySource.clip = enermyUnit.Pokemon.Base.Cry;           
            enermyUnit.TrainnerPokemonEnter();
            yield return new WaitForSeconds(0.2f);
            enermyCrySource.Play();
            yield return dialogBox.TypeDialog($"{trainer.TrainerName} sent out {enermyUnit.Pokemon.Base.Name}!");           
            
            
            playerUnit.gameObject.SetActive(true);
            playerUnit.SetUp(playerParty.GetHealthyPokemon());
            playerPokemonCounter.SetActive(true);
            playerCounterScript.SetPokeBalls(playerParty);
            var playerCrySource = playerUnit.GetComponent<AudioSource>();
            playerCrySource.clip = playerUnit.Pokemon.Base.Cry;
            playerUnit.EnterAnimation();
            yield return new WaitForSeconds(0.2f);
            playerCrySource.Play();
            yield return dialogBox.TypeDialog($"Go {playerUnit.Pokemon.Base.Name} !");
            yield return new WaitForSeconds(1.5f);
        }
        partyScreen.Initi();
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        ActionSelection();
    }
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        this.playerParty = null;
        wildPokemon = null;
        SetUpBattle();
        dialogBox.transform.GetChild(0).GetComponent<Text>().text = "";
        playerCounterScript.ResetPokeBalls();
        enermyCounterScript.ResetPokeBalls();
        playerPokemonCounter.SetActive(false);
        enermyPokemonCounter.SetActive(false);
        playerUnit.Hud.ClearEvents();
        enermyUnit.Hud.ClearEvents();
        playerUnit.BattleOver();
        enermyUnit.BattleOver();
        OnBattleOver(won);
    }

    public IEnumerator Transition2()
    {
        transition2.fillAmount = 1f;

        while ((transition2.fillAmount - 0.002f) > 0)
        {
            transition2.fillAmount -= Time.deltaTime;
            
            yield return null;
        }
        transition2.fillAmount = 0f;
    }
    void ActionSelection()
    {
        dialogBox.EnableMoveSelector(false);
        state = BattleState.ActionSelection;
        dialogBox.EnableDialogText(true);
        if(dialogBox.IsTyping() == true)
        {
            dialogBox.Stop();
        }
        Debug.Log("type");
        dialogBox.Go();
        StartCoroutine(dialogBox.TypeDialog($"What will {playerUnit.Pokemon.Base.Name} do ?"));
        dialogBox.EnableActionSelector(true);

    }
    void OpenPartyScreen()
    {
        prevState = state;
        state = BattleState.PartySelect;
        dialogBox.EnableActionSelector(false);
        partyScreen.SetPartyData();
        partyScreen.gameObject.SetActive(true);
        partyScreen.MessageText.text = "Select a Pokemon";
        partyScreen.getSelectedSprite();
    }
    
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Choose a move you want to forget");
        learnMoveUI.gameObject.SetActive(true);
        learnMoveUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleState.MoveForget;


    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if(playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[(int)currentMove];
            enermyUnit.Pokemon.CurrentMove = enermyUnit.Pokemon.GetRandomMove();           
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enermyMovePriority = enermyUnit.Pokemon.CurrentMove.Base.Priority;
            bool playerGoseFirst = true;
            if(enermyMovePriority > playerMovePriority)
            {
                playerGoseFirst = false;
            }
            else if (enermyMovePriority == playerMovePriority)
            {
                if(playerUnit.Pokemon.Speed > enermyUnit.Pokemon.Speed)
                {
                    playerGoseFirst = true;
                }
                else if(playerUnit.Pokemon.Speed < enermyUnit.Pokemon.Speed)
                {
                    playerGoseFirst = false;
                }
                else if (playerUnit.Pokemon.Speed == enermyUnit.Pokemon.Speed)
                {
                    if (sameSpeedDecider == 3)
                    {
                        playerGoseFirst = true;
                        sameSpeedDecider = 2;
                    }
                    else if (sameSpeedDecider == 2)
                    {
                        playerGoseFirst = false;
                        sameSpeedDecider = 3;
                    }
                    else if (sameSpeedDecider <= 0.5f)
                    {
                        playerGoseFirst = true;
                        sameSpeedDecider = 2;
                    }
                    else if (sameSpeedDecider > 0.5f)
                    {
                        playerGoseFirst = false;
                        sameSpeedDecider = 3;
                    }
                }  
            }
            var firstUnit = (playerGoseFirst) ? playerUnit : enermyUnit;
            var secondUnit = (playerGoseFirst) ? enermyUnit : playerUnit;
            var secondPokemon = secondUnit.Pokemon;
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunOnAfterTurn(firstUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
            if (secondPokemon.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunOnAfterTurn(secondUnit);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }
            }           
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
            }
            else if(playerAction == BattleAction.Run)
            {
                yield return TryToRun();
            }
            
            var enermyMove = enermyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enermyUnit, playerUnit, enermyMove);
            yield return RunOnAfterTurn(enermyUnit);
            if(state == BattleState.BattleOver)
            {
                yield break;
            }
        }
        if(state != BattleState.BattleOver)
        {
            turnCounter++;
            turnCountertext.text = turnCounter.ToString();
            ActionSelection();
        }      
    }
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        
        bool canMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHpUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        sourceUnit.Hud.UpdateStatus(sourceUnit.Pokemon);
        move.PP--;
        if (move.PP < 0)
        {
            move.PP = 0;
        }
        if (dialogBox.IsTyping() == true)
        {
            dialogBox.Stop();
            
        }
        dialogBox.Go();
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (checkMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            if (move.Base.MoveCategory == Category.Status)
            {

                yield return RunMoveEffects(move.Base.Effects, sourceUnit, targetUnit, move.Base.Target);
            }

            else
            {

                targetUnit.PlayHitAnimation();
                yield return new WaitForSeconds(1f);
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHpUpdate();
                yield return ShowDamageDetalis(damageDetails);
            }
            if(move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if(rnd <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit, targetUnit, secondary.Target);
                    }
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s Attack Missed!");
        }
        
        
    }

    IEnumerator RunMoveEffects(MoveEffects effects, BattleUnit sourceUnit, BattleUnit targetUnit, MoveTarget moveTarget)
    {
       
       
        if (effects.Boosts != null)
        {
            sourceUnit.PlayStatAnimation(effects, moveTarget);
            if (moveTarget  == MoveTarget.self)
            {
                sourceUnit.Pokemon.ApplyBoosts(effects.Boosts);
            }
            else
            {
                targetUnit.Pokemon.ApplyBoosts(effects.Boosts);
            }
        }
        
        if(effects.Staus != ConditionID.none)
        {
            targetUnit.Pokemon.SetStatus(effects.Staus);
            targetUnit.Hud.UpdateStatus(targetUnit.Pokemon);
        }
        if (effects.VolatileStaus != ConditionID.none)
        {
            targetUnit.Pokemon.SetVolatileStatus(effects.VolatileStaus);
           
        }

        yield return new WaitForSeconds(1f);
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return ShowStatusChanges(targetUnit.Pokemon);
        yield return new WaitForSeconds(1f);
    }
    IEnumerator RunOnAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state ==BattleState.RunningTurn);
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHpUpdate();

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }
    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted ");
        faintedUnit.Pokemon.SetStatus(ConditionID.ftn);
        faintedUnit.Hud.UpdateStatus(faintedUnit.Pokemon);
        if (isTrainerBattle)
        {
            if (faintedUnit.IsPlayerUnit)
            {
                playerCounterScript.SetPokeBalls(playerParty);
            }
            else
            {
                enermyCounterScript.SetPokeBalls(trainerParty);
            }
        }
        
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            if (!isTrainerBattle)
            {
                musicAudioSource.clip = victoryMusic;
                musicAudioSource.Play();

            }
            yield return HandleExpGain(faintedUnit);
        }
        CheckForBattleEnd(faintedUnit);
    }
    IEnumerator HandleExpGain(BattleUnit faintedUnit)
    {
       int expYield = faintedUnit.Pokemon.Base.ExpYield;
       int level = faintedUnit.Pokemon.Level;
       int yourLevel = playerUnit.Pokemon.Level;
       float trainerBounus = 1f;
       if(isTrainerBattle)
       {
            trainerBounus = 1.5f;
       }
       int expGain = Mathf.FloorToInt((expYield * level * trainerBounus) / 7);
       int scaledExpGain = Mathf.FloorToInt(((trainerBounus * expYield * level) / 5) * (Mathf.Pow(((2f * (float)level) + 10f) / ((float)level + (float)yourLevel + 10f), 2.5f)) + 1);
       playerUnit.Pokemon.Exp += scaledExpGain;
       yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained  {scaledExpGain} EXP");
       yield return playerUnit.Hud.SetExpSmoothly();
       while (playerUnit.Pokemon.CheckForLevelUp())
       {
            playerUnit.Hud.SetLevel();
            SoundEffectAudio.PlayOneShot(LevelUp);
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level} !");
            var newMove = playerUnit.Pokemon.GetLearnableMoveAtLevel();
            Debug.Log(playerUnit.Pokemon.GetLearnableMoveAtLevel());
            if(newMove != null)
            {
                if(playerUnit.Pokemon.Moves.Count < PokemonBase.MaxMoves)
                {
                    playerUnit.Pokemon.LearnMove(newMove.Base);
                    yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learnt how to use {newMove.Base.Name} !");
                    dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                }
                else
                {
                    yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} wants to learn how to use {newMove.Base.Name}");
                    yield return dialogBox.TypeDialog($"but {playerUnit.Pokemon.Base.Name} Can't learn move than four moves");  
                    yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.Base);
                    yield return new WaitUntil(() => state != BattleState.MoveForget);
                    yield return new WaitForSeconds(2f);
                }
            }
            yield return playerUnit.Hud.SetExpSmoothly(true);
       }
       
       yield return new WaitForSeconds(0.5f);
    }
    IEnumerator ShowStatusChanges(Pokemon pokemon) 
    { 
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
    void CheckForBattleEnd(BattleUnit faintedUnit)
    {
        if(faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon != null)
            {
                prevState = null;
                fainted = true;
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);

            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                }
                else
                {
                    VictoryMusic = trainer.VictoryTheme;
                    musicAudioSource.clip = VictoryMusic;
                    musicAudioSource.Play();
                    state = BattleState.Busy;
                    enermyUnit.TrainnerDefetedAnimation();
                    dialogBox.EnableActionSelector(false);
                    
                    StartCoroutine(DisplayEndingMessage());
                    
                }
            }
        }
    }
    IEnumerator DisplayEndingMessage()
    {
        
        yield return dialogBox.ShowDialogML(trainer.DefeatedDialog);
        BattleOver(true);
    }
    bool checkMoveHits(Move move, Pokemon source, Pokemon targetPokemon)
    {
        if (move.Base.AlwaysHits)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuary;
        int accacyStat = source.StatBoosts[Stat.Accuracy];
        int evasion = source.StatBoosts[Stat.Evasion];

        var boostVals = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };
        if (accacyStat >0)
        {
            moveAccuracy *= boostVals[accacyStat];
        }
        else
        {
            moveAccuracy /= boostVals[-accacyStat];
        }
        if (evasion > 0)
        {
            moveAccuracy /= boostVals[evasion];
        }
        else
        {
            moveAccuracy *= boostVals[-evasion];
        }

        if (UnityEngine.Random.Range(1,101) <= moveAccuracy)
        {
            return true; 
        }
        else
        {
            return false;
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
        if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PartySelect)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                SoundEffectAudio.PlayOneShot(back);
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };
            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                SoundEffectAudio.PlayOneShot(next);
                StartCoroutine(OnItemUsed(usedItem));
            };
            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.MoveForget)
        {
            Action<int> onMoveSelect = (moveIndex) =>
            {
                learnMoveUI.gameObject.SetActive(false);
                if (moveIndex == 0)
                {
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon} did not learn {moveToLearn.Name}"));
                }
                else
                {
                    var selectedMove = playerUnit.Pokemon.Moves[moveIndex-1].Base;
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon} forgot the {selectedMove.Name} and learn the move {moveToLearn.Name}"));
                    playerUnit.Pokemon.Moves[moveIndex-1] = new Move(moveToLearn);
                }
                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            

            learnMoveUI.HandleSelection(onMoveSelect);
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
            SoundEffectAudio.PlayOneShot(cursor);
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
            MoveActionSelectionArrow();
            SoundEffectAudio.PlayOneShot(cursor);
        }
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        {
           
            if (action == CurrentAction.Fight)
            {
                MoveSelection();
                SoundEffectAudio.PlayOneShot(next);
            }
           else if(action == CurrentAction.Bag)
            {
                OpenBag();
                SoundEffectAudio.PlayOneShot(next);

            }
            else if (action == CurrentAction.Pokemon)
            {
               
                OpenPartyScreen();
                SoundEffectAudio.PlayOneShot(next);
            }
            else if (action == CurrentAction.Run)
            {

                if (isTrainerBattle)
                {
                    SoundEffectAudio.PlayOneShot(error);
                    if (dialogBox.IsTyping() == false)
                    {
                        StartCoroutine(dialogBox.TypeDialog($"There's no Running from a trainer Battle!"));
                    }
                    
                }
                else
                {
                    SoundEffectAudio.PlayOneShot(next);
                    StartCoroutine(RunTurns(BattleAction.Run));
                }
                
            }
        }
    }

    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);

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
            SoundEffectAudio.PlayOneShot(cursor);
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
            SoundEffectAudio.PlayOneShot(cursor);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {

            ActionSelection();
            SoundEffectAudio.PlayOneShot(back);

        }

        if ((int)currentMove <= playerUnit.Pokemon.Moves.Count - 1)
        {
            dialogBox.UpdateMovesSelection(currentMove, playerUnit.Pokemon.Moves[(int)currentMove]);
        }
        else
        {
            dialogBox.UpdateMovesSelection(currentMove, null);
        }
        if (Input.GetKeyDown(KeyCode.Z) && (int)currentMove < playerUnit.Pokemon.Moves.Count)
        {
            var move = playerUnit.Pokemon.Moves[(int)currentMove];
            if (move.PP <= 0)
            {
                SoundEffectAudio.PlayOneShot(error);
                return;
            }
            SoundEffectAudio.PlayOneShot(next);
            Debug.Log(Input.GetKeyDown(KeyCode.Z));
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        if (Input.GetKeyDown(KeyCode.Z) && (int)currentMove >= playerUnit.Pokemon.Moves.Count)
        {
            SoundEffectAudio.PlayOneShot(error);
            Debug.Log("click non-move");
        }
    }

    void HandlePartySelection()
    {
        Action onSelected = () =>
        {

            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.MessageText.text = "Can not send out a Fainted Pokemon!";
                SoundEffectAudio.PlayOneShot(error);
                return;
            }
            if(selectedMember == playerUnit.Pokemon)
            {
                partyScreen.MessageText.text = $"{playerUnit.Pokemon.Base.Name} is already out !";
                SoundEffectAudio.PlayOneShot(error);
                return;
            }
            SoundEffectAudio.PlayOneShot(next);
            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {

                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
            prevState = null;
        };
        Action onBack = () =>
        {
            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                SoundEffectAudio.PlayOneShot(back);
                partyScreen.gameObject.SetActive(false);

                ActionSelection();
            }
            else if (prevState != BattleState.ActionSelection)
            {
                SoundEffectAudio.PlayOneShot(error);
                partyScreen.MessageText.text = "You Must Select A new, non fainted, Pokemon";
            }
        };
        partyScreen.HandleUpdate(onSelected, onBack);


    }
    IEnumerator TryToRun()
    {
        state = BattleState.Busy;
        int playerSpeed = playerUnit.Pokemon.Speed;
        int enermySpeed = enermyUnit.Pokemon.Speed;
        escapeAttempts++;
        if (playerSpeed >= enermySpeed)
        {
            if (dialogBox.IsTyping() == true)
            {
                dialogBox.Stop();
            }
            dialogBox.Go();
            yield return dialogBox.TypeDialog($"Got Away Safely");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enermySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0,256) < f)
            {
                if (dialogBox.IsTyping() == true)
                {
                    dialogBox.Stop();
                }
                dialogBox.Go();
                yield return dialogBox.TypeDialog($"Got Away Safely");
                BattleOver(true);
            }
            else
            {
                if (dialogBox.IsTyping() == true)
                {
                    dialogBox.Stop();
                }
                dialogBox.Go();
                yield return dialogBox.TypeDialog($"Couldn't Escape");
            }
        }
        state = BattleState.RunningTurn;




    }
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {   
        if (playerUnit.Pokemon.HP > 0)
        {      
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayRecallAnimation();
            yield return new WaitForSeconds(2f);
        }
        int index1 = PartyPokemon.GetPlayerParty().Pokemons.IndexOf(playerUnit.Pokemon);
        int index2 = PartyPokemon.GetPlayerParty().Pokemons.IndexOf(newPokemon);
        PartyPokemon.GetPlayerParty().SwitchPartyOrder(index1, index2);
        playerUnit.SetUp(newPokemon);
        var playerCrySource = playerUnit.GetComponent<AudioSource>();
        playerCrySource.clip = playerUnit.Pokemon.Base.Cry;
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        playerUnit.EnterSecondAnimation();
        yield return new WaitForSeconds(0.2f);
        playerCrySource.Play();
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name} !");

        state = BattleState.RunningTurn;
    }
    IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon)
    {
        state = BattleState.Busy;
        enermyUnit.SetUp(nextPokemon);
        var enermyCrySource = enermyUnit.GetComponent<AudioSource>();
        enermyCrySource.clip = enermyUnit.Pokemon.Base.Cry;
        enermyUnit.EnterSecondAnimation();
        yield return new WaitForSeconds(0.2f);
        enermyCrySource.Play();
        yield return dialogBox.TypeDialog($"{trainer.TrainerName} sent out  {nextPokemon.Base.name}");
        state = BattleState.RunningTurn;
        
    }

    
    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);
        if (usedItem is PokeballItem)
        {

            yield return ThrowPokeball((PokeballItem)usedItem);
        }
        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        
        state = BattleState.Busy;
        dialogBox.EnableActionSelector(false);
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"you cant STEAL a traniers Pokemon!");
            state = BattleState.RunningTurn;
            yield break;
        }
        yield return dialogBox.TypeDialog($"{player.TrainerName}  Through a {pokeballItem.Name}");
       
        var prefab = pokeballItem.PokeballPrefab;
        var pokeballObject = Instantiate(prefab, playerUnit.transform.position - new Vector3(4, 0, 0), Quaternion.identity);
        var pokeball = pokeballObject.GetComponent<SpriteRenderer>();
        var pokeballAnim = pokeballObject.GetComponent<pokeballSprites>();
        pokeballAnim.SetUpThrow();
        SoundEffectAudio.PlayOneShot(pokeballThrow);
        yield return pokeball.transform.DOJump(enermyUnit.transform.GetChild(0).gameObject.transform.position + new Vector3(0, 2, 0), 2f,1,1).WaitForCompletion();
        pokeballAnim.SetUpCatch();
        SoundEffectAudio.PlayOneShot(pokeballCatch);
        yield return enermyUnit.PlayCaptureAnimation();
        yield return new WaitForSeconds(1.2f);
        yield return pokeball.transform.DOMoveY(pokeball.transform.position.y - 4.5f, 0.5f).WaitForCompletion();
        SoundEffectAudio.PlayOneShot(pokeballDrop);
        var catchRate = pokeballItem.CalculateCatchRate(turnCounter);
        int shakeCount = CatchChecker(enermyUnit.Pokemon, catchRate);
        for (int i=0; i < Mathf.Min(shakeCount,3); i++)
        {
            pokeballAnim.SetUpShake();
            SoundEffectAudio.PlayOneShot(pokeballShake);
            yield return new WaitUntil(() => pokeballAnim.Finished() == true);
            yield return new WaitForSeconds(0.5f);
        }
        if (shakeCount == 4)
        {
            SoundEffectAudio.PlayOneShot(pokeballCaught);
            pokeballAnim.Cought();
            yield return new WaitUntil(() => pokeballAnim.Finished() == true);
            musicAudioSource.clip = VictoryMusic;
            musicAudioSource.Play();
            yield return dialogBox.TypeDialog($"Gotcha!  {enermyUnit.Pokemon.Base.Name} was Caught!");
            playerParty.AddPokemon(enermyUnit.Pokemon);
            yield return new WaitForSeconds(1f);
            if(playerParty.Pokemons.Count < 6)
            {
                yield return dialogBox.TypeDialog($"{enermyUnit.Pokemon.Base.Name} was added to the party!");
            }
            else
            {
                yield return dialogBox.TypeDialog($"{enermyUnit.Pokemon.Base.Name} was sent to the PC");
            }
            
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            yield return HandleExpGain(enermyUnit);
            
            yield return enermyUnit.CoughtReset();
            Destroy(pokeballObject);
            BattleOver(true);

        }
        else
        {
            pokeballAnim.BreakOut();
            SoundEffectAudio.PlayOneShot(pokeballbreak);
            yield return enermyUnit.PlayBreakOutAnimation();
            pokeball.DOFade(0, 1f);

            if (shakeCount == 3)
            {
                yield return dialogBox.TypeDialog($"Shoot! It was so close too!");
            }
            else if(shakeCount == 2)
            {
                yield return dialogBox.TypeDialog($"Aargh! Almost had it!");
            }
            else if(shakeCount == 1)
            {
                yield return dialogBox.TypeDialog($"Darn! it appeared to be caught!");
            }
            else if (shakeCount == 0)
            {
                yield return dialogBox.TypeDialog($"Oh no! {enermyUnit.Pokemon.Base.Name} broke free!");
            }

        }
        Destroy(pokeballObject);
        state = BattleState.RunningTurn;

    }
    int CatchChecker(Pokemon pokemon, float catchRate)
    {
        Debug.Log(catchRate);
        if (catchRate >= 255)
        {
            Debug.Log("masterBall");
            return 4;
        }
        float statusBounus = ConditionsDB.GetStatusBounus(pokemon.Status);
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * catchRate  / (3 * pokemon.MaxHp) * statusBounus;
        if(a >= 255)
        {
            return 4;
        }
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));
        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if(UnityEngine.Random.Range(0,65535) >= b)
            {
                break;
            }
            ++shakeCount;
        }
        return shakeCount;
    }
    void MoveActionSelectionArrow()
    {
        if(currentActionX == 0 && currentActionY == 0)
        {
            selectionArrow.anchoredPosition = new Vector2(-136, 16);
            
            action = CurrentAction.Fight; 
        }
        else if (currentActionX == 0 && currentActionY == 1)
        {
            selectionArrow.anchoredPosition = new Vector2(-136, -24);
            action = CurrentAction.Pokemon;
        }
        else if (currentActionX == 1 && currentActionY == 0)
        {
            selectionArrow.transform.localPosition = new Vector3(18, 16);
            action = CurrentAction.Bag;
        }
        else if (currentActionX == 1 && currentActionY == 1)
        {
            selectionArrow.transform.localPosition = new Vector3(18, -24);
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

/*IEnumerator PlayerMove(bool playerFaster)
   {
       state = BattleState.RunningTurn;
       var move = playerUnit.Pokemon.Moves[(int)currentMove];

       yield return RunMove(playerUnit, enermyUnit, move);

       if(state == BattleState.RunningTurn)
       {
           if (playerFaster == true)
           {
               StartCoroutine(EnermyMove(true));
           }
           else if (playerFaster == false)
           {
               ActionSelection();
           }
       }
   }
   IEnumerator EnermyMove(bool playerFaster)
   {
       state = BattleState.RunningTurn;
       var move = enermyUnit.Pokemon.GetRandomMove();

       yield return RunMove(enermyUnit, playerUnit, move);

       if (state == BattleState.RunningTurn)
       {
           if (playerFaster == false)
           {
               StartCoroutine(PlayerMove(false));
           }
           else if(playerFaster == true) 
           {
               ActionSelection();
           }    
       }
   }*/

/*void WhoGoseFirst()
{
    if (playerUnit.Pokemon.Speed > enermyUnit.Pokemon.Speed)
    {
        StartCoroutine(PlayerMove(true));
    }
    else if (playerUnit.Pokemon.Speed < enermyUnit.Pokemon.Speed)
    {
        StartCoroutine(EnermyMove(false));
    }
    else
    {
        if (sameSpeedDecider == 1)
        {
            StartCoroutine(PlayerMove(true));
            sameSpeedDecider = 2;
        }
        else if (sameSpeedDecider == 2)
        {
            StartCoroutine(EnermyMove(false));
            sameSpeedDecider = 1;
        }
        else if(sameSpeedDecider < 0.5f)
        {
            StartCoroutine(PlayerMove(true));
            sameSpeedDecider = 2;
        }
        else if (sameSpeedDecider > 0.5f)
        {
            StartCoroutine(EnermyMove(false));
            sameSpeedDecider = 1;
        }

    }

}*/
