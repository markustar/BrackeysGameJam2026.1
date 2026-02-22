using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern makes it easy to call GameManager.Instance from any other script
    public static GameManager Instance { get; private set; }

    [Header("Transition Settings")]
    [Tooltip("The Animator attached to your transition Canvas/Image.")]
    public Animator transitionAnimator;
    [Tooltip("The duration of the transition animation in seconds.")]
    public float transitionDuration = 2f;

    private void Awake()
    {
        // Ensure only one GameManager exists in the game
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (transitionAnimator != null)
            {
                DontDestroyOnLoad(transitionAnimator.transform.root.gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            LoadScene(1);
        }
    }


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }


    public void LoadScene(int sceneBuildIndex)
    {
        StartCoroutine(LoadSceneRoutine(sceneBuildIndex));
    }


    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Freeze the game
        Time.timeScale = 0f;

        if (transitionAnimator != null)
        {
            // Allow the animator to play while the game is paused
            transitionAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            transitionAnimator.SetTrigger("EndTransitionTrigger");
        }
        else
        {
            Debug.LogWarning("[GameManager] No Transition Animator assigned! Fading will be skipped.");
        }

        // Wait in real-time, unaffected by Time.timeScale
        yield return new WaitForSecondsRealtime(transitionDuration);

        SceneManager.LoadScene(sceneName);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransitionTrigger");
        }

        yield return new WaitForSecondsRealtime(transitionDuration);

        // Unfreeze the game after transition finishes
        Time.timeScale = 1f;
    }

    private IEnumerator LoadSceneRoutine(int sceneBuildIndex)
    {
        // Freeze the game
        Time.timeScale = 0f;

        if (transitionAnimator != null)
        {
            // Allow the animator to play while the game is paused
            transitionAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            transitionAnimator.SetTrigger("EndTransitionTrigger");
        }
        else
        {
            Debug.LogWarning("[GameManager] No Transition Animator assigned! Fading will be skipped.");
        }

        // Wait in real-time, unaffected by Time.timeScale
        yield return new WaitForSecondsRealtime(transitionDuration);

        SceneManager.LoadScene(sceneBuildIndex);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransitionTrigger");
        }

        yield return new WaitForSecondsRealtime(transitionDuration);

        // Unfreeze the game after transition finishes
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Call this to quit the application. Handles both Editor and standalone builds.
    /// Example: GameManager.Instance.QuitGame();
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[GameManager] Quitting Game...");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
