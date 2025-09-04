using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCodingManager : MonoBehaviour
{
    public enum CurrentMethod
    {
        Main,
        Function,
        Loop,
    }
    public CurrentMethod ECurrentMethod { get; set; } = CurrentMethod.Main;

    public static BlockCodingManager Instance { get; private set; }

    public static PlayerManager PlayerManager_Instance { get; private set; }
    public static StageManager StageManager_Instance { get; private set; }


    public List<CodingBlock> MainMethodList { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> FunctionMethodList { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> LoopMethodList { get; private set; } = new List<CodingBlock>();
    public int LoopReaptCount { get; set; } = 1;

    //public Action<bool> changeCompilerRunning;
    //private bool IsCompilerRunning = false;
    //public bool IsCompilerRunning
    //{
    //    get { return IsCompilerRunning; }
    //    set
    //    {
    //        IsCompilerRunning = value;
    //        changeCompilerRunning?.Invoke(value);
    //    }
    //}

    public bool IsCompilerRunning { get; set; } = false;
    public bool IsStageClear { get; set; } = false;


    public Coroutine BlockCompiler { get; set; } = null;
    public Coroutine SubBlockCompiler { get; set; } = null;


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
        WaitUntilSubMethodTrigger = new WaitUntil(() => ECurrentMethod != CurrentMethod.Main);
        WaitUntilEndOfSubMethod = new WaitUntil(() => ECurrentMethod == CurrentMethod.Main);

        Application.targetFrameRate = 144;
    }

    private void Start()
    {
        //BlockCompiler = StartCoroutine(BlockCompiler_Co());
        //SubBlockCompiler = StartCoroutine(SubBlockCompiler_Co());
    }

    /// <summary>
    /// 사용자가 코딩블록 실행 버튼을 누를 때까지 코드 제어권을 Unity 이벤트 함수에 넘기면서 대기합니다. 
    /// 사용자가 코딩블록 실행 버튼을 누르면, MainLayout에 있는 블록들이 순차적으로 실행됩니다.
    /// </summary>
    public IEnumerator BlockCompiler_Co()
    {
        CodingUIManager.Instance.PlayButton.interactable = false;

        // .. 블록 실행을 누르면 카메라 타겟이 플레이어 위치로 고정
        PlayerManager_Instance.CameraTargetObject.transform.localPosition = PlayerManager_Instance.CamTargetStartPosition;

        foreach (CodingBlock block in MainMethodList)
        {
            yield return Utils.WaitForSecond(1.0f);

            if (!IsCompilerRunning || IsStageClear)
                break;

            AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
            block.enabled = true;
            block.MoveOrder();
            // .. Func, Loop 블록이 실행중이라면 실행이 끝날때까지 대기합니다.
            yield return WaitUntilEndOfSubMethod;
        }

        if (IsCompilerRunning && !IsStageClear) yield return Utils.WaitForSecond(1.0f);

        PlayerManager_Instance.PlayerAnimator.SetBool("WaitEmote", IsCompilerRunning);

        IsCompilerRunning = false;
        CodingUIManager.Instance.DisableBlockHighlights();
        CodingUIManager.Instance.LockUIElements(false);

        CodingUIManager.Instance.PlayButton.interactable = true;
    }

    /// <summary>
    /// 함수[Func], 반복문[Loop] 블록이 실행 될 때까지 코드 제어권을 Unity 이벤트 함수에 넘기면서 대기합니다.
    /// 블록이 실행되면 함수[Func], 반복문[Loop] 블록의 내부에 있는 코딩블록을 순차적으로 실행합니다.
    /// </summary>
    public IEnumerator SubBlockCompiler_Co()
    {

        //// .. MainLayout에서 [Func], [Loop] 블록이 실행될때까지 여기서 대기하며 코드 제어권을 Unity 이벤트 함수에 넘깁니다.
        //yield return WaitUntilSubMethodTrigger;

        switch (ECurrentMethod)
        {

            #region ======================== * Function Compiler Start * ================
            case CurrentMethod.Function:

                foreach (CodingBlock codingBlock in FunctionMethodList)
                {
                    yield return Utils.WaitForSecond(1.0f);

                    if (!IsCompilerRunning || IsStageClear)
                        break;

                    AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                    codingBlock.enabled = true;
                    codingBlock.MoveOrder();
                }

                if (IsCompilerRunning || !IsStageClear) yield return Utils.WaitForSecond(1.0f);

                foreach (CodingBlock block in FunctionMethodList)
                {
                    block.ToggleHighLight(false);
                }

                break;

            #endregion ====================== Function Compiler End ======================




            #region ======================== * Loop Compiler Start * =====================
            case CurrentMethod.Loop:

                // LoopReaptCount: UI에 표시되는 Loop 반복 횟수 변수
                // tempLoopReaptCount: 코드 내부적으로 Loop가 동작하는 반복 횟수를 저장하는 변수 

                int tempLoopReaptCount = LoopReaptCount;

                for (int i = 0; i < tempLoopReaptCount; i++)
                {
                    if (!IsCompilerRunning || IsStageClear)
                        break;

                    foreach (CodingBlock codingBlock in LoopMethodList)
                    {
                        yield return Utils.WaitForSecond(1.0f);

                        if (!IsCompilerRunning || IsStageClear)
                            break;

                        AudioManager.Instance.Play_UISFX("ActiveCodingBlock");
                        codingBlock.enabled = true;
                        codingBlock.MoveOrder();
                    }

                    // 블록 실행 중지 버튼이 눌리지 않으면, 1초 딜레이 후 Loop 내부의 블록 하이라이트를 전부 제거합니다.
                    if (IsCompilerRunning || !IsStageClear)
                    {
                        yield return Utils.WaitForSecond(1.0f);

                        foreach (CodingBlock block in LoopMethodList)
                        {
                            block.ToggleHighLight(false);
                        }
                    }
                    // Loop의 한 사이클이 끝나면 UI의 표시되는 Loop의 반복횟수(LoopReaptCount)를 -1 감소시키고, for 문으로 돌아가서 Loop를 반복합니다.
                    LoopReaptCount--;
                    CodingUIManager.Instance.LoopCountText.text = LoopReaptCount.ToString();
                }

                // Loop가 완료되면 UI에서 표시되는 Loop의 반복횟수(LoopReaptCount)를 초기값(tempLoopReaptCount)으로 리셋합니다.
                LoopReaptCount = tempLoopReaptCount;
                CodingUIManager.Instance.LoopCountText.text = LoopReaptCount.ToString();
                break;

                #endregion ======================== Loop Compiler End ====================
        }

        CodingUIManager.Instance.SelectMethod(CodingUIManager.CurrentLayout.Main);
        ECurrentMethod = CurrentMethod.Main;
    }

    public void Initialize_CodingMethod()
    {
        // .. 레이아웃에 있는 블록 오브젝트들을 오브젝트 풀에 전부 반환.
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

        // .. 각 리스트 내부의 코딩블럭 데이터를 전부 삭제.
        MainMethodList.Clear();
        FunctionMethodList.Clear();
        LoopMethodList.Clear();

        // .. 블럭 컴파일러의 실행 상태 & 게임 클리어 상태를 저장하는 변수를 변수의 기본 값인 false로 초기화.
        IsCompilerRunning = false;
        IsStageClear = false;

        // .. 루프문의 반복 횟수를 기본 값인 1로 초기화.
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