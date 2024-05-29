using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [field: SerializeField] public GameObject UpperCurtainObject { get; private set; }
    [field: SerializeField] public GameObject LowerCurtainObject { get; private set; }

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
        // ... 아래의 코드를 메서드로 묶어서 사용?

        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            DOTween.KillAll();
            GameManager.CodingUIManager_Instance.Initialize_CodingUIButtons();
        }

        
        // 클리어 패널의 위치를 초기 위치로 돌려야함
        // 정지 버튼이 비활성화 되어 있었고, 시작버튼 오브젝트로 바꿔야함
        if (GameManager.CodingUIManager_Instance.OptionPanel.activeSelf)
            GameManager.CodingUIManager_Instance.ActiveOption();


        GameManager.GameManager_Instance.Initialize_CodingMethod();
        GameManager.CodingUIManager_Instance.ClearPanel.transform.localPosition = GameManager.CodingUIManager_Instance.ClearPanelInitPos;

        // 커튼이 닫히기 전 코딩 버튼 비활성화
        // 커튼 이미지가 닫히고 난 뒤 씬 로드
        // 씬 로드 후에 0.5초 ~ 1초 딜레이 후 커튼 열림

        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void LoadNextScene()
    {
        // ... 아래의 코드를 메서드로 묶어서 사용?
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            DOTween.KillAll();
            GameManager.CodingUIManager_Instance.Initialize_CodingUIButtons();
        }

        
        // 클리어 패널의 위치를 초기 위치로 돌려야함
        // 정지 버튼이 비활성화 되어 있었고, 시작버튼 오브젝트로 바꿔야함
        if (GameManager.CodingUIManager_Instance.OptionPanel.activeSelf)
            GameManager.CodingUIManager_Instance.ActiveOption();


        GameManager.GameManager_Instance.Initialize_CodingMethod();
        GameManager.CodingUIManager_Instance.ClearPanel.transform.localPosition = GameManager.CodingUIManager_Instance.ClearPanelInitPos;


        // TODO: 테스트용 프로토타입 코드
        if (SceneManager.GetActiveScene().buildIndex < 9)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
