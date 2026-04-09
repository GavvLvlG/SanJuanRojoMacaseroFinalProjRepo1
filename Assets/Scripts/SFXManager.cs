using UnityEngine;

[System.Serializable]
public class SFXData
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;

    // Not serialized: runtime-only
    private AudioSource audioSource;

    public void SetAudioSource(AudioSource src)
    {
        audioSource = src;
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }

    public AudioSource GetAudioSource() => audioSource;
}

public class SFXManager : MonoBehaviour
{
    public SFXData[] sfxDatas;

    private static SFXManager instance;

    private AudioSource sfxSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

       
        if (sfxDatas != null)
        {
            foreach (var s in sfxDatas)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                s.SetAudioSource(src);
            }
        }
    }


    public static void Play(string name)
    {
        if (instance == null)
        {
            Debug.LogWarning("SFXManager: No instance found in scene.");
            return;
        }

        if (string.IsNullOrEmpty(name))
            return;

        var entry = System.Array.Find(instance.sfxDatas, sd => sd.name == name);
        if (entry == null)
        {
            Debug.LogWarning($"SFXManager: SFX '{name}' not found.");
            return;
        }

        if (entry.clip == null)
        {
            Debug.LogWarning($"SFXManager: SFX '{name}' has no AudioClip assigned.");
            return;
        }

       
        instance.sfxSource.PlayOneShot(entry.clip, entry.volume);
    }

    public void PlaySfxInstance(string name)
    {
        Play(name);
    }
}
