using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMusicTrigger : MonoBehaviour
{
    [SerializeField] AudioClip zoneMusic;
    private void OnTriggerEnter(Collider other)
    {
        // Start is called before the first frame update
        if (other.gameObject.name == "Player")
        {
            SetMusic();
        }
    }
    public void SetMusic()
    {
        if (BackgroundMusic.i.BackgroundMusicSource().clip != zoneMusic)
        {
            BackgroundMusic.i.BackgroundMusicSource().clip = zoneMusic;
            BackgroundMusic.i.BackgroundMusicSource().Play();
        }


        else if (!BackgroundMusic.i.BackgroundMusicSource().isPlaying)
        {
            BackgroundMusic.i.BackgroundMusicSource().Play();
        }
    }
}
