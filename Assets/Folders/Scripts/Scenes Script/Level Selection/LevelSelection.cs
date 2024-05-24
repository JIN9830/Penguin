using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public Button saveClearButton;
    public Button[] levelsButton;

    private void Awake()
    {
        StageStatus();
    }

    private void Start()
    {
        // 레벨버튼에 각 씬의 인덱스값을 대입 하는 코드
        for (int i = 0; i < levelsButton.Length; i++)
        {
            int index = i;
            levelsButton[index].onClick.AddListener(() => GameSceneManager.Instance.LoadIndexScene(index + 1));
        }

        saveClearButton.onClick.AddListener(() =>{ PlayerPrefs.DeleteAll(); StageStatus(); });
    }

    public void StageStatus() // 플레이어의 스테이지 진행도에 따라서 각 레벨씬으로 이동 버튼의 활성화 상태를 업데이트하는 코드
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // 스테이지 세이브 파일 초기화 코드

        for (int i = 0; i < levelsButton.Length; i++)
        {
            levelsButton[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            levelsButton[i].interactable = true;
        }
    }
}
