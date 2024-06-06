using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUIController : MonoBehaviour
{
    [field: SerializeField] public Slider musicSlider, uiSfxSlider, playerSfxSlider;

    public void Start()
    {
        musicSlider.value = AudioManager.Instance.musicSource.volume;

        uiSfxSlider.value = AudioManager.Instance.uiSfxSource.volume;

        playerSfxSlider.value = AudioManager.Instance.playerSfxSource.volume;
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ToggleUISFX()
    {
        AudioManager.Instance.ToggleUISFX();
    }

    public void TogglePlayerSFX()
    {
        AudioManager.Instance.TogglePlayerSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicSlider.value);
    }

    public void UISFXVolume()
    {
        AudioManager.Instance.UISFXVolume(uiSfxSlider.value);
    }

    public void PlayerSFXVolume()
    {
        AudioManager.Instance.PlayerSFXVolume(playerSfxSlider.value);
    }

    public void ChangeFPS()
    {
        if(Application.targetFrameRate >= 120)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = 120;
        }
    }
}
