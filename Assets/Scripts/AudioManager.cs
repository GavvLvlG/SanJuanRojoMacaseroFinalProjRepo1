using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    // Optional legacy SoundData list — kept for compatibility
    public SoundData[] soundDatas;

    [Header("Menu Audio")]
    [Tooltip("Names of scenes that should play the shared menu music. Case-sensitive and must match Unity scene names.")]
    public string[] menuSceneNames = new string[] { "Title", "MainMenu" };

    [Tooltip("Optional: assign a single AudioSource that will play for both Title and Menu scenes. If provided, this source will be played on menu scenes and stopped on gameplay scenes.")]
    public AudioSource menuMusicSource;

    [Tooltip("Fallback SoundData name to use if no menuMusicSource is assigned. (optional)")]
    public string menuMusicName;

    [Header("Stop Music On Scenes")]
    [Tooltip("Names of scenes where the menu music should be stopped immediately. Case-insensitive.")]
    public string[] stopMusicSceneNames = new string[] { "Gameplay" };

    private static AudioManager instance;

    // Persistent AudioSource created on the AudioManager GameObject to ensure
    // menu music continues across scene loads even if the inspector-assigned
    // AudioSource belonged to a scene object that gets destroyed.
    private AudioSource persistentMenuSource;

    void Awake()
    {
        // singleton behavior
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // initialize SoundData audio sources if provided
        if (soundDatas != null)
        {
            foreach (SoundData sound in soundDatas)
            {
                AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
                newAudioSource.clip = sound.clip;
                newAudioSource.volume = sound.volume;
                newAudioSource.loop = false;
                newAudioSource.playOnAwake = false;

                sound.SetAudioSource(newAudioSource);
            }
        }

        // Create a persistent AudioSource on this GameObject to play menu music
        persistentMenuSource = gameObject.AddComponent<AudioSource>();
        persistentMenuSource.playOnAwake = false;

        // If the user assigned an AudioSource from a scene GameObject in the inspector,
        // copy its clip/volume to the persistent source so it will survive scene changes.
        if (menuMusicSource != null)
        {
            if (menuMusicSource.clip != null)
            {
                persistentMenuSource.clip = menuMusicSource.clip;
                persistentMenuSource.volume = menuMusicSource.volume;
            }

            // Redirect to persistent source so we don't reference a scene-bound AudioSource
            menuMusicSource = persistentMenuSource;
        }
        else
        {
            // If no inspector AudioSource assigned but a SoundData name is set, try to use its clip
            if (!string.IsNullOrEmpty(menuMusicName) && soundDatas != null)
            {
                SoundData sd = Array.Find(soundDatas, s => s.name == menuMusicName);
                if (sd != null && sd.clip != null)
                {
                    persistentMenuSource.clip = sd.clip;
                    persistentMenuSource.volume = sd.volume;
                    menuMusicSource = persistentMenuSource;
                }
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the loaded scene is explicitly configured to stop music, do that first.
        if (stopMusicSceneNames != null)
        {
            foreach (var sname in stopMusicSceneNames)
            {
                if (string.Equals(scene.name?.Trim(), sname?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"AudioManager: OnSceneLoaded('{scene.name}') -> stop-music scene. Stopping menu music immediately.");

                    // Stop the persistent/preset menu source
                    if (menuMusicSource != null && menuMusicSource.isPlaying)
                        menuMusicSource.Stop();

                    // Stop any AudioSource playing the same clip
                    if (menuMusicSource != null && menuMusicSource.clip != null)
                    {
                        var allSources = FindObjectsOfType<AudioSource>();
                        foreach (var src in allSources)
                        {
                            if (src.clip == menuMusicSource.clip && src.isPlaying)
                                src.Stop();
                        }
                    }

                    // Also stop SoundData-based menu music if configured
                    if (!string.IsNullOrEmpty(menuMusicName) && soundDatas != null)
                    {
                        SoundData sd = Array.Find(soundDatas, s => s.name == menuMusicName);
                        if (sd != null)
                        {
                            var src = sd.GetAudioSource();
                            if (src != null && src.isPlaying)
                                src.Stop();
                        }
                    }

                    return; // done
                }
            }
        }

        bool isMenu = false;
        if (menuSceneNames != null)
        {
            foreach (var name in menuSceneNames)
            {
                if (string.Equals(scene.name?.Trim(), name?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    isMenu = true;
                    break;
                }
            }
        }

        if (isMenu)
        {
            Debug.Log($"AudioManager: OnSceneLoaded('{scene.name}') -> menu scene. Playing menu music.");

            // If a SoundData name is configured but no AudioSource was assigned in the inspector,
            // try to use the SoundData's generated AudioSource so the music persists across scenes.
            if (menuMusicSource == null && !string.IsNullOrEmpty(menuMusicName) && soundDatas != null)
            {
                SoundData sd = Array.Find(soundDatas, s => s.name == menuMusicName);
                if (sd != null)
                {
                    var generated = sd.GetAudioSource();
                    if (generated != null)
                        menuMusicSource = generated;
                }
            }

            // Play the shared menu source if assigned
            if (menuMusicSource != null)
            {
                if (!menuMusicSource.isPlaying)
                {
                    menuMusicSource.loop = true;
                    menuMusicSource.Play();
                }
            }
            else if (!string.IsNullOrEmpty(menuMusicName) && soundDatas != null)
            {
                // fallback: play by SoundData name
                SoundData sd = Array.Find(soundDatas, s => s.name == menuMusicName);
                if (sd != null)
                {
                    var src = sd.GetAudioSource();
                    if (src != null)
                    {
                        src.loop = true;
                        if (!src.isPlaying)
                            src.Play();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"AudioManager: No menu music configured for scene '{scene.name}'. Assign `menuMusicSource` or `menuMusicName` in the inspector.");
            }
        }
        else
        {
            Debug.Log($"AudioManager: OnSceneLoaded('{scene.name}') -> gameplay scene. Stopping menu music.");
            // Entering gameplay: stop the shared menu music so gameplay audio can run separately
            if (menuMusicSource != null && menuMusicSource.isPlaying)
                menuMusicSource.Stop();

            // Also defensively stop any AudioSource that is playing the same clip as the menuMusicSource
            if (menuMusicSource != null && menuMusicSource.clip != null)
            {
                var allSources = FindObjectsOfType<AudioSource>();
                foreach (var src in allSources)
                {
                    if (src == menuMusicSource)
                        continue;

                    if (src.clip == menuMusicSource.clip && src.isPlaying)
                        src.Stop();
                }
            }

            // Also stop SoundData-based menu music if configured
            if (!string.IsNullOrEmpty(menuMusicName) && soundDatas != null)
            {
                SoundData sd = Array.Find(soundDatas, s => s.name == menuMusicName);
                if (sd != null)
                {
                    var src = sd.GetAudioSource();
                    if (src != null && src.isPlaying)
                        src.Stop();
                }
            }
        }
    }

    // Simple Play helper (keeps compatibility)
    public void Play(string name)
    {
        if (soundDatas == null)
            return;

        SoundData curSound = Array.Find(soundDatas, soundDatas => soundDatas.name == name);
        if (curSound == null)
        {
            Debug.LogWarning($"AudioManager: sound '{name}' not found.");
            return;
        }

        var src = curSound.GetAudioSource();
        if (src == null)
        {
            Debug.LogWarning($"AudioManager: AudioSource for sound '{name}' is null.");
            return;
        }

        src.loop = false;
        src.Play();
    }
}
