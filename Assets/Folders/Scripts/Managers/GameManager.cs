using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum PlayerStatus
    {
        Idle,
        Forward,
        TurnLeft,
        TrunRight,
    }
    private PlayerStatus playerStatus = PlayerStatus.Idle;

    public static GameManager Instance { get; private set; }
    public GameObject PlayerObject { get; private set; }

    public Stack<CodingBlock> MainMethod { get; private set; } = new Stack<CodingBlock>();
    public Stack<CodingBlock> Function { get; private set; } = new Stack<CodingBlock>();



    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("인스턴스 생성!");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("중복된 인스턴스 삭제");
        }
        #endregion

    }

    private void Start()
    {

    }

    IEnumerator PlayerMove()
    {
        while(true)
        {
            yield return null;
        }

    }
}
