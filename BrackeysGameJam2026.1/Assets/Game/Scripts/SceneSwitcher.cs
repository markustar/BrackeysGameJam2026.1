
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitcher : MonoBehaviour
{
   EngineStart engineStart;

    void Awake()
    {
        engineStart = FindFirstObjectByType<EngineStart>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WinConditionMet());
    }

    IEnumerator WinConditionMet()
    {   
        yield return new WaitForSeconds(4f);

        if(engineStart.HasStarted())
        {
          SceneManager.LoadScene("MainMenu");
        }

        yield return new WaitForSeconds(4f);
       
    }
}
