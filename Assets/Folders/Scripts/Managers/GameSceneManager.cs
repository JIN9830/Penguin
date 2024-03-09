using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public void GoToStartScene()
    {
        SceneManager.LoadScene("Start");
    }

    public void GoToCityScene()
    {
        SceneManager.LoadScene("City");

    }
}
