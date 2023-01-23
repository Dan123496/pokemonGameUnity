using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneMusic { PalletTown , Route1, Viridian, Route10, none}
public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] AudioSource backgroundMusicSource;
    

    public static BackgroundMusic i { get; set; }

    private void Awake()
    {
        i = this;
        
    }
   public AudioSource BackgroundMusicSource()
    {
        return backgroundMusicSource;
    }
 
}

