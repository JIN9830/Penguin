using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] backgorundSounds, musicSounds, uiSfxSound, playerSfxSounds;
    private Sound _background, _music, _uiSfx, _playerSfx;
    public AudioSource backgroundSource, musicSource, playerSfxSource, uiSfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadVolume();
    }

    public void Play_Background(string name)
    {
        _background = null;
        _background = Array.Find(backgorundSounds, x => x.name == name);

        if( _background == null )
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            backgroundSource.clip = _background.clip;
            backgroundSource.Play();
        }
    }

    public void Play_Music(string name)
    {
        _music = null;
        _music = Array.Find(musicSounds, x => x.name == name);

        if (_music == null)
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
        
        _playerSfx = null;
        _playerSfx = Array.Find(playerSfxSounds, x => x.name == name);


        if (_playerSfx == null)
        {
            Debug.Log("Player Sound Not Found");
        }
        else
        {
            playerSfxSource.PlayOneShot(_playerSfx.clip);
        }
    }

    public void Play_UISFX(string name)
    {
        _uiSfx = null;
        _uiSfx = Array.Find(uiSfxSound, x => x.name == name);

        if (_uiSfx == null)
        {
            Debug.Log("UI Sound Not Found");
        }
        else
        {
            uiSfxSource.PlayOneShot(_uiSfx.clip);
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
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void UISFXVolume(float volume)
    {
        uiSfxSource.volume = volume;
        PlayerPrefs.SetFloat("uiSFXVolume", volume);
    }
    public void PlayerSFXVolume(float volume)
    {
        playerSfxSource.volume = volume;
        PlayerPrefs.SetFloat("playerSFXVolume", volume);
    }


    public void LoadVolume()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
            musicSource.volume = PlayerPrefs.GetFloat("musicVolume");

        if (PlayerPrefs.HasKey("uiSFXVolume"))
            uiSfxSource.volume = PlayerPrefs.GetFloat("uiSFXVolume");

        if (PlayerPrefs.HasKey("playerSFXVolume"))
            playerSfxSource.volume = PlayerPrefs.GetFloat("playerSFXVolume");
    }
}
