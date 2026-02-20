using System.Collections;
using UnityEngine;

public class StationAnnouncementManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] announcements;

    void Start()
    {
        DontDestroyOnLoad(gameObject); //Play station SFX across scenes. This line can be removed if desired.
        StartCoroutine(PlayStationAnnouncementsRandom());
    }

    private IEnumerator PlayStationAnnouncementsRandom()
    {
        while (true)
        {
            //Play a station announcement with a certain chance
            var randomNum = Random.Range(0, 100);
            if (randomNum < 40)
            {
                SoundFXManager.instance.PlayRandomSoundFXClip(announcements, transform, Random.Range(.3f, .4f));
            }
            
            yield return new WaitForSeconds(Random.Range(15, 30));
        }
    }

}
