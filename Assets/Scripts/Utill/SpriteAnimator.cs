using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;
    
    int currentFrame;
    float timer;
    

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.2f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }
    public void Start()
    {
        currentFrame = 0;
        timer = 0f;
        spriteRenderer.sprite = frames[0];


    }
    public void ReSet()
    {
        if(currentFrame == 0 || currentFrame == 3)
        {
            currentFrame = 0;
            spriteRenderer.sprite = frames[0];
        }
        else if (currentFrame == 1 || currentFrame == 2)
        {
            currentFrame = 2;
            spriteRenderer.sprite = frames[2];

        }
        
        timer = 0f;


    }
    public void HandleUpdate()
    {
        if(frames.Count != 0)
        {
            timer += Time.deltaTime;
            if (timer > frameRate)
            {
                currentFrame = (currentFrame + 1) % frames.Count;
                spriteRenderer.sprite = frames[currentFrame];
                timer -= frameRate;
            }
        }
        

    }
    public List<Sprite> Frames
    {
        get { return frames; }
    }


}
