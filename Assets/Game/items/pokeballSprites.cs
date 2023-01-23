using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pokeballSprites : MonoBehaviour
{
    [SerializeField] Sprite[] spinningSprites;
    [SerializeField] Sprite[] catchSprites;
    [SerializeField] Sprite[] breakOutSprites;
    [SerializeField] Sprite[] shakeSprites;
    [SerializeField] Sprite[] CoughtSprites;
    Sprite[] currentArrry;


    SpriteRenderer theRenderer;
    float frameRate;
    int currentFrame;
    float timer;
    bool finished= true;

    public bool Finished()
    {
        return finished;
    }
    public void Awake()
    {
        theRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetUpThrow()
    {
        frameRate = 0.25f;
        currentFrame =0;
        timer = 0;
        currentArrry = spinningSprites;
        theRenderer.sprite = currentArrry[0];
    }
    public void SetUpCatch()
    {
        frameRate = 0.14f;
        currentFrame = 0;
        timer = 0;
        currentArrry = catchSprites;
        theRenderer.sprite = currentArrry[0];
    }
    public void SetUpShake()
    {
        frameRate = 0.142f;
        currentFrame = 0;
        timer = 0;
        currentArrry = shakeSprites;
        theRenderer.sprite = currentArrry[0];
        finished = false;

    }
    public void BreakOut()
    {
        frameRate = 0.25f;
        currentFrame = 0;
        timer = 0;
        currentArrry = breakOutSprites;
        theRenderer.sprite = currentArrry[0];
        finished = false;
    }
    public void Cought()
    {
        frameRate = 0.18f;
        currentFrame = 0;
        timer = 0;
        currentArrry = CoughtSprites;
        theRenderer.sprite = currentArrry[0];
        finished = false;
    }

    public void Update()
    {   
       
            if (currentArrry.Length != 0 && currentFrame < currentArrry.Length - 1)
            {
                timer += Time.deltaTime;
                if (timer > frameRate)
                {
                    currentFrame = (currentFrame + 1) ;
                    theRenderer.sprite = currentArrry[currentFrame];
                    timer -= frameRate;
                }
            }
            else
            {
                finished = true;
            }
        
            
    }
    
}
/* int shakeCount = CatchChecker(enermyUnit.Pokemon);
        for (int i=0; i < Mathf.Min(shakeCount,3); i++)
        {
            yield return new WaitUntil(() => pokeballAnim.Finished() == true);
            yield return new WaitForSeconds(0.5f);
        }
        if (shakeCount == 4)
        {
            pokeballAnim.Cought();
        }


    }
    int CatchChecker(Pokemon pokemon)
    {
        float statusBounus = ConditionsDB.GetStatusBounus(pokemon.Status);
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * statusBounus / (3 * pokemon.MaxHp);
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
    */
