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
    public ECurrentMethod currentMethod = ECurrentMethod.Main;

    public static GameManager GameManager_Instance { get; private set; }
    public static ObjectPoolManager ObjectPoolManager_Instance { get; private set; }
    public static PlayerManager PlayerManager_Instance { get; private set; }
    public static CodingUIManager CodingUIManager_Instance { get; private set; }
    public static StageManager StageManager_Instance { get; private set; }

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> FunctionMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> LoopMethod { get; private set; } = new List<CodingBlock>();

    [field: SerializeField] public int LoopReaptCount { get; set; } = 1;
    private int _loopCountTemp;

    public bool IsCompilerRunning { get; private set; } = false;
    public bool IsStageClear { get; private set; } = false;

    private Coroutine _blockCompiler;
    private Coroutine _subBlockCompiler;

    public readonly WaitForSeconds WAIT_FOR_SECONDS = new(1.0f);
    public readonly WaitForSeconds WAIT_FOR_HALF_SECONDS = new(0.5f);

    public WaitUntil WaitUntilExecutionTrigger { get; private set; }
    public WaitUntil WaitUntilSubMethodTrigger { get; private set; }
    public WaitUntil WaitUntilEndOfSubMethod { get; private set; }

    

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

        WaitUntilExecutionTrigger = new WaitUntil(() => IsCompilerRunning == true);
        WaitUntilSubMethodTrigger = new WaitUntil(() => currentMethod != ECurrentMethod.Main);
        WaitUntilEndOfSubMethod = new WaitUntil(() => currentMethod == ECurrentMethod.Main);

        Application.targetFrameRate = 144;
    }

    private void Start()
    {
        _blockCompiler = StartCoroutine(BlockCompiler_Co());
        _subBlockCompiler = StartCoroutine(SubBlockCompiler_Co());
    }

    /// <summary>
    /// GameManager 스크립트에서 무한 루프를 돌고 있는 메서드이며, MainLayout에 있는 블록들을 순차적으로 실행합니다.
    /// </summary>
    public IEnumerator BlockCompiler_Co()
    {
        while (true)
        {
            // .. 플레이어가 블록 실행 버튼을 누르기 전까지 해당 부분에서 대기하다가 블록 실행 버튼을 누르면 아래의 코드들이 진행되며 블록들이 실행됩니다.
            yield return WaitUntilExecutionTrigger;

            PlayerManager_Instance.cameraTargetObject.transform.localPosition = PlayerManager_Instance.cameraTargetObjectInitPos;

            foreach (CodingBlock block in MainMethod)
            {
                yield return WAIT_FOR_SECONDS;

                if (!IsCompilerRunning || IsStageClear) // .. 플레이어가 블록 정지 버튼을 누르거나 스테이지를 클리어하면 IsCompilerRunning가 false로 바뀌어 블록 순차 실행 코드를 탈출하여 블록 실행을 중단합니다.
                    break;

                AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                PlayerManager_Instance.InitPlayerMoveVector();
                block.GetComponent<CodingBlock>().enabled = true;
                block.MoveOrder();
                yield return WaitUntilEndOfSubMethod; // .. Func, Loop 메서드 블록이 실행중이라면 실행이 끝날때까지 대기합니다.
            }

            if (IsCompilerRunning) yield return WAIT_FOR_SECONDS;

            PlayerManager_Instance.PlayerAnimator.SetBool("WaitEmote", IsCompilerRunning);

            IsCompilerRunning = false;
            CodingUIManager_Instance.DisableBlockHighlights();
            CodingUIManager_Instance.LockUIElements(false);

        }
    }

    /// <summary>
    /// GameManager 스크립트에서 무한 루프를 돌고 있는 메서드이며, MainLayout에 있는 Func, Loop 블록의 내부에 있는 블록들을 순차적으로 실행합니다.
    /// </summary>
    public IEnumerator SubBlockCompiler_Co()
    {
        while (true)
        {
            // .. MainLayout에서 [Func, Loop] 블록이 실행되었다면 아래의 코드를 진행합니다. 그렇지 않다면 대기합니다.
            yield return WaitUntilSubMethodTrigger;

            switch (currentMethod)
            {

                #region ======================== Function Compiler Code ===========================================
                case ECurrentMethod.Function:

                    foreach (CodingBlock block in FunctionMethod)
                    {
                        yield return WAIT_FOR_SECONDS;

                        if (!IsCompilerRunning || IsStageClear)
                            break;

                        AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                        PlayerManager_Instance.InitPlayerMoveVector();
                        block.GetComponent<CodingBlock>().enabled = true;
                        block.MoveOrder();
                    }

                    if (IsCompilerRunning == true) yield return WAIT_FOR_SECONDS;

                    foreach (CodingBlock block in FunctionMethod)
                    {
                        block.ToggleHighLight(false);
                    }

                    CodingUIManager_Instance.SelectMethod(CodingUIManager.ECurrentLayout.Main);
                    currentMethod = ECurrentMethod.Main;
                    break;

                #endregion ========================================================================================



                #region ======================== Loop Compiler Code ===============================================
                case ECurrentMethod.Loop:

                    int LoopReaptCountTemp = LoopReaptCount;

                    for (int i = 0; i < LoopReaptCountTemp; i++)
                    {
                        // .. Loop 메서드의 내부의 블록들을 순차적으로 실행합니다.
                        foreach (CodingBlock block in LoopMethod)
                        {
                            yield return WAIT_FOR_SECONDS;

                            if (!IsCompilerRunning || IsStageClear)
                                break;

                            AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                            PlayerManager_Instance.InitPlayerMoveVector();
                            block.GetComponent<CodingBlock>().enabled = true;
                            block.MoveOrder();
                        }

                        // .. Loop 메서드 실행 도중에 중지버튼을 누르지 않았다면 다음 루프를 반복 실행할 준비를 합니다.
                        // ... 1초 딜레이 후 실행했던 블록들의 하이라이트를 제거 하고 Loop 메서드의 카운트 만큼 반복합니다.
                        if (IsCompilerRunning == true)
                        {
                            yield return WAIT_FOR_SECONDS;

                            foreach (CodingBlock block in LoopMethod)
                            {
                                block.ToggleHighLight(false);
                            }
                        }

                        LoopReaptCount--;
                        CodingUIManager_Instance.LoopCountText.text= LoopReaptCount.ToString();
                    }

                    LoopReaptCount = LoopReaptCountTemp;
                    CodingUIManager_Instance.LoopCountText.text = LoopReaptCount.ToString();

                    CodingUIManager_Instance.SelectMethod(CodingUIManager.ECurrentLayout.Main);
                    currentMethod = ECurrentMethod.Main;
                    break;

                    #endregion ===================================================================================

            }

            CodingUIManager_Instance.SelectMethod(CodingUIManager.ECurrentLayout.Main);
            currentMethod = ECurrentMethod.Main;
        }
    }

    public void Initialize_CodingMethod()
    {
        foreach (CodingBlock blockObj in MainMethod)
        {
            blockObj.ReleaseBlock();
        }
        foreach (CodingBlock blockObj in FunctionMethod)
        {
            blockObj.ReleaseBlock();
        }
        foreach (CodingBlock blockObj in LoopMethod)
        {
            blockObj.ReleaseBlock();
        }

        MainMethod.Clear();
        FunctionMethod.Clear();   
        LoopMethod.Clear();

        IsCompilerRunning = false;
        IsStageClear = false;
    }

    public void Register_ObjectPoolManager(GameObject obj)
    {
        obj.TryGetComponent(out ObjectPoolManager instance);
        ObjectPoolManager_Instance = instance;
    }
    public void Register_UIManager(GameObject obj)
    {
        obj.TryGetComponent(out CodingUIManager instance);
        CodingUIManager_Instance = instance;
    }
    public void Register_PlayerManager(GameObject obj)
    {
        obj.TryGetComponent(out PlayerManager instance);
        PlayerManager_Instance = instance;
    }
    public void Register_StageManager(GameObject obj)
    {
        obj.TryGetComponent(out StageManager instance);
        StageManager_Instance = instance;
    }

    /// <summary>
    /// 해당 프로퍼티의 수정은 게임플레이(블록 실행 / 정지) 부분에서 오류가 발생할수있습니다.
    /// </summary>
    /// <param name="enable"></param>
    public void Set_IsCompilerRunning(bool enable)
    {
        IsCompilerRunning = enable;
    }

    public void Set_IsStageClear(bool enable)
    {
        IsStageClear = enable;
    }
}