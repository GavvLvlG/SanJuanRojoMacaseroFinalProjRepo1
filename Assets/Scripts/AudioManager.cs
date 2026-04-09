using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
       public SoundData[] soundDatas;

    [Header("Menu Audio")]
    [Tooltip("Names of scenes that should play the shared menu music. Case-sensitive and must match Unity scene names.")]
    public string[] menuSceneNames = new string[] { "Title", "MainMenu", "Credits" };

    [Tooltip("Optional: assign a single AudioSource that will play for both Title and Menu scenes. If provided, this source will be played on menu scenes and stopped on gameplay scenes.")]
    public AudioSource menuMusicSource;

    [Tooltip("Fallback SoundData name to use if no menuMusicSource is assigned. (optional)")]
    public string menuMusicName;

    [Header("Stop Music On Scenes")]
    [Tooltip("Names of scenes where the menu music should be stopped immediately. Case-insensitive.")]
    public string[] stopMusicSceneNames = new string[] { "Gameplay" };

    private static AudioManager instance;

   
    private AudioSource persistentMenuSource;

    // Public accessor for the singleton
    public static AudioManager Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
           
            if (menuMusicSource != null)
            {
               
                if (menuMusicSource.gameObject != gameObject)
                {
                    persistentMenuSource = gameObject.AddComponent<AudioSource>();
                    persistentMenuSource.clip = menuMusicSource.clip;
                    persistentMenuSource.volume = menuMusicSource.volume;
                    persistentMenuSource.pitch = menuMusicSource.pitch;
                    persistentMenuSource.loop = menuMusicSource.loop;
                    persistentMenuSource.playOnAwake = false;
                    persistentMenuSource.spatialBlend = menuMusicSource.spatialBlend;

                    
                    menuMusicSource = persistentMenuSource;
                }
                else
                {
                    
                    persistentMenuSource = menuMusicSource;
                }
            }

         
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void OnDestroy()
    {
       
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
        if (stopMusicSceneNames != null)
        {
            foreach (var sname in stopMusicSceneNames)
            {
                if (string.Equals(scene.name?.Trim(), sname?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"AudioManager: OnSceneLoaded('{scene.name}') -> stop-music scene. Stopping menu music immediately.");

              
                    if (menuMusicSource != null && menuMusicSource.isPlaying)
                        menuMusicSource.Stop();

                   
                    if (menuMusicSource != null && menuMusicSource.clip != null)
                    {
                        var allSources = FindObjectsOfType<AudioSource>();
                        foreach (var src in allSources)
                        {
                            if (src.clip == menuMusicSource.clip && src.isPlaying)
                                src.Stop();
                        }
                    }

                  
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

                    return; 
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
          
            if (menuMusicSource != null && menuMusicSource.isPlaying)
                menuMusicSource.Stop();

         
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

     public void Play(string name)
    {
        if (soundDatas == null)
            return;
        SoundData curSound = Array.Find(soundDatas, s => s.name == name);
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
