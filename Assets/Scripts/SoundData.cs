using UnityEngine;

[System.Serializable]
public class SoundData
{
    private AudioSource audioSource;
    public string name;
    public AudioClip clip;
    [Range(0.0f,1.0f)]
    public float volume;
   
    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    public void SetAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }
}

