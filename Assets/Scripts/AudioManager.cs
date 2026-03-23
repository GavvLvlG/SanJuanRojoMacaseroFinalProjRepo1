using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{

    public SoundData[] soundDatas;

    void Awake()
    {
        foreach (SoundData sound in soundDatas)
        {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.clip = sound.clip;
            newAudioSource.volume = sound.volume;
            
            sound.SetAudioSource(newAudioSource);
        }
    }


    public void Play(string name)
    {
        SoundData curSound = Array.Find(soundDatas, soundDatas => soundDatas.name == name);

        curSound.GetAudioSource().Play();
    }
}
