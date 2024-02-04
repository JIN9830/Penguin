using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum CurrentLayout
    {
        Main,
        Function,
    }
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
    public Status playerStatus = Status.Idle;
    public CurrentLayout currentLayout = CurrentLayout.Main;

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("그리드 레이아웃 오브젝트")]
    public GameObject mainLayout;
    public GameObject functionLayout;
    private Button mainLayoutButton;
    private Button functionLayoutButton;

    [Header("코딩블럭 버튼 오브젝트")]
    public Button forwardButton;
    public Button turnLeftButton;
    public Button turnRightButton;
    public Button functionButton;


    [Header("코딩블럭 프리팹")]
    public GameObject forwardPrefab;
    public GameObject turnLeftPrefab;
    public GameObject turnRightPrefab;
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

    private void Start()
    {
        MainMethod.Clear();
        Function.Clear();

        #region Coding Blocks Initialize
        // : each buttons link to each Prefab
        forwardButton.onClick.AddListener(() => InsertBlocks(forwardPrefab));
        turnLeftButton.onClick.AddListener(() => InsertBlocks(turnLeftPrefab));
        turnRightButton.onClick.AddListener(() => InsertBlocks(turnRightPrefab));
        functionButton.onClick.AddListener(() => InsertBlocks(functionPrefab));
        #endregion

        #region Layout Activate Buttons Initialize
        mainLayoutButton = mainLayout.GetComponent<Button>();
        functionLayoutButton = functionLayout.GetComponent<Button>();

        mainLayoutButton.onClick.AddListener(() => currentLayout = CurrentLayout.Main);
        functionLayoutButton.onClick.AddListener(() => currentLayout = CurrentLayout.Function);
        #endregion
    }

    public void InsertBlocks(GameObject prefab)
    {
        if (prefab == functionPrefab)
        {
            if (MainMethod.Count < 10)
            {
                MainMethod.Push(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
            }
        }
        else
        {
            switch (currentLayout)
            {
                case CurrentLayout.Main:
                    if (MainMethod.Count < 10) MainMethod.Push(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                    break;

                case CurrentLayout.Function:
                    if (Function.Count < 10) Function.Push(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                    break;
            }
        }
    }

    IEnumerator PlayerMove()
    {
        while (true)
        {
            switch (playerStatus)
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
