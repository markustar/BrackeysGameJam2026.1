using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
   
    bool isWaiting;
    void Awake()
    {
        
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
