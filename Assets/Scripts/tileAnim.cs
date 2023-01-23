using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileAnim : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer theRenderer;
    [SerializeField]  float frameRate =0.8f;
    int currentFrame = 0;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        if(theRenderer == null)
        {
            theRenderer = GetComponent<SpriteRenderer>();
            currentFrame = 0;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
        if (sprites.Length != 0)
        {
            timer += Time.deltaTime;
            if (timer > frameRate)
            {
                currentFrame = (currentFrame + 1) % sprites.Length;
                theRenderer.sprite = sprites[currentFrame];
                timer -= frameRate;
            }
        }
    }
}
