using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public enum CurrentLayout
    {
        Main,
        Function,
        Loop,
    }
    public enum SelectMethod
    {
        Function,
        Loop,
    }

    public static GameManager Instance { get; private set; }
    public UIManager UI { get; private set; } = new UIManager();

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Function = new List<CodingBlock>();
    public List<CodingBlock> Loop = new List<CodingBlock>();
    private Coroutine playBlock;
    private Tweener buttonTweener;

    public readonly WaitForSeconds waitForSeconds = new(1.0f);
    public readonly WaitForSeconds waitForHalfSeconds = new(0.5f);
    public readonly WaitForSeconds waitForPointEightSeconds = new(0.8f);
    public WaitUntil waitUntilPlay;

    public bool playBlockToggle { get; private set; } = false;
    private bool isPlayBlockRunning = false;

    [Header("현재 플레이어의 상태")]
    [SerializeField]
    public GameObject playerObject;
    public Animator playerAnimator;
    public CurrentLayout currentLayout = CurrentLayout.Main;
    public Vector3 playerStartPos { get; private set; }
    public Vector3 playerNewPos { get; private set; }

    private Vector3 playerRestPos;
    private Quaternion playerRestRot;

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("그리드 레이아웃 오브젝트")]
    public GameObject mainLayout;
    public GameObject functionLayout;
    public GameObject loopLayout;

    [Header("북마크 오브젝트")]
    public GameObject mainBookmark;
    public GameObject functionBookmark;
    public GameObject loopBookmark;

    [Header("블럭 삭제 버튼")]
    public GameObject mainDelete;
    public GameObject functionDelete;
    public GameObject loopDelete;

    [Header("플레이 & 정지, 스피드업 버튼")]
    public GameObject playButton;
    public GameObject stopButton;
    public GameObject speedUpButton;

    [Header("코딩블럭 버튼 오브젝트")]
    public GameObject forwardButton;
    public GameObject turnLeftButton;
    public GameObject turnRightButton;
    public GameObject functionButton;
    public GameObject loopButton;


    [Header("코딩블럭 프리팹")]
    public GameObject forwardPrefab;
    public GameObject turnLeftPrefab;
    public GameObject turnRightPrefab;
    public GameObject functionPrefab;
    public GameObject loopPrefab;

    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("[Singleton] GameManager is Created!");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("GameManager is Destroyed");
        }
        #endregion

        Application.targetFrameRate = 144;

        playerAnimator = playerObject.GetComponent<Animator>();
        waitUntilPlay = new WaitUntil(() => playBlockToggle == true);
    }

    private void Start()
    {
        playerRestPos = playerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        playerRestRot = playerObject.transform.rotation;

        MainMethod.Clear(); // OnSceneLoad 델리게이트 체인을 걸어서 사용하기, 새로운 스테이지 마다 블록 초기화
        Function.Clear();   // 레이아웃 내부에 블록 프리팹도 Destroy 하기
        Loop.Clear();

        playBlock = StartCoroutine(PlayBlock()); // 메인 게임이 시작될때만 동작 메뉴씬 이라면 코루틴 중지

        #region Coding blocks onClickAddListener
        // : each buttons link to each Prefab
        forwardButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(forwardPrefab));
        turnLeftButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(turnLeftPrefab));
        turnRightButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(turnRightPrefab));
        functionButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(functionPrefab));
        loopButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(loopPrefab));
        #endregion

        #region Layout activate onClickAddListener

        // Coding layout
        mainLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Main));
        functionLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Function));
        loopLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Loop));

        // Bookmark
        mainBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Main));
        functionBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Loop));
        loopBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Function));
        #endregion

        #region block delete OnClickAddListener

        mainDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Main; DeleteBlock(currentLayout); });
        functionDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Function; DeleteBlock(currentLayout); });
        loopDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Loop; DeleteBlock(currentLayout); });
        #endregion

        #region Play, Stop & TimeControl OnClickAddListener

        playButton.GetComponent<Button>().onClick.AddListener(() => playBlockToggle = true);
        stopButton.GetComponent<Button>().onClick.AddListener(() => StopBlock());
        speedUpButton.GetComponent<Button>().onClick.AddListener(() => PlaySpeedControl());
        #endregion
    }

    public void InsertBlock(GameObject prefab)
    {
        if (prefab == functionPrefab || prefab == loopPrefab)
        {
            if (MainMethod.Count < 10)
            {
                MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                prefab.GetComponent<CodingBlock>().enabled = false;
                UI.Block_PopAnimation(MainMethod.Last().gameObject);
            }
        }
        else
        {
            switch (currentLayout)
            {
                case CurrentLayout.Main:
                    if (MainMethod.Count < 10)
                    {
                        MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UI.Block_PopAnimation(MainMethod.Last().gameObject);
                    }
                    break;

                case CurrentLayout.Function:
                    if (Function.Count < 10)
                    {
                        Function.Add(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UI.Block_PopAnimation(Function.Last().gameObject);
                    }
                    break;

                case CurrentLayout.Loop:
                    if (Loop.Count < 10)
                    {
                        Loop.Add(Instantiate(prefab, loopLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UI.Block_PopAnimation(Loop.Last().gameObject);
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
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case CurrentLayout.Function:
                if (Function.Count > 0)
                {
                    CodingBlock lastblock = Function.Last();
                    Function.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case CurrentLayout.Loop:
                if (Loop.Count > 0)
                {
                    CodingBlock lastblock = Loop.Last();
                    Loop.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;
        }
    }

    public IEnumerator PlayBlock()
    {
        while (true)
        {
            if (!playBlockToggle) yield return waitUntilPlay;

            if (!isPlayBlockRunning && MainMethod != null)
            {
                isPlayBlockRunning = true;
                stopButton.gameObject.SetActive(true);
                UILock(true);

                foreach (CodingBlock block in MainMethod)
                {
                    if (!isPlayBlockRunning) 
                        break;

                    yield return waitForHalfSeconds;

                    PlayerMoveVectorInit();
                    block.GetComponent<CodingBlock>().enabled = true;

                    if (block.gameObject.tag == "Method")
                    {
                        block.MoveOrder();
                        yield return block.StartCoroutine(block.Subroutine());
                    }
                    else
                    {
                        block.MoveOrder();
                    }


                    if (isPlayBlockRunning) yield return waitForPointEightSeconds;
                }

                if (isPlayBlockRunning) yield return waitForSeconds;

                playBlockToggle = false;
                isPlayBlockRunning = false;
                BlockHighLightOff();
                UILock(false);
            }
        }
    }

    public void StopBlock()
    {
        playBlockToggle = false;
        isPlayBlockRunning = false;

        playerObject.transform.DOMove(playerRestPos, 0.3f);
        playerObject.transform.DORotateQuaternion(playerRestRot, 1f);
        playerAnimator.SetTrigger("Reset");

        BlockHighLightOff();
        ResetBlocksAnimation();

        stopButton.gameObject.SetActive(false);
    }

    public void PlayerMoveVectorInit()
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
        foreach (CodingBlock block in Loop)
        {
            block.ToggleHighLight(false);
        }
    }

    public void SelectedMethods(CurrentLayout selectMethod)
    {
        switch (selectMethod)
        {
            case CurrentLayout.Main:
                currentLayout = CurrentLayout.Main;
                break;

            case CurrentLayout.Function:
                currentLayout = CurrentLayout.Function;
                functionButton.gameObject.SetActive(true);
                loopButton.gameObject.SetActive(false);
                functionLayout.transform.parent.gameObject.SetActive(true);
                loopLayout.transform.parent.gameObject.SetActive(false);
                break;

            case CurrentLayout.Loop:
                currentLayout = CurrentLayout.Loop;
                loopButton.gameObject.SetActive(true);
                functionButton.gameObject.SetActive(false);
                loopLayout.transform.parent.gameObject.SetActive(true);
                functionLayout.transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    public void UILock(bool enable)
    {
        #region Blocks Lock
        forwardButton.GetComponent<Button>().enabled = !enable;
        turnLeftButton.GetComponent<Button>().enabled = !enable;
        turnRightButton.GetComponent<Button>().enabled = !enable;
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

    public void PlaySpeedControl()
    {
        if (Time.timeScale == 1f)
        {
            buttonTweener = UI.SpeedBtn_Animation(speedUpButton);
            Time.timeScale = 1.5f;
        }
        else
        {
            buttonTweener.Kill();
            speedUpButton.transform.localScale = Vector3.one;
            Time.timeScale = 1f;
        }
    }

    public void ResetBlocksAnimation()
    {
        foreach (CodingBlock block in MainMethod)
        {
            UI.BlockShakeAnimation(block.gameObject);
        }
    }
}
