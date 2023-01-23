using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] bool isPlayerUnit;
    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }
    [SerializeField] BattleHud hud;
    public BattleHud Hud
    {
        get { return hud; }
    }


    [SerializeField] Image statUpImage;
    [SerializeField] Image statDownImage;
    [SerializeField] Image statUpImageEnermy;
    [SerializeField] Image statDownImageEnermy;



    public Image trainer = null;
    
    Image pokemonImage;
    Vector3 originalPos;
    Vector3 trainerPos;
    float animTimer;
    int currentFrame;
    Color originalColor;
    Sprite[] pokemonArray;
    Animator trainerAnimator;
    float time = 0;




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

    public void BattleOver()
    {
       
            trainer.DOFade(255f, 0.00001f);
        
    }
    public void DisableHud()
    {
        hud.gameObject.SetActive(false);
    }
    public void Clear()
    {
        hud.ClearData();
    }
    public void SetUp(Pokemon pokemon)
    {



        Pokemon = pokemon;

        if (isPlayerUnit)
        {
            pokemonImage.sprite = Pokemon.Base.BackSprite[0];
            pokemonArray = Pokemon.Base.BackSprite;
            pokemonImage.transform.localPosition = new Vector3(-700f, originalPos.y);
           
        }
        else
        {
            pokemonImage.sprite = Pokemon.Base.FrontSprite[0];
            pokemonArray = Pokemon.Base.FrontSprite;
            trainerPos = trainer.transform.localPosition;
            Debug.Log("trainner pos " + trainerPos);
            pokemonImage.transform.localPosition = new Vector3(700f, originalPos.y);
            
        }
        transform.localScale = new Vector3(1, 1, 1);
        pokemonImage.DOFade(255f, 0.001f);
        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);
       
        
    }
    private void OnEnable()
    {
       
    }
    public void EnterAnimation()
    {     
        if (trainer != null && trainer.gameObject.name == "PlayerBattleSprite")
        {
            trainerAnimator = trainer.gameObject.GetComponent<Animator>();
            trainerAnimator.SetBool("Start", true);
            var sequence = DOTween.Sequence();
            sequence.Append(trainer.DOFade(255f, 0.001f));
            sequence.Append(trainer.transform.DOLocalMoveX(-510, 2f));
            sequence.Append(trainer.DOFade(0f, 0.01f));
            sequence.Append(trainer.transform.DOLocalMoveX(-195, 0.1f));           
        }      
        if (isPlayerUnit)
        {           
            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1.5f);
        }
        else
        {          
            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1f);
        }     
    }
    public void TrainnerDefetedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(trainer.transform.DOLocalMoveX(510, 0.001f));
        sequence.Append(trainer.DOFade(255f, 0.001f));
        sequence.Append(trainer.transform.DOLocalMoveX(trainerPos.x, 1.5f));
    }
    public void TrainerEnterAnimation()
    { 
        var sequence = DOTween.Sequence();
        sequence.Append(trainer.DOFade(255f, 0.001f));
        sequence.Append(trainer.transform.DOLocalMoveX(510, 1.5f));
        sequence.Append(trainer.DOFade(0f, 0.01f));
        sequence.Append(trainer.transform.DOLocalMoveX(195, 0.1f));     
    }
    public void TrainnerPokemonEnter()
    {
        pokemonImage.transform.DOLocalMoveX(originalPos.x, 1.5f);
    }

    public void EnterSecondAnimation()
    {

        Debug.Log("here");
       
        if (isPlayerUnit)
        {
            trainer.DOFade(0f, 0.00001f);
            trainerAnimator.SetBool("Start", false);
            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1.5f);
        }
        else
        {

            pokemonImage.transform.DOLocalMoveX(originalPos.x, 1.5f);
        }

    }
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(0f, 0.2f));
        sequence.Join(pokemonImage.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));

    }
    public void PlayRecallAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(0f, 1f));
        sequence.Join(pokemonImage.transform.DOLocalMoveX(-700f, 1f));

    }
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(pokemonImage.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(pokemonImage.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }
        sequence.Append(pokemonImage.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }
    public void PlayStatAnimation(MoveEffects effects, MoveTarget moveTarget)
    {
        var sequence = DOTween.Sequence();
        var statImag = statUpImage;
        if(effects.Boosts.Count != 0)
        {
            if (moveTarget == MoveTarget.self)
            {
                if (effects.Boosts[0].boost > 0)
                {
                    statImag = statUpImage;
                    time = 0;
                    StartCoroutine(AnimateStat(statImag, true));
                }
                else if (effects.Boosts[0].boost < 0)
                {
                    statImag = statDownImage;
                    time = 0;
                    StartCoroutine(AnimateStat(statImag, false));
                }
                sequence.Append(statImag.material.DOFade(0f, 0.01f));
                sequence.Append(statImag.DOFade(255f, 0.01f));
                //sequence.Append(statImag.DOFade(30, 0.01f));
                //sequence.Append(statImag.DOFade(30, 0.9f));
                sequence.Append(statImag.material.DOFade(0.7f, 0.2f));
                sequence.AppendInterval(0.7f);
                sequence.Append(statImag.material.DOFade(0f, 0.2f));
                sequence.Append(statImag.DOFade(0, 0.01f));
                sequence.Append(statImag.material.DOFade(1f, 0.01f));
            }
            else if (moveTarget == MoveTarget.foe)
            {
                if (effects.Boosts[0].boost > 0)
                {
                    statImag = statUpImageEnermy;
                    time = 0;
                    StartCoroutine(AnimateStat(statImag, true));
                }
                else if (effects.Boosts[0].boost < 0)
                {
                    statImag = statDownImageEnermy;
                    time = 0;
                    StartCoroutine(AnimateStat(statImag, false));
                }
                sequence.Append(statImag.material.DOFade(0f, 0.01f));
                sequence.Append(statImag.DOFade(255f, 0.01f));
                sequence.Append(statImag.material.DOFade(0.7f, 0.2f));
                sequence.Append(statImag.material.DOFade(0.7f, 0.7f));
                sequence.Append(statImag.material.DOFade(0f, 0.2f));
                sequence.Append(statImag.DOFade(0f, 0.01f));
                sequence.Append(statImag.material.DOFade(1f, 0.01f));
            }

        }

    }
    IEnumerator AnimateStat(Image image, bool up)
    {

        var renderMaterial = image.materialForRendering;
        while (time < 1.2f)
        {
            time += Time.deltaTime;

            if (up == true)
            {
                renderMaterial.mainTextureOffset -= new Vector2(0, 0.5f * Time.deltaTime);
            }
            else
            {
                renderMaterial.mainTextureOffset += new Vector2(0, 0.5f * Time.deltaTime);
            }
            
            yield return null;
        }
        renderMaterial.mainTextureOffset = new Vector2(0, 0);

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
    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(0f, 0.5f));
        sequence.Join(transform.DOLocalMove(originalPos + new Vector3(80,130,0), 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f,0.3f,1f), 0.5f));
        yield return sequence.WaitForCompletion();

    }
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(255f, 0.5f));
        sequence.Join(transform.DOLocalMove(originalPos, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    public IEnumerator CoughtReset()
    {
        var sequence = DOTween.Sequence();
        sequence.Join(transform.DOLocalMove(originalPos, 0.001f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.001f));
        Debug.Log("hi");
        yield return sequence;


    }


    void animateSprites()
    {
        if (Pokemon.Status.Name == "Sleep" || Pokemon.Status.Name == "Freeze")
        {
            return;
        }
        else
        {
            animTimer += Time.deltaTime;
        } 
        if (Pokemon.Status.Name == "Paralysis" || Pokemon.HP < Pokemon.MaxHp / 5)
        {
            if (animTimer > 0.16f)
            {

                animTimer -= 0.16f;
                ;
                if (currentFrame > pokemonArray.Length - 1)
                {
                    currentFrame = 0;
                }
                pokemonImage.sprite = pokemonArray[currentFrame];
                currentFrame++;
            }
        }
        else
        {
            if (animTimer > 0.08f)
            {

                animTimer -= 0.08f;
                ;
                if (currentFrame > pokemonArray.Length - 1)
                {
                    currentFrame = 0;
                }
                pokemonImage.sprite = pokemonArray[currentFrame];
                currentFrame++;
            }
        }
        
                
        
    }

    private void Update()
    {
        animateSprites();
    }
}
