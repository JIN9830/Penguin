using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum CurrentLayout
    {
        Main,
        Function,
    }
    public enum ProcessingStatus
    {
        Stop,
        Playing,
    }
    public enum Status
    {
        Idle,
        Forward,
        TurnLeft,
        TurnRight,
    }

    public static GameManager Instance { get; private set; }

    [Header("현재 플레이어의 상태")]
    [SerializeField]
    private GameObject PlayerObject;
    public Status playerStatus = Status.Idle;
    public CurrentLayout currentLayout = CurrentLayout.Main;
    public ProcessingStatus currentProcessing = ProcessingStatus.Stop;

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("그리드 레이아웃 오브젝트")]
    public GameObject mainLayout;
    public GameObject functionLayout;
    private Button mainLayoutButton;
    private Button functionLayoutButton;

    [Header("블럭 삭제 버튼")]
    public Button mainDelete;
    public Button functionDelete;

    [Header("플레이 & 정지, 스피드업 버튼")]
    public Button playButton;
    public Sprite stopImage;
    public Button speedUpButton;

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


    public float deltaTimeCount = 0;

    public bool isMoving = false;

    private readonly WaitForSeconds waitForSeconds = new(1.0f);
    private readonly WaitForSeconds waitForHalfSeconds = new(0.5f);

    public Stack<CodingBlock> MainMethod { get; private set; } = new Stack<CodingBlock>();
    public Stack<CodingBlock> Function = new Stack<CodingBlock>();


    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("싱글톤 게임매니저 생성!");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("중복된 싱글톤 게임매니저 삭제");
        }
        #endregion

        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        MainMethod.Clear();
        Function.Clear();

        StartCoroutine(PlayerMove());

        // TODO: 추후에 Awake 메서드로 가야할지 정하기
        #region Codingblocks onClickAddListener
        // : each buttons link to each Prefab
        forwardButton.onClick.AddListener(() => InsertBlock(forwardPrefab));
        turnLeftButton.onClick.AddListener(() => InsertBlock(turnLeftPrefab));
        turnRightButton.onClick.AddListener(() => InsertBlock(turnRightPrefab));
        functionButton.onClick.AddListener(() => InsertBlock(functionPrefab));
        #endregion

        #region Layout activate onClickAddListener
        mainLayoutButton = mainLayout.GetComponent<Button>();
        functionLayoutButton = functionLayout.GetComponent<Button>();

        mainLayoutButton.onClick.AddListener(() => currentLayout = CurrentLayout.Main);
        functionLayoutButton.onClick.AddListener(() => currentLayout = CurrentLayout.Function);
        #endregion

        #region block delete OnClickAddListener
        mainDelete.onClick.AddListener(() => {currentLayout = CurrentLayout.Main; DeleteBlock();});
        functionDelete.onClick.AddListener(() => { currentLayout = CurrentLayout.Function; DeleteBlock(); });
        #endregion

        playButton.onClick.AddListener(() => StartCoroutine(PlayBlockCode()));
        //stopButton.onClick.AddListener(() => { });
        //speedUpButton.onClick.AddListener(() => { });

    }

    public void InsertBlock(GameObject prefab)
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
                    Debug.Log("Main Stack:" + MainMethod.Count);
                    break;

                case CurrentLayout.Function:
                    if (Function.Count < 10) Function.Push(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                    Debug.Log("Function Stack:" + Function.Count);
                    break;
            }
        }
    }

    public void DeleteBlock()
    {
        switch (currentLayout)
        {
            case CurrentLayout.Main:
                if(MainMethod.Count > 0)
                {
                    CodingBlock lastblock = MainMethod.Pop();
                    Destroy(lastblock.gameObject);
                    Debug.Log("Main Stack:" + MainMethod.Count);
                } break;

            case CurrentLayout.Function:
                if (Function.Count > 0)
                {
                    CodingBlock lastblock = Function.Pop();
                    Destroy(lastblock.gameObject);
                    Debug.Log("Function Stack:" + Function.Count);
                } break;
        }
    }

    public IEnumerator PlayBlockCode()
    {
        if (MainMethod != null)
        {
            // TODO: 버튼 비활성화 코드 넣기

            foreach(CodingBlock block in MainMethod)
            {
                if(!isMoving)
                {
                    yield return waitForSeconds;
                    block.MoveOrder();
                    yield return waitForHalfSeconds;
                }
                
            }
        }
        else yield return null;
    }

    public IEnumerator PlayerMove()
    {
            switch (playerStatus)
            {
                case Status.Idle:
                    
                    break;

                case Status.Forward:
                    if (Physics.Raycast(PlayerObject.transform.localPosition, PlayerObject.transform.forward, out RaycastHit hit, 0.6f))
                    {
                        
                    }
                    else
                    {
                        while(isMoving)
                        {
                            Vector3 playerStartPos;
                            Vector3 playerNewPos;

                            playerStartPos = PlayerObject.transform.localPosition;
                            playerNewPos = playerStartPos + PlayerObject.transform.forward;

                            deltaTimeCount += Time.deltaTime;
                            Vector3 newPos = Vector3.Lerp(playerStartPos, playerNewPos, 0.1f * deltaTimeCount);
                            PlayerObject.transform.localPosition = newPos;

                            if (deltaTimeCount >= 1)
                            {
                                deltaTimeCount = 0;
                                playerStatus = Status.Idle;
                                isMoving = false;
                            } yield return null;
                        }
                    }
                    break;

                case Status.TurnLeft:
                    
                    break;

                case Status.TurnRight:
                    
                    break;

                default: break;
            }
        yield return null;
    }
}
