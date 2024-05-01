using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [field: SerializeField] public GameObject CurtainObject;

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

    public void LoadScene(int sceneIndex)
    {
        GameManager.GameManager_Instance.Initialize_CodingMethod();
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void LoadNextScene()
    {
        // TODO: 테스트용 코드 수정이 필요함
        if (SceneManager.GetActiveScene().buildIndex < 9)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
