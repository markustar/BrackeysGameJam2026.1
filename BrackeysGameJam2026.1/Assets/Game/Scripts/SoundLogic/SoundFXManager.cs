using UnityEngine;

/// <summary>
/// Singleton class to play one or multiple different sounds. Works across scenes.
/// </summary>
public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject; //A Prefab (empty gameObject) that has an AudioSource as a component


    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
        
    }

    /// <summary>
    /// Plays a AudioClip.
    /// </summary>
    /// <param name="audioClip">the clip to play</param>
    /// <param name="spawnTransform">where the clip should be played</param>
    /// <param name="Volume">volume of the clip</param>
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform,float Volume)
    {
        if(audioClip == null) return;
        if (spawnTransform == null) return;

        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;
        //assign volume
        audioSource.volume = Volume;
        //play sound
        audioSource.Play();
        //getlenght of sfx clip
        float clipLenght = audioSource.clip.length;
        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLenght);
    }
    /// <summary>
    /// Plays a random AudioClip from a given AudioClip Array.
    /// </summary>
    /// <param name="audioClips">the array of AudioClips</param>
    /// <param name="spawnTransform">where the clip should be played</param>
    /// <param name="Volume">volume of the clip</param>
    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float Volume)
    {
        if (audioClips == null || audioClips.Length == 0 || spawnTransform == null) return;

        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClips[Random.Range(0,audioClips.Length)];
        //assign volume
        audioSource.volume = Volume;
        //play sound
        audioSource.Play();
        //getlenght of sfx clip
        float clipLenght = audioSource.clip.length;
        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLenght);
    }

    /// <summary>
    /// Plays a AudioClip and stops it after <paramref name="stoppingTime"/> seconds.
    /// </summary>
    /// <param name="audioClip">the clip to play</param>
    /// <param name="spawnTransform">where the clip should be played</param>
    /// <param name="Volume">volume of the clip</param>
    public void PlaySoundFXClipWithStoppingTime(AudioClip audioClip, Transform spawnTransform, float Volume, float stoppingTime)
    {
        if (audioClip == null || spawnTransform == null || soundFXObject == null) return;

        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;
        //assign volume
        audioSource.volume = Volume;
        //play sound
        audioSource.Play();

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, stoppingTime);
    }
}
