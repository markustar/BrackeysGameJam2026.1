using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance;
    bool isWaiting;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   public void StopTime(float duration)
    {   
        if(isWaiting) return;

        Time.timeScale = 0f;
        StartCoroutine(TimeStopWait(duration));
    }

    IEnumerator TimeStopWait(float duration)
    {   
        isWaiting = true;
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        isWaiting = false;
    }
}
