using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public enum ECurrentMethod
    {
        Main,
        Function,
        Loop,
    }
    public enum ECodingBlockState
    {
        Playing,
        Finished,
    }
    public ECurrentMethod currentMethod = ECurrentMethod.Main;
    public ECodingBlockState codingBlockState = ECodingBlockState.Finished;

    public static GameManager GameManager_Instance { get; private set; }
    public static PlayerManager PlayerManager_Instance { get; private set; }
    public static CodingUIManager CodingUIManager_Instance { get; private set; }

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> FunctionMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> LoopMethod { get; private set; } = new List<CodingBlock>();

    [SerializeField] private int loopReaptCount = 1;

    public bool ExecutionToggle { get; private set; } = false;
    public bool IsMainMethodRunning { get; private set; } = false;

    private Coroutine blockCompiler;
    private Coroutine subBlockCompiler;

    public readonly WaitForSeconds waitForSeconds = new(1.0f);
    public readonly WaitForSeconds waitForHalfSeconds = new(0.5f);
    public readonly WaitForSeconds waitForPointSevenSeconds = new(0.7f);

    public WaitUntil waitUntilExecutionTrigger;
    public WaitUntil waitUntilBlockFinished;
    public WaitUntil waitUntilSubMethodTrigger;
    public WaitUntil waitUntilEndOfSubMethod;

    private void Awake()
    {
        #region Singleton Code
        if (GameManager_Instance == null)
        {
            GameManager_Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        #endregion

        waitUntilExecutionTrigger = new WaitUntil(() => ExecutionToggle == true);
        waitUntilBlockFinished = new WaitUntil(() => codingBlockState == ECodingBlockState.Finished);
        waitUntilSubMethodTrigger = new WaitUntil(() => currentMethod != ECurrentMethod.Main);
        waitUntilEndOfSubMethod = new WaitUntil(() => currentMethod == ECurrentMethod.Main);

        Application.targetFrameRate = 144;
    }

    private void Start()
    {
        blockCompiler = StartCoroutine(BlockCompiler_Co());
        subBlockCompiler = StartCoroutine(SubBlockCompiler_Co());
    }


    /// <summary>
    /// 무한 루프를 돌고 있다가 실행 버튼의 클릭 이벤트를 감지했을 때 메인 레이아웃에 있는 블록들을 읽고 실행합니다.
    /// </summary>
    public IEnumerator BlockCompiler_Co()
    {
        while (true)
        {
            // .. 플레이어가 실행 버튼을 누르기 전까지 해당 부분에서 코드 제어권이 유니티에게 돌아갑니다. 실행 버튼이 눌리면 아래의 블록 컴파일 코드가 진행됩니다.
            yield return waitUntilExecutionTrigger;

            // .. 블록 컴파일이 진행중이라면 참(true), 아니라면 거짓(false)으로 블록 컴파일의 상태를 담고 있는 변수입니다.
            IsMainMethodRunning = true;

            CodingUIManager_Instance.executionButton.gameObject.SetActive(false); 
            CodingUIManager_Instance.stopButton.gameObject.SetActive(true);
            CodingUIManager_Instance.LockUIElements(true);

            foreach (CodingBlock block in MainMethod)
            {
                if (!IsMainMethodRunning)
                    break;

                yield return waitForHalfSeconds;

                PlayerManager_Instance.InitPlayerMoveVector();
                block.GetComponent<CodingBlock>().enabled = true;
                block.MoveOrder();

                yield return waitUntilEndOfSubMethod;

                if (IsMainMethodRunning) yield return waitForPointSevenSeconds;
            }

            if (IsMainMethodRunning) yield return waitForSeconds;

            ExecutionToggle = false;
            IsMainMethodRunning = false;
            CodingUIManager_Instance.DisableBlockHighlights();
            CodingUIManager_Instance.LockUIElements(false);
        }
    }

    /// <summary>
    /// 무한 루프를 돌고 있다가 함수, 루프 블록이 메인 레이아웃에서 실행됐을 때 함수, 루프 레이아웃에 있는 블록들을 읽고 실행합니다.
    /// </summary>
    public IEnumerator SubBlockCompiler_Co()
    {
        while (true)
        {
            // .. 메인 레이아웃에서 함수, 루프 블록이 실행되기 전까지 해당 부분에서 코드 제어권이 유니티에게 돌아갑니다.
            yield return waitUntilSubMethodTrigger;

            switch (currentMethod)
            {
                #region Function Compiler Code
                case ECurrentMethod.Function:

                    foreach (CodingBlock block in FunctionMethod)
                    {
                        if (ExecutionToggle == false)
                            break;

                        yield return waitForPointSevenSeconds;

                        PlayerManager_Instance.InitPlayerMoveVector();
                        block.GetComponent<CodingBlock>().enabled = true;
                        block.MoveOrder();

                        if (ExecutionToggle == true) yield return waitForPointSevenSeconds;
                    }

                    if (ExecutionToggle == true) yield return waitForPointSevenSeconds;

                    foreach (CodingBlock block in FunctionMethod)
                    {
                        block.ToggleHighLight(false);
                    }

                    CodingUIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Main);
                    currentMethod = ECurrentMethod.Main;
                    break;
                #endregion

                #region Loop Compiler Code
                case ECurrentMethod.Loop:

                    for (int i = 0; i < loopReaptCount; i++)
                    {
                        if (ExecutionToggle == false)
                            break;

                        foreach (CodingBlock block in LoopMethod)
                        {
                            if (ExecutionToggle == false)
                                break;

                            yield return waitForPointSevenSeconds;

                            PlayerManager_Instance.InitPlayerMoveVector();
                            block.GetComponent<CodingBlock>().enabled = true;
                            block.MoveOrder();

                            if (ExecutionToggle == true) yield return waitForHalfSeconds;
                        }

                        if (ExecutionToggle == true)
                        {
                            yield return waitForPointSevenSeconds;

                            foreach (CodingBlock block in LoopMethod)
                            {
                                block.ToggleHighLight(false);
                            }
                        }

                    }

                    CodingUIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Main);
                    currentMethod = ECurrentMethod.Main;
                    break;
                    #endregion
            }


            CodingUIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Main);
            currentMethod = ECurrentMethod.Main;
        }
    }

    public void Initialize_CodingMethod()
    {
        MainMethod.Clear(); // OnSceneLoad 델리게이트 체인을 걸어서 사용하기, 새로운 스테이지 마다 블록 초기화
        FunctionMethod.Clear();   // 레이아웃 내부에 블록 프리팹도 Destroy 하기
        LoopMethod.Clear();

        foreach (CodingBlock blockObj in MainMethod)
        {
            Destroy(blockObj.gameObject);
        }
        foreach (CodingBlock blockObj in FunctionMethod)
        {
            Destroy(blockObj.gameObject);
        }
        foreach (CodingBlock blockObj in LoopMethod)
        {
            Destroy(blockObj.gameObject);
        }
    }
    public void Get_UIManager(GameObject obj)
    {
        obj.TryGetComponent(out CodingUIManager instance);
        CodingUIManager_Instance = instance;
    }
    public void Get_PlayerManager(GameObject obj)
    {
        obj.TryGetComponent(out PlayerManager instance);
        PlayerManager_Instance = instance;
    }

    public void Set_ExecutionToggle(bool enable)
    {
        ExecutionToggle = enable;
    }
    public void Set_IsMainMethodRunning(bool enable)
    {
        IsMainMethodRunning = enable;
    }
}