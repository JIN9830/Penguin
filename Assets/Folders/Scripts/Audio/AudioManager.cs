using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, uiSfxSound, playerSfxSounds;
    private Sound _music, _uiSfx, _playerSfx;
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

    public void Play_Music(string name)
    {
        _music = null;
        _music = Array.Find(musicSounds, x => x.name == name);

        if(_music == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = _music.clip;
            musicSource.Play();
        }
    }

    public void Play_PlayerSFX(string name)
    {
        _uiSfx = null;
        _uiSfx = Array.Find(playerSfxSounds, x => x.name == name);

        if (_uiSfx == null)
        {
            Debug.Log("Player Sound Not Found");
        }
        else
        {
            playerSfxSource.PlayOneShot(_uiSfx.clip);
        }
    }

    public void Play_UISFX(string name)
    {
        _playerSfx = null;
        _playerSfx = Array.Find(uiSfxSound, x => x.name == name);

        if (_playerSfx == null)
        {
            Debug.Log("UI Sound Not Found");
        }
        else
        {
            uiSfxSource.PlayOneShot(_playerSfx.clip);
        }
    }




    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleUISFX()
    {
        uiSfxSource.mute = !uiSfxSource.mute;
    }
    public void TogglePlayerSFX()
    {
        playerSfxSource.mute = !playerSfxSource.mute;
    }




    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void UISFXVolume(float volume)
    {
        uiSfxSource.volume = volume;
    }
    public void PlayerSFXVolume(float volume)
    {
        playerSfxSource.volume = volume;
    }
}
