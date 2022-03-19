using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] bool isPlayerPokemon;

    [SerializeField] PokemonBase wildPokemon1;
    [SerializeField] PokemonBase wildPokemon2;

    public Image trainer = null;
    
    Image pokemonImage;
    Vector3 originalPos;
    Vector3 trainerPos;
    float animTimer;
    int currentFrame;
    Color originalColor;
    Sprite[] pokemonArray;
    Animator trainerAnimator;


    public Pokemon Pokemon { get; set; }

    public void Awake()
    {
        pokemonImage = GetComponent<Image>();
        originalPos = pokemonImage.transform.localPosition;
        if(trainer.gameObject.name == "PlayerBattleSprite")
        {
            trainerPos = trainer.transform.localPosition;
            
        }
        
    }
    public void SetUp(Pokemon pokemon)
    {



        Pokemon = pokemon;

        if (isPlayerPokemon)
        {
            pokemonImage.sprite = Pokemon.Base.BackSprite[0];
             pokemonArray = Pokemon.Base.BackSprite;
            pokemonImage.transform.localPosition = new Vector3(-700f, originalPos.y);
            pokemonImage.DOFade(255f, 0.2f);
        }
        else
        {
            pokemonImage.sprite = Pokemon.Base.FrontSprite[0];
            pokemonArray = Pokemon.Base.FrontSprite;
            pokemonImage.transform.localPosition = new Vector3(700f, originalPos.y);
            pokemonImage.DOFade(255f, 0.2f);
        }
        if (trainer.gameObject.name == "PlayerBattleSprite" )
        {
            trainer.DOFade(255f, 0.00001f);
        }
    }
    private void OnEnable()
    {
       
    }
    public void PlayerEnterAnimation()
    {

       
        if (trainer != null && trainer.gameObject.name == "PlayerBattleSprite")
        {
            trainerAnimator = trainer.gameObject.GetComponent<Animator>();
            trainerAnimator.SetBool("Start", true);
            var sequence = DOTween.Sequence();
            sequence.Append(trainer.DOFade(255f, 0.001f));
            sequence.Append(trainer.transform.DOLocalMoveX(-600, 2f));
            sequence.Append(trainer.DOFade(0f, 0.01f));
            sequence.Append(trainer.transform.DOLocalMoveX(-185, 0.1f));

        }
        
        if (isPlayerPokemon)
        {
            
            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1.5f);
        }
        else
        {
            
            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1f);
        }
        
        
    }
    public void PlayerEnterSecondAnimation()
    {
        trainer.DOFade(0f, 0.00001f);
        trainerAnimator.SetBool("Start", false);

        if (isPlayerPokemon)
        {

            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1.5f);
        }
        else
        {

            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1f);
        }


    }
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerPokemon)
        {
            sequence.Append(pokemonImage.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(pokemonImage.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }
        sequence.Append(pokemonImage.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(0f, 0.1f));
        sequence.Append(pokemonImage.DOFade(0f, 0.1f));
        sequence.Append(pokemonImage.DOFade(255f, 0.1f));
        sequence.Append(pokemonImage.DOFade(255f, 0.1f));
        sequence.Append(pokemonImage.DOFade(0f, 0.1f));
        sequence.Append(pokemonImage.DOFade(0f, 0.1f));
        sequence.Append(pokemonImage.DOFade(255f, 0.1f));
        sequence.Append(pokemonImage.DOFade(255f, 0.1f));
        sequence.Append(pokemonImage.DOFade(0f, 0.1f));
        sequence.Append(pokemonImage.DOFade(0f, 0.1f));
        sequence.Append(pokemonImage.DOFade(255f, 0.1f));
        sequence.Append(pokemonImage.DOFade(255f, 0.1f));








    }
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(0f, 0.2f));
        sequence.Join(pokemonImage.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        
    }

    void animateSprites()
    {
        
        animTimer += Time.deltaTime;
        if(animTimer > 0.08f)
        {
            
            animTimer -= 0.08f;
            ;
            if (currentFrame > pokemonArray.Length -1)
            {
                currentFrame = 0;
            }
            pokemonImage.sprite = pokemonArray[currentFrame];
            currentFrame++;


        }
                
        
    }

    private void Update()
    {
        animateSprites();
    }
}
