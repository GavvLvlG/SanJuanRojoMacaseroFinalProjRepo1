using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    [Tooltip("Name of the scene that should stop this object's AudioSource (case-sensitive by default).")]
    public string targetSceneName = "GamePlay";

    [Tooltip("If true, destroy the GameObject when the target scene loads. Otherwise just stop the AudioSource.")]
    public bool destroyOnTargetScene = false;

    [Header("Button helper scene names")]
    [Tooltip("Optional: scene name to use for a 'Main Menu' button. Leave empty to call LoadSceneByName from the Button instead.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Optional: scene name to use for a 'Gameplay' button. Leave empty to call LoadSceneByName from the Button instead.")]
    public string gameplaySceneName = "GamePlay";
    
    [Tooltip("Optional: scene name to use for a 'Title' button. Leave empty to call LoadSceneByName from the Button instead.")]
    public string titleSceneName = "Title";

    [Tooltip("Optional: scene name to use for a 'Credits' button. Leave empty to call LoadSceneByName from the Button instead.")]
    public string creditsSceneName = "Credits";

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.Equals(scene.name?.Trim(), targetSceneName?.Trim(), StringComparison.Ordinal))
        {
            var src = GetComponent<AudioSource>();
            if (src != null && src.isPlaying)
                src.Stop();

            if (destroyOnTargetScene)
                Destroy(gameObject);
        }
    }

        public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("SceneChanger: LoadSceneByName called with empty sceneName.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

       public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

  
    public void LoadMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogWarning("SceneChanger: mainMenuSceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

       public void LoadGameplay()
    {
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogWarning("SceneChanger: gameplaySceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void LoadTitle()
    {
        if (string.IsNullOrWhiteSpace(titleSceneName))
        {
            Debug.LogWarning("SceneChanger: titleSceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(titleSceneName);
    }

  
    public void LoadCredits()
    {
        if (string.IsNullOrWhiteSpace(creditsSceneName))
        {
            Debug.LogWarning("SceneChanger: creditsSceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(creditsSceneName);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}