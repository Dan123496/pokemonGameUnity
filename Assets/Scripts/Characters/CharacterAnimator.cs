using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> runDownSprites;
    [SerializeField] List<Sprite> runUpSprites;
    [SerializeField] List<Sprite> runLeftSprites;
    [SerializeField] List<Sprite> runRightSprites;
    [SerializeField] MoveDirection direction = MoveDirection.Down;

    public float MoveX { get; set; }
    public float MoveY { get; set; }
      public bool IsMoving { get; set; }
    public bool IsRunning { get; set; }
    

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator runDownAnim;
    SpriteAnimator runUpAnim;
    SpriteAnimator runLeftAnim;
    SpriteAnimator runRightAnim;

    SpriteAnimator currentAnim;
    bool wasPrevMoving;

    SpriteRenderer spriteRenderer;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        runDownAnim = new SpriteAnimator(runDownSprites, spriteRenderer, 0.18f);
        runUpAnim = new SpriteAnimator(runUpSprites, spriteRenderer, 0.18f);
        runLeftAnim = new SpriteAnimator(runLeftSprites, spriteRenderer, 0.18f);
        runRightAnim = new SpriteAnimator(runRightSprites, spriteRenderer, 0.18f);
        SetFacingDirection(direction);
        currentAnim = walkDownAnim;
    }
    public void Update()
    {


        var prevAnim = currentAnim;
        if (IsRunning && IsMoving)
        {
            if (MoveX == 1)
            {
                currentAnim = runRightAnim;
            }
            else if (MoveX == -1)
            {
                currentAnim = runLeftAnim;
            }
            else if (MoveY == 1)
            {
                currentAnim = runUpAnim;
            }
            else if (MoveY == -1)
            {
                currentAnim = runDownAnim;
            }
            
        }
        else
        {
            if (MoveX == 1)
            {
                currentAnim = walkRightAnim;
            }
            else if (MoveX == -1)
            {
                currentAnim = walkLeftAnim;
            }
            else if (MoveY == 1)
            {
                currentAnim = walkUpAnim;
            }
            else if (MoveY == -1)
            {
                currentAnim = walkDownAnim;
            }
        }

        if(currentAnim != prevAnim || IsMoving != wasPrevMoving)
        {
            currentAnim.ReSet();
        }

        if (IsMoving)
        {
            currentAnim.HandleUpdate();
        }
        else
        {
            currentAnim.ReSet();
        }
        wasPrevMoving = IsMoving;
    }
    public void SetFacingDirection(MoveDirection dir)
    {
        switch (dir)
        {
            case MoveDirection.Right:
                MoveX = 1;
                MoveY = 0;
                break;
            case MoveDirection.Left:
                MoveX = -1;
                MoveY = 0;
                break;
            case MoveDirection.Up:
                MoveY = 1;
                MoveX = 0;
                break;
            case MoveDirection.Down:
                MoveY = -1;
                MoveX = 0;
                break;

        }

    }
    public MoveDirection FacingDirection
    {
        get => direction;
    }
}
