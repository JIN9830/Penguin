using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button LoadSceneTest;

    private void Awake()
    {
        LoadSceneTest.onClick.AddListener(() => GameSceneManager.Instance.LoadIndexScene(1));
    }
}
