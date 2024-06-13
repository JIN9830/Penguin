using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum CurrentMethod
    {
        Main,
        Function,
        Loop,
    }
    public CurrentMethod currentMethod { get; set; } = CurrentMethod.Main;


    public static GameManager Instance { get; private set; }

    public static PlayerManager PlayerManager_Instance { get; private set; }
    public static StageManager StageManager_Instance { get; private set; }


    public List<CodingBlock> MainMethodList { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> FunctionMethodList { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> LoopMethodList { get; private set; } = new List<CodingBlock>();
    [field: SerializeField] public int LoopReaptCount { get; set; } = 1;


    public bool IsCompilerRunning { get; set; } = false;
    public bool IsStageClear { get; set; } = false;



    private Coroutine _blockCompiler;
    private Coroutine _subBlockCompiler;


    public WaitUntil WaitUntilExecutionTrigger { get; private set; }
    public WaitUntil WaitUntilSubMethodTrigger { get; private set; }
    public WaitUntil WaitUntilEndOfSubMethod { get; private set; }



    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        #endregion

        WaitUntilExecutionTrigger = new WaitUntil(() => IsCompilerRunning == true);
        WaitUntilSubMethodTrigger = new WaitUntil(() => currentMethod != CurrentMethod.Main);
        WaitUntilEndOfSubMethod = new WaitUntil(() => currentMethod == CurrentMethod.Main);

        Application.targetFrameRate = 144;
    }

    private void Start()
    {
        _blockCompiler = StartCoroutine(BlockCompiler_Co());
        _subBlockCompiler = StartCoroutine(SubBlockCompiler_Co());
    }

    /// <summary>
    /// 사용자가 코딩 블록 실행 버튼을 누를 때까지 대기하며 이 기간 동안 코드 제어권을 Unity 이벤트 함수에 넘깁니다.
    /// 사용자가 실행 버튼을 누르면, MainLayout에 있는 블록들이 순차적으로 실행됩니다.
    /// </summary>
    public IEnumerator BlockCompiler_Co()
    {
        while (true)
        {
            // .. 사용자가 코딩 블록 실행 버튼을 누를 때까지 대기하며 이 기간 동안 코드 제어권을 Unity 이벤트 함수에 넘깁니다.
            yield return WaitUntilExecutionTrigger;

            // .. 블록 코딩을 시작하면 코드 실행 버튼을 비활성화 합니다.
            CodingUIManager.Instance.ExecuteButton.interactable = false;

            // .. 블록 실행을 누르면 카메라 타겟이 플레이어 위치로 고정
            PlayerManager_Instance.CameraTargetObject.transform.localPosition = PlayerManager_Instance.CamTargetStartPosition; 

            foreach (CodingBlock block in MainMethodList)
            {
                yield return Util.WaitForSecond(1.0f);

                // .. 플레이어가 블록 정지 버튼을 누르거나 스테이지를 클리어하면 IsCompilerRunning = false로 바뀌어 블록 순차 실행 코드를 탈출합니다.
                if (!IsCompilerRunning || IsStageClear) 
                    break;

                AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                PlayerManager_Instance.InitPlayerMoveVector();
                block.enabled = true;
                block.MoveOrder();
                // .. Func, Loop 블록이 실행중이라면 실행이 끝날때까지 대기합니다.
                yield return WaitUntilEndOfSubMethod; 
            }

            if (IsCompilerRunning && !IsStageClear) yield return Util.WaitForSecond(1.0f);

            PlayerManager_Instance.PlayerAnimator.SetBool("WaitEmote", IsCompilerRunning);

            IsCompilerRunning = false;
            CodingUIManager.Instance.DisableBlockHighlights();
            CodingUIManager.Instance.LockUIElements(false);

            // .. 블록 코딩이 끝나면 코드 실행 버튼을 활성화 합니다.
            CodingUIManager.Instance.ExecuteButton.interactable = true; 
        }
    }

    /// <summary>
    /// 함수(func), 반복문(Loop) 블록이 실행 될 때까지 대기하며 이 기간 동안 코드 제어권을 Unity 이벤트 함수에 넘깁니다.
    /// 함수, 반복문 블록이 실행되면 MainLayout에 있는 Func, Loop 블록의 내부에 있는 블록들을 순차적으로 실행합니다.
    /// </summary>
    public IEnumerator SubBlockCompiler_Co()
    {
        while (true)
        {
            // .. MainLayout에서 [Func], [Loop] 블록이 실행될때까지 대기하며 이 기간 동안 코드 제어권을 Unity 이벤트 함수에 넘깁니다.
            yield return WaitUntilSubMethodTrigger;

            switch (currentMethod)
            {

                #region ======================== * Function Compiler Start * ================
                case CurrentMethod.Function:

                    foreach (CodingBlock block in FunctionMethodList)
                    {
                        yield return Util.WaitForSecond(1.0f);

                        if (!IsCompilerRunning || IsStageClear)
                            break;

                        AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                        PlayerManager_Instance.InitPlayerMoveVector();
                        block.enabled = true;
                        block.MoveOrder();
                    }

                    if (IsCompilerRunning || !IsStageClear) yield return Util.WaitForSecond(1.0f);

                    foreach (CodingBlock block in FunctionMethodList)
                    {
                        block.ToggleHighLight(false);
                    }

                    break;

                #endregion ====================== Function Compiler End ======================





                #region ======================== * Loop Compiler Start * =====================
                case CurrentMethod.Loop:

                    int loopReaptCountTemp = LoopReaptCount;

                    for (int i = 0; i < loopReaptCountTemp; i++)
                    {
                        if (!IsCompilerRunning || IsStageClear)
                            break;

                        foreach (CodingBlock block in LoopMethodList)
                        {
                            yield return Util.WaitForSecond(1.0f);

                            if (!IsCompilerRunning || IsStageClear)
                                break;

                            AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                            PlayerManager_Instance.InitPlayerMoveVector();
                            block.enabled = true;
                            block.MoveOrder();
                        }

                        // .. 블록 실행 중지 버튼이 눌리지 않았다면, 다음 루프를 반복 실행할 준비를 합니다. 1초 딜레이 후,
                        // .. 실행했던 블록들의 하이라이트를 제거하고 LoopReaptCount 만큼 반복합니다.
                        if (IsCompilerRunning || !IsStageClear)
                        {
                            yield return Util.WaitForSecond(1.0f);

                            foreach (CodingBlock block in LoopMethodList)
                            {
                                block.ToggleHighLight(false);
                            }
                        }
                        
                        LoopReaptCount--;
                        CodingUIManager.Instance.LoopCountText.text= LoopReaptCount.ToString();
                    }

                    LoopReaptCount = loopReaptCountTemp;
                    CodingUIManager.Instance.LoopCountText.text = LoopReaptCount.ToString();

                    break;

                    #endregion ======================== Loop Compiler End ====================
            }

            CodingUIManager.Instance.SelectMethod(CodingUIManager.ECurrentLayout.Main);
            currentMethod = CurrentMethod.Main;
        }
    }

    public void Initialize_CodingMethod()
    {
        // .. 레이아웃에 있는 블록 오브젝트들을 오브젝트 풀에 전부 반환
        foreach (CodingBlock blockObj in MainMethodList)
        {
            blockObj.ReleaseBlock();
        }
        foreach (CodingBlock blockObj in FunctionMethodList)
        {
            blockObj.ReleaseBlock();
        }
        foreach (CodingBlock blockObj in LoopMethodList)
        {
            blockObj.ReleaseBlock();
        }

        // .. 각 리스트 내부의 코딩블럭 데이터를 전부 삭제
        MainMethodList.Clear();
        FunctionMethodList.Clear();   
        LoopMethodList.Clear();

        // .. 코딩블럭 컴파일의 실행 상태 변수와 & 게임 클리어 상태 변수를 디폴드 값인 거짓으로 변경
        IsCompilerRunning = false;
        IsStageClear = false;

        // .. 루프 반속 횟수를 기본값인 1로 변경
        LoopReaptCount = 1;
        CodingUIManager.Instance.LoopCountText.text = Instance.LoopReaptCount.ToString();
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
}