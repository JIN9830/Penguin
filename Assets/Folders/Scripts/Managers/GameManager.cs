using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Status
    {
        Idle,
        Forward,
        TurnLeft,
        TurnRight,
    }

    public static GameManager Instance { get; private set; }
    public GameObject PlayerObject { get; private set; }

    [Header("현재 플레이어의 상태")]
    public Status PlayerStatus = Status.Idle;

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("코딩블럭 버튼 오브젝트")]
    public GameObject forwardButton;
    public GameObject turnLeftButton;
    public GameObject turnRightButton;
    public GameObject functionButton;


    [Header("코딩블럭 프리팹")]
    public GameObject forwardPrefab;
    public GameObject trunLeftPrefab;
    public GameObject trunRightPrefab;
    public GameObject functionPrefab;


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



    IEnumerator PlayerMove()
    {
        while (true)
        {
            switch (PlayerStatus)
            {
                case Status.Idle:
                    // 플레이어 대기 상태
                    break;

                case Status.Forward:
                    // 플레이어가 앞으로 이동
                    break;

                case Status.TurnLeft:
                    // 플레이어가 왼쪽으로 회전
                    break;

                case Status.TurnRight: 
                    // 플레이어가 오른쪽으로 회전
                    break;

            }
            yield return null;
        }

    }
}
