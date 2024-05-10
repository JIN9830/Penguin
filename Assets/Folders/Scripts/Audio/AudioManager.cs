using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, playerSfxSounds, uiSfxSound;
    public AudioSource musicSource, playerSfxSource, uiSfxSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if(s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlayerSFX(string name)
    {
        Sound s = Array.Find(playerSfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Player Sound Not Found");
        }
        else
        {
            playerSfxSource.PlayOneShot(s.clip);
        }
    }

    public void UISFX(string name)
    {
        Sound s = Array.Find(uiSfxSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("UI Sound Not Found");
        }
        else
        {
            uiSfxSource.PlayOneShot(s.clip);
        }
    }
}
