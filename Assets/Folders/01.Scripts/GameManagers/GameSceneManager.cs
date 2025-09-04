using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [field: SerializeField] public GameObject LoadingTouchBlockPanel { get; private set; }

    [field: SerializeField] public GameObject CurtainUpper { get; private set; }
    [field: SerializeField] public GameObject CurtainLower { get; private set; }


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

    public IEnumerator LoadIndexScene_Co(int sceneIndex)
    {
        SetUp_NextCodingScene();

        LoadingTouchBlockPanel.SetActive(true);
        CodingUIManager.Instance.UIAnimation.Animation_LoadingCurtain(CurtainUpper, CurtainLower, true);

        yield return Utils.WaitForSecond(1.0f);

        SceneManager.LoadSceneAsync(sceneIndex);

        LoadingTouchBlockPanel.SetActive(false);
        CodingUIManager.Instance.UIAnimation.Animation_LoadingCurtain(CurtainUpper, CurtainLower, false);
    }

    public IEnumerator LoadNextScene_Co()
    {
        SetUp_NextCodingScene();

        LoadingTouchBlockPanel.SetActive(true);
        CodingUIManager.Instance.UIAnimation.Animation_LoadingCurtain(CurtainUpper, CurtainLower, true);

        yield return Utils.WaitForSecond(1.0f);

        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings + 1)
            SceneManager.LoadSceneAsync(0);
        else
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        LoadingTouchBlockPanel.SetActive(false);
        CodingUIManager.Instance.UIAnimation.Animation_LoadingCurtain(CurtainUpper, CurtainLower, false);
    }

    public void SetUp_NextCodingScene()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            Time.timeScale = 1.0f;

            DOTween.KillAll();

            CodingUIManager.Instance.Initialize_CodingUIButtonState();

            if (CodingUIManager.Instance.OptionPanel.activeSelf)
                CodingUIManager.Instance.ActiveOption();
        }

        BlockCodingManager.Instance.Initialize_CodingMethod();
        CodingUIManager.Instance.SelectMethod(CodingUIManager.CurrentLayout.Main);
    }
}
