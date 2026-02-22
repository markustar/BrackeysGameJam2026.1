
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
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
        
    }

    void WinConditionMet()
    {
        if(engineStart.HasStarted())
        {
          SceneManager.LoadScene("MainMenu");
        }
    }
}
