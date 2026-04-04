using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stops (or optionally destroys) an AudioSource attached to this GameObject
/// when a configured scene is loaded (e.g., the gameplay scene).
/// Attach to the GameObject that holds the music you want to stop on gameplay.
/// </summary>
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

    // Public methods intended to be called from UI Buttons in the Inspector.
    // Examples:
    // - Add a Button, drag the GameObject with this component into the OnClick slot,
    //   and select SceneChanger.LoadMainMenu or LoadGameplay, or LoadSceneByName.

    /// <summary>
    /// Load a scene by its name. Use this for Buttons that pass the scene name.
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("SceneChanger: LoadSceneByName called with empty sceneName.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a scene by its build index.
    /// </summary>
    public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    /// <summary>
    /// Convenience: load the configured main menu scene name.
    /// </summary>
    public void LoadMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogWarning("SceneChanger: mainMenuSceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Convenience: load the configured gameplay scene name.
    /// </summary>
    public void LoadGameplay()
    {
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogWarning("SceneChanger: gameplaySceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Convenience: load the configured title scene name.
    /// </summary>
    public void LoadTitle()
    {
        if (string.IsNullOrWhiteSpace(titleSceneName))
        {
            Debug.LogWarning("SceneChanger: titleSceneName is empty. Use LoadSceneByName instead or set the name in the inspector.");
            return;
        }

        SceneManager.LoadScene(titleSceneName);
    }

    /// <summary>
    /// Quit the application (no-op in the editor).
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}