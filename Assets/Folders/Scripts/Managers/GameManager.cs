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
        Loop,
    }
    public enum ProcessingStatus
    {
        Stop,
        Playing,
    }
    public static GameManager Instance { get; private set; }

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Function = new List<CodingBlock>();
    public List<CodingBlock> Loop = new List<CodingBlock>();

    [Header("현재 플레이어의 상태")]
    [SerializeField]
    public GameObject playerObject;
    public CurrentLayout currentLayout = CurrentLayout.Main;
    public ProcessingStatus currentProcessing = ProcessingStatus.Stop;

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("그리드 레이아웃 오브젝트")]
    public Button mainLayout;
    public Button functionLayout;
    public Button loopLayout;

    [Header("북마크 오브젝트")]
    public Button mainBookmark;
    public Button functionBookmark;
    public Button loopBookmark;

    [Header("블럭 삭제 버튼")]
    public Button mainDelete;
    public Button functionDelete;
    public Button loopDelete;

    [Header("플레이 & 정지, 스피드업 버튼")]
    public Button playButton;
    public Button stopButton;
    public Button speedUpButton;

    [Header("코딩블럭 버튼 오브젝트")]
    public Button forwardButton;
    public Button turnLeftButton;
    public Button turnRightButton;
    public Button functionButton;
    public Button loopButton;


    [Header("코딩블럭 프리팹")]
    public GameObject forwardPrefab;
    public GameObject turnLeftPrefab;
    public GameObject turnRightPrefab;
    public GameObject functionPrefab;
    public GameObject loopPrefab;


    private readonly WaitForSeconds waitForSeconds = new(1.0f);
    private readonly WaitForSeconds waitForHalfSeconds = new(0.5f);

    public Vector3 playerStartPos;
    public Vector3 playerNewPos;

    public bool isMoving = false;
    public Animator playerAnimator;

    private Vector3 playerRestPos;
    private Quaternion playerRestRot;

    private Coroutine playBlock;

    public bool isPlayBlockRunning = false;
    public bool playBlockToggle = false;


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

        Application.targetFrameRate = 144;
    }

    private void Start()
    {
        // TODO: 스크립트가 시작되면이 아니라 추후에 스테이지가 시작될때 초기화로 변경
        MainMethod.Clear();
        Function.Clear();
        Loop.Clear();

        playerRestPos = playerObject.transform.position;
        playerRestRot = playerObject.transform.rotation;

        // TODO: 추후에 Awake 메서드로 가야할지 정하기
        #region Codingblocks onClickAddListener
        // : each buttons link to each Prefab
        forwardButton.onClick.AddListener(() => InsertBlock(forwardPrefab));
        turnLeftButton.onClick.AddListener(() => InsertBlock(turnLeftPrefab));
        turnRightButton.onClick.AddListener(() => InsertBlock(turnRightPrefab));
        functionButton.onClick.AddListener(() => InsertBlock(functionPrefab));
        loopButton.onClick.AddListener(() => InsertBlock(loopPrefab));
        #endregion

        #region Layout activate onClickAddListener

        // Coding area
        mainLayout.onClick.AddListener(() => currentLayout = CurrentLayout.Main);
        functionLayout.onClick.AddListener(() => currentLayout = CurrentLayout.Function);
        loopLayout.onClick.AddListener(() => currentLayout = CurrentLayout.Loop);

        // Bookmark
        mainBookmark.onClick.AddListener(() => {currentLayout = CurrentLayout.Main; BookMark(true);});
        functionBookmark.onClick.AddListener(() => {currentLayout = CurrentLayout.Loop; BookMark(true);});
        loopBookmark.onClick.AddListener(() => { currentLayout = CurrentLayout.Function; BookMark(true);});
        #endregion

        #region block delete OnClickAddListener
        mainDelete.onClick.AddListener(() => {currentLayout = CurrentLayout.Main; DeleteBlock(currentLayout);});
        functionDelete.onClick.AddListener(() => {currentLayout = CurrentLayout.Function; DeleteBlock(currentLayout);});
        loopDelete.onClick.AddListener(() => { currentLayout = CurrentLayout.Loop; DeleteBlock(currentLayout); });
        #endregion

        playBlock = StartCoroutine(PlayBlock());

        playButton.onClick.AddListener(() => playBlockToggle = true);
        stopButton.onClick.AddListener(() => StopBlock());
        //speedUpButton.onClick.AddListener(() => { });


        playerAnimator = playerObject.GetComponent<Animator>();
    }

    public void InsertBlock(GameObject prefab)
    {
        if (prefab == functionPrefab || prefab == loopPrefab)
        {
            if (MainMethod.Count < 10)
            {
                MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                prefab.GetComponent<CodingBlock>().enabled = false;
            }
        }
        else
        {
            switch (currentLayout)
            {
                case CurrentLayout.Main:
                    if (MainMethod.Count < 10) {
                        MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false; 
                    } 
                    break;

                case CurrentLayout.Function:
                    if (Function.Count < 10) {
                        Function.Add(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                    }
                    break;

                case CurrentLayout.Loop:
                    if (Loop.Count < 10) {
                        Loop.Add(Instantiate(prefab, loopLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                    }
                    break;
            }
        }
    }

    public void DeleteBlock(CurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case CurrentLayout.Main:
                if (MainMethod.Count > 0)
                {
                    CodingBlock lastblock = MainMethod.Last();
                    MainMethod.Remove(lastblock);
                    Destroy(lastblock.gameObject);
                }
                break;

            case CurrentLayout.Function:
                if (Function.Count > 0)
                {
                    CodingBlock lastblock = Function.Last();
                    Function.Remove(lastblock);
                    Destroy(lastblock.gameObject);
                }
                break;

            case CurrentLayout.Loop:
                if (Loop.Count > 0)
                {
                    CodingBlock lastblock = Loop.Last();
                    Loop.Remove(lastblock);
                    Destroy(lastblock.gameObject);
                }
                break;
        }
    }

    public IEnumerator PlayBlock()
    {
        while(true)
        {
            if (!playBlockToggle) yield return new WaitUntil(() => playBlockToggle == true); // new 연산자 Utills 클래스에 캐스팅하기

            if (!isPlayBlockRunning && MainMethod != null)
            {
                isPlayBlockRunning = true;
                stopButton.gameObject.SetActive(true);
                UILock(true);

                foreach (CodingBlock block in MainMethod)
                {
                    if (!isPlayBlockRunning) break; // 정지 버튼을 누르면 실행됨

                    yield return waitForHalfSeconds;

                    Player_MoveVectorInit();
                    block.GetComponent<CodingBlock>().enabled = true;
                    block.MoveOrder();

                    if (isPlayBlockRunning) yield return waitForHalfSeconds;
                }

                StopOrder();
                UILock(false);
                BlockHighLightOff();
            }
        }
    }

    public void StopBlock()
    {
        StopOrder();
        BlockHighLightOff();
        playerObject.transform.position = playerRestPos;
        playerObject.transform.rotation = playerRestRot;
        stopButton.gameObject.SetActive(false);
    }

    private void StopOrder()
    {
        isPlayBlockRunning = false;
        playBlockToggle = false;
    }

    public void Player_MoveVectorInit()
    {
        playerStartPos = playerObject.transform.localPosition;
        playerNewPos = playerStartPos + playerObject.transform.forward;
    }

    public void BlockHighLightOff()
    {
        foreach (CodingBlock block in MainMethod)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in Function)
        {
            block.ToggleHighLight(false);
        }
        foreach(CodingBlock block in Loop)
        {
            block.ToggleHighLight(false);
        }
    }

    public void BookMark(bool enable)
    {
        switch(currentLayout)
        {
            case CurrentLayout.Main:
                mainLayout.transform.parent.gameObject.SetActive(enable);
                break;

            case CurrentLayout.Function:
                functionLayout.transform.parent.gameObject.SetActive(enable);
                loopLayout.transform.parent.gameObject.SetActive(!enable);
                break;

            case CurrentLayout.Loop:
                loopLayout.transform.parent.gameObject.SetActive(enable);
                functionLayout.transform.parent.gameObject.SetActive(!enable);
                break;
        }
    }

    public void UILock(bool enable)
    {
        #region Blocks Lock
        forwardButton.GetComponent<Button>().enabled = !enable;
        turnLeftButton.GetComponent<Button>().enabled = !enable;
        turnRightButton.GetComponent<Button>().enabled= !enable;
        functionButton.GetComponent<Button>().enabled = !enable;
        loopButton.GetComponent<Button>().enabled = !enable;
        #endregion

        #region Layout & Bookmark & Delete Lock
        mainLayout.GetComponent<Button>().interactable = !enable;
        functionLayout.GetComponent<Button>().interactable = !enable;
        loopLayout.GetComponent<Button>().interactable = !enable;

        mainBookmark.GetComponent<Button>().interactable = !enable;
        functionBookmark.GetComponent<Button>().interactable = !enable;
        loopBookmark.GetComponent<Button>().interactable = !enable;

        mainDelete.GetComponent<Button>().interactable = !enable;
        functionDelete.GetComponent<Button>().interactable = !enable;
        loopDelete.GetComponent<Button>().interactable = !enable;
        #endregion
    }
}
