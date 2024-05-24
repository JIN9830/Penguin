using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [field: SerializeField] public GameObject CurtainObject { get; private set; }

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
    }

    public void LoadIndexScene(int sceneIndex)
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1) // TODO: 인덱스 1번 씬에서도 DOTween을 사용한다면 코드의 조건문 수정이 필요함
        {
            DOTween.KillAll();
        }

        GameManager.GameManager_Instance.Initialize_CodingMethod();

        if(SceneManager.GetActiveScene().name == "Level Selection") // TODO: CodingUICanvas 게임 오브젝트의 On, Off를 변경하는 코드 (시티1에서 시티2로 넘어갈때 CodingUICanvas가 꺼질 수 있음 코드 수정이 필요함)
        {
            //GameManager.CodingUIManager_Instance.CodingUICanvas.SetActive(true);
        }
        else
        {
            GameManager.CodingUIManager_Instance.OpenOption();
            //GameManager.CodingUIManager_Instance.CodingUICanvas.SetActive(false);
        }

        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void LoadNextScene()
    {
        // TODO: 테스트용 프로토타입 코드
        if (SceneManager.GetActiveScene().buildIndex < 9)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
