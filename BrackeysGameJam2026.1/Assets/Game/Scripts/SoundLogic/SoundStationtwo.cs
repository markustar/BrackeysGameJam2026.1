using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStationtwo : MonoBehaviour
{
    [SerializeField] private AudioClip[] EeeriSounds;
    [SerializeField] private AudioClip flicker;
    private bool isPlaying;

    void Start()
    {
        DontDestroyOnLoad(gameObject); //Play station SFX across scenes. This line can be removed if desired.
        StartCoroutine(PlayRandomSound());
        isPlaying = false;
    }

    void LateUpdate()
    {
        if(!isPlaying)
        {
            PlayFlickerSound();
        }
    }

    private IEnumerator PlayRandomSound()
    {
        while (true)
        {
            //Play a station announcement with a certain chance
            var randomNum = Random.Range(0, 100);
            if (randomNum < 40)
            {
                SoundFXManager.instance.PlayRandomSoundFXClip(EeeriSounds, transform, Random.Range(.3f, .4f));
            }
            
            yield return new WaitForSeconds(Random.Range(15, 30));
        }
    }

    void PlayFlickerSound()
    {
        SoundFXManager.instance.PlaySoundFXClip(flicker, this.transform, Random.Range(0.3f,0.4f));
        isPlaying = true;
    }
}
