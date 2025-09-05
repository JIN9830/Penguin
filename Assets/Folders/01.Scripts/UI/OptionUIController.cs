using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 내 옵션 UI(사운드, FPS 등)를 제어하는 클래스입니다.
/// </summary>
public class OptionUIController : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider uiSfxSlider;
    [SerializeField] private Slider playerSfxSlider;

    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        if (_audioManager == null)
        {
            Debug.LogError("AudioManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 슬라이더 초기 값 설정
        musicSlider.value = _audioManager.musicSource.volume;
        uiSfxSlider.value = _audioManager.uiSfxSource.volume;
        playerSfxSlider.value = _audioManager.playerSfxSource.volume;

        // 슬라이더 이벤트 리스너 등록
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        uiSfxSlider.onValueChanged.AddListener(OnUISFXVolumeChanged);
        playerSfxSlider.onValueChanged.AddListener(OnPlayerSFXVolumeChanged);
    }

    public void ToggleMusic()
    {
        _audioManager.ToggleMusic();
    }

    public void ToggleUISFX()
    {
        _audioManager.ToggleUISFX();
    }

    public void TogglePlayerSFX()
    {
        _audioManager.TogglePlayerSFX();
    }

    public void OnMusicVolumeChanged(float value)
    {
        _audioManager.MusicVolume(value);
    }

    public void OnUISFXVolumeChanged(float value)
    {
        _audioManager.UISFXVolume(value);
    }

    public void OnPlayerSFXVolumeChanged(float value)
    {
        _audioManager.PlayerSFXVolume(value);
    }

    public void ChangeFPS()
    {
        Application.targetFrameRate = (Application.targetFrameRate >= 120) ? 60 : 120;
    }
}
