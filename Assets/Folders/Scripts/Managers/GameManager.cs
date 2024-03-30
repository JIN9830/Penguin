using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public enum ECurrentMethod
    {
        Main,
        Function,
        Loop,
    }

    public ECurrentMethod currentMethod = ECurrentMethod.Main;
    public static GameManager GameManager_Instance { get; private set; }
    public static PlayerManager PlayerManager_Instance { get; private set; }
    public static CodingUIManager UIManager_Instance { get; private set; }

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> FunctionMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> LoopMethod { get; private set; } = new List<CodingBlock>();

    [SerializeField] private int loopReaptCount = 1;

    public bool PlayToggle { get; private set; } = false;
    public bool IsMainMethodRunning { get; private set; } = false;

    private Coroutine blockCompiler;
    private Coroutine subBlockCompiler;

    public readonly WaitForSeconds waitForSeconds = new(1.0f);
    public readonly WaitForSeconds waitForHalfSeconds = new(0.5f);
    public readonly WaitForSeconds waitForPointEightSeconds = new(0.8f);

    public WaitUntil waitUntilPlayTrigger;
    public WaitUntil waitUntilSubMethodTrigger;
    public WaitUntil waitUntilEndOfSubMethod;

    private void Awake()
    {
        #region Singleton Code
        if (GameManager_Instance == null)
        {
            GameManager_Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("GameManager is Created!");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("GameManager is Destroyed");
        }
        #endregion

        waitUntilPlayTrigger = new WaitUntil(() => PlayToggle == true);
        waitUntilSubMethodTrigger = new WaitUntil(() => currentMethod != ECurrentMethod.Main);
        waitUntilEndOfSubMethod = new WaitUntil(() => currentMethod == ECurrentMethod.Main);

        Application.targetFrameRate = 144;
    }

    private void Start()
    {
        blockCompiler = StartCoroutine(BlockCompiler_Co());
        subBlockCompiler = StartCoroutine(SubBlockCompiler_Co());
    }

    public IEnumerator BlockCompiler_Co()
    {
        while (true)
        {
            if (!PlayToggle) yield return waitUntilPlayTrigger;

            IsMainMethodRunning = true;
            UIManager_Instance.playButton.gameObject.SetActive(false);
            UIManager_Instance.stopButton.gameObject.SetActive(true);
            UIManager_Instance.LockUIElements(true);

            foreach (CodingBlock block in MainMethod)
            {
                if (!IsMainMethodRunning)
                    break;

                yield return waitForHalfSeconds;

                PlayerManager_Instance.InitializePlayerMoveVector();
                block.GetComponent<CodingBlock>().enabled = true;
                block.MoveOrder();

                yield return waitUntilEndOfSubMethod;

                if (IsMainMethodRunning) yield return waitForPointEightSeconds;
            }

            if (IsMainMethodRunning) yield return waitForSeconds;

            PlayToggle = false;
            IsMainMethodRunning = false;
            UIManager_Instance.DisableBlockHighlights();
            UIManager_Instance.LockUIElements(false);
        }
    }

    public IEnumerator SubBlockCompiler_Co()
    {
        while (true)
        {
            yield return waitUntilSubMethodTrigger;

            switch (currentMethod)
            {
                #region Function Compiler Code
                case ECurrentMethod.Function:

                    foreach (CodingBlock block in FunctionMethod)
                    {
                        if (PlayToggle == false)
                            break;

                        yield return waitForPointEightSeconds;

                        PlayerManager_Instance.InitializePlayerMoveVector();
                        block.GetComponent<CodingBlock>().enabled = true;
                        block.MoveOrder();

                        if (PlayToggle == true) yield return waitForPointEightSeconds;
                    }

                    if (PlayToggle == true) yield return waitForPointEightSeconds;

                    foreach (CodingBlock block in FunctionMethod)
                    {
                        block.ToggleHighLight(false);
                    }

                    UIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Main);
                    currentMethod = ECurrentMethod.Main;
                    break;
                #endregion

                #region Loop Compiler Code
                case ECurrentMethod.Loop:

                    for (int i = 0; i < loopReaptCount; i++)
                    {
                        if (PlayToggle == false)
                            break;

                        foreach (CodingBlock block in LoopMethod)
                        {
                            if (PlayToggle == false)
                                break;

                            yield return waitForPointEightSeconds;

                            PlayerManager_Instance.InitializePlayerMoveVector();
                            block.GetComponent<CodingBlock>().enabled = true;
                            block.MoveOrder();

                            if (PlayToggle == true) yield return waitForHalfSeconds;
                        }

                        if (PlayToggle == true)
                        {
                            yield return waitForPointEightSeconds;

                            foreach (CodingBlock block in LoopMethod)
                            {
                                block.ToggleHighLight(false);
                            }
                        }

                    }

                    UIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Main);
                    currentMethod = ECurrentMethod.Main;
                    break;
                    #endregion
            }


            UIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Main);
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
        UIManager_Instance = instance;
    }
    public void Get_PlayerManager(GameObject obj)
    {
        obj.TryGetComponent(out PlayerManager instance);
        PlayerManager_Instance = instance;
    }

    public void Set_PlayToggle(bool enable)
    {
        PlayToggle = enable;
    }
    public void Set_IsMainMethodRunning(bool enable)
    {
        IsMainMethodRunning = enable;
    }
}