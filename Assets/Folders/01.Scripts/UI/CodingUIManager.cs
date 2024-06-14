using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CodingUIManager : MonoBehaviour
{
    public enum CurrentLayout
    {
        Main,
        Function,
        Loop,
    } 
    public CurrentLayout ECurrentLayout { get; private set; }


    public static CodingUIManager Instance { get; private set; }

    public UIAnimation UIAnimation { get; private set; } = new UIAnimation();

    [field: SerializeField] public GameObject ReleasedBlocks { get; private set; }

    [field: SerializeField] public GameObject CodingUICanvas { get; private set; }


    [field: Header("그리드 레이아웃 오브젝트")]
    [field: SerializeField] public Button MainLayout { get; private set; }
    [field: SerializeField] public Button FunctionLayout { get; private set; }
    [field: SerializeField] public Button LoopLayout { get; private set; }

    private Image _mainLayoutImageComponent, _functionLayoutImageComponent, _loopLayoutImageComponent;
    private readonly Color _GREY_LAYOUT_COLOR = new Color32(135, 135, 135, 125);
    private readonly Color _GREEN_LAYOUT_COLOR = new Color32(122, 149, 113, 125);
    private readonly Color _PURPLE_LAYOUT_COLOR = new Color32(122, 104, 142, 125);
    private readonly Color _ORANGE_LAYOUT_COLOR = new Color32(186, 150, 118, 125);


    [field: Header("블럭 삭제 버튼")]
    [field: SerializeField] public Button MainDelete { get; private set; }
    [field: SerializeField] public Button FunctionDelete { get; private set; }
    [field: SerializeField] public Button LoopDelete { get; private set; }


    [field: Header("북마크 오브젝트")]
    [field: SerializeField] public Button MainBookmark { get; private set; }
    [field: SerializeField] public Button FunctionBookmark { get; private set; }
    [field: SerializeField] public Button LoopBookmark { get; private set; }
    [field: SerializeField] public Button LoopCountPlus { get; private set; }
    [field: SerializeField] public Button LoopCountMinus { get; private set; }
    [field: SerializeField] public TextMeshProUGUI LoopCountText { get; private set; }


    [field: Header("플레이 & 정지, 스피드업 버튼")]
    [field: SerializeField] public Button ExecuteButton { get; private set; }
    [field: SerializeField] public Button AbortButton { get; private set; }
    [field: SerializeField] public Button TimeControlButton { get; private set; }
    [SerializeField] private Sprite _timeControlOn;
    [SerializeField] private Sprite _timeControlOff;


    [field: Header("코딩블럭 버튼 오브젝트")]
    [field: SerializeField] public Button ForwardButton { get; private set; }
    [field: SerializeField] public Button TurnLeftButton { get; private set; }
    [field: SerializeField] public Button TurnRightButton { get; private set; }
    [field: SerializeField] public Button FunctionButton { get; private set; }
    [field: SerializeField] public Button LoopButton { get; private set; }


    [field: Header("옵션 UI")]
    [field: SerializeField] public Button OptionMenuOpenButton { get; private set; }
    public bool IsOptionMenuOpen { get; private set; } = false;
    [field: SerializeField] public GameObject OptionPanel { get; private set; }
    [field: SerializeField] public Button OptionMenuBackButton { get; private set; }
    [field: SerializeField] public Button OptionMenuExitButton { get; private set; }
    [field: SerializeField] public GameObject TuchBlockPanel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI CityNameObj { get; private set; }


    [field: Header("클리어 메뉴 UI")]
    [field: SerializeField] public GameObject ClearPanel { get; private set; }
    public Vector3 ClearPanelInitPos { get; private set; }
    [field: SerializeField] public Button ClearNextButton { get; private set; }
    [field: SerializeField] public Button ClearBackButton { get; private set; }


    // ============= TEST CODE ============= //

    // 테스트용 코드 (레이아웃 UI(Main, Fucn, Loop) 터치 할 때 팝업 애니메이션 벡터 값)
    private Vector3 initScale = new Vector3(0.9f, 0.9f, 0.9f);
    private Vector3 targetScale = new Vector3(1f, 1f, 1f);

    private Vector3 funcinitScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 functargetScale = new Vector3(0.75f, 0.75f, 0.75f);

    
    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
            Destroy(this.gameObject);
        #endregion

        #region =============================== Coding blocks onClickAddListener
        // 유저가 블록을 클릭하면, ObjectPoolManager의 blockCategory가 해당 블록으로 설정되고 InsertCodingBlock 메서드가 실행됩니다.
        ForwardButton.onClick.AddListener(() => { ObjectPoolManager.Instance.EBlockCategory = ObjectPoolManager.BlockCategory.Forward; InsertCodingBlock(); });
        TurnLeftButton.onClick.AddListener(() => { ObjectPoolManager.Instance.EBlockCategory = ObjectPoolManager.BlockCategory.Left; InsertCodingBlock(); });
        TurnRightButton.onClick.AddListener(() => { ObjectPoolManager.Instance.EBlockCategory = ObjectPoolManager.BlockCategory.Right; InsertCodingBlock(); });
        FunctionButton.onClick.AddListener(() => { ObjectPoolManager.Instance.EBlockCategory = ObjectPoolManager.BlockCategory.Function; InsertCodingBlock(); });
        LoopButton.onClick.AddListener(() => { ObjectPoolManager.Instance.EBlockCategory = ObjectPoolManager.BlockCategory.Loop; InsertCodingBlock(); });
        #endregion =============================================================

        #region =============================== Layout activate onClickAddListener & Layout ImageComponent _variable
        MainLayout.onClick.AddListener(() => SelectMethod(CurrentLayout.Main));
        FunctionLayout.onClick.AddListener(() => SelectMethod(CurrentLayout.Function));
        LoopLayout.onClick.AddListener(() => SelectMethod(CurrentLayout.Loop));

        MainBookmark.onClick.AddListener(() => SelectMethod(CurrentLayout.Main));
        FunctionBookmark.onClick.AddListener(() => SelectMethod(CurrentLayout.Loop));
        LoopBookmark.onClick.AddListener(() => SelectMethod(CurrentLayout.Function));

        MainLayout.TryGetComponent<Image>(out _mainLayoutImageComponent);
        FunctionLayout.TryGetComponent<Image>(out _functionLayoutImageComponent);
        LoopLayout.TryGetComponent<Image>(out _loopLayoutImageComponent);
        #endregion =================================================================================================

        #region =============================== block delete OnClickAddListener 
        MainDelete.onClick.AddListener(() => { SelectMethod(CurrentLayout.Main); DeleteCodingBlock(ECurrentLayout); });
        FunctionDelete.onClick.AddListener(() => { SelectMethod(CurrentLayout.Function); DeleteCodingBlock(ECurrentLayout); });
        LoopDelete.onClick.AddListener(() => { SelectMethod(CurrentLayout.Loop); DeleteCodingBlock(ECurrentLayout); });
        #endregion =============================================================================================

        #region =============================== Execution, Stop & TimeControl & Loop Count Plus, Minus OnClickAddListener
        ExecuteButton.onClick.AddListener(() => ExecuteCodingBlock());
        AbortButton.onClick.AddListener(() => AbortCodingBlock());
        TimeControlButton.onClick.AddListener(() => ControlTimeScale());
        LoopCountPlus.onClick.AddListener(() => ControlLoopCount(true));
        LoopCountMinus.onClick.AddListener(() => ControlLoopCount(false));
        #endregion ======================================================================================================

        #region =============================== Option & Clear OnClickAddListener 
        OptionMenuOpenButton.onClick.AddListener(() => ActiveOption());
        OptionMenuExitButton.onClick.AddListener(() => ActiveOption());
        OptionMenuBackButton.onClick.AddListener(() => StartCoroutine(GameSceneManager.Instance.LoadIndexScene(0)));

        ClearBackButton.onClick.AddListener(() => StartCoroutine(GameSceneManager.Instance.LoadIndexScene(0)));
        ClearNextButton.onClick.AddListener(() => StartCoroutine(GameSceneManager.Instance.LoadNextScene()));
        #endregion ==============================================================

        #region =============================== Transform, Vectors Inits
        ClearPanelInitPos = ClearPanel.transform.localPosition;
        #endregion
    }

    public void SelectMethod(CurrentLayout selectMethod)
    {
        if(selectMethod == ECurrentLayout)
            return;

        AudioManager.Instance.Play_UISFX("SelectMethod");

        switch (selectMethod)
        {
            #region MainLayout Select Code
            case CurrentLayout.Main:

                if (ECurrentLayout != CurrentLayout.Main)
                {
                    // .. MainLayout 활성화 애니메이션
                    MainLayout.transform.parent.transform.localScale = initScale;
                    MainLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                }

                ECurrentLayout = CurrentLayout.Main;

                // .. MainLayout 컬러 변경
                _mainLayoutImageComponent.color = _GREEN_LAYOUT_COLOR;
                _functionLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                _loopLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                break;
            #endregion

            #region FunctionLayout Select Code
            case CurrentLayout.Function:

                if (ECurrentLayout != CurrentLayout.Function)
                {
                    // .. Function 버튼 활성화 & 애니메이션
                    FunctionButton.gameObject.SetActive(true);
                    LoopButton.gameObject.SetActive(false);
                    FunctionButton.transform.localScale = funcinitScale;
                    FunctionButton.transform.DOScale(functargetScale, 0.3f).SetEase(Ease.OutBack);

                    // .. FunctionLayout 활성화 & 애니메이션
                    FunctionLayout.transform.parent.gameObject.SetActive(true);
                    LoopLayout.transform.parent.gameObject.SetActive(false);
                    FunctionLayout.transform.parent.transform.localScale = initScale;
                    FunctionLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                }

                ECurrentLayout = CurrentLayout.Function;

                // .. FunctionLayout 컬러 변경
                _functionLayoutImageComponent.color = _PURPLE_LAYOUT_COLOR;
                _mainLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                _loopLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                break;
            #endregion

            #region LoopLayout Select Code
            case CurrentLayout.Loop:

                if (ECurrentLayout != CurrentLayout.Loop)
                {
                    // .. Loop 버튼 활성화 & 애니메이션
                    LoopButton.gameObject.SetActive(true);
                    FunctionButton.gameObject.SetActive(false);
                    LoopButton.transform.localScale = funcinitScale;
                    LoopButton.transform.DOScale(functargetScale, 0.3f).SetEase(Ease.OutBack);

                    // .. LoopLayout 활성화 & 애니메이션
                    LoopLayout.transform.parent.gameObject.SetActive(true);
                    FunctionLayout.transform.parent.gameObject.SetActive(false);
                    LoopLayout.transform.parent.transform.localScale = initScale;
                    LoopLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                }

                ECurrentLayout = CurrentLayout.Loop;

                // .. LoopLayout 컬러 변경
                _loopLayoutImageComponent.color = _ORANGE_LAYOUT_COLOR;
                _mainLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                _functionLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                break;
                #endregion
        }
    }

    public void InsertCodingBlock()
    {
        AudioManager.Instance.Play_UISFX("InsertCodingBlock");

        // 유저가 클릭한 블록이 함수(Function) 또는 반복문(Loop) 블록인 경우, 이를 MainLayout에 추가합니다.
        if (ObjectPoolManager.Instance.EBlockCategory == ObjectPoolManager.BlockCategory.Function ||  
            ObjectPoolManager.Instance.EBlockCategory == ObjectPoolManager.BlockCategory.Loop) 
        {
            
            if (GameManager.Instance.MainMethodList.Count < 10)
            {
                SelectMethod(CurrentLayout.Main);

                CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.EBlockCategory);
                block.transform.SetParent(MainLayout.transform);

                GameManager.Instance.MainMethodList.Add(block);
                block.enabled = false;
                UIAnimation.Animation_BlockPop(GameManager.Instance.MainMethodList.Last().gameObject);
            }

        }
        else
        {
            // 유저가 클릭한 블록을 오브젝트 풀에서 꺼내어 현재 선택된 레이아웃에 추가합니다.
            switch (ECurrentLayout)
            {
                case CurrentLayout.Main:
                    if (GameManager.Instance.MainMethodList.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 MainLayout에 블록을 넣어줍니다.
                        CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.EBlockCategory);
                        block.transform.SetParent(MainLayout.transform);

                        GameManager.Instance.MainMethodList.Add(block);
                        block.enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.MainMethodList.Last().gameObject);
                    }
                    break;

                case CurrentLayout.Function:
                    if (GameManager.Instance.FunctionMethodList.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 FunctionLayout에 블록을 넣어줍니다.
                        CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.EBlockCategory);
                        block.transform.SetParent(FunctionLayout.transform);

                        GameManager.Instance.FunctionMethodList.Add(block);
                        block.enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.FunctionMethodList.Last().gameObject);
                    }
                    break;

                case CurrentLayout.Loop:
                    if (GameManager.Instance.LoopMethodList.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 LoopLayout에 블록을 넣어줍니다.
                        CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.EBlockCategory);
                        block.transform.SetParent(LoopLayout.transform);

                        GameManager.Instance.LoopMethodList.Add(block);
                        block.enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.LoopMethodList.Last().gameObject);
                    }
                    break;
            }
        }
    }

    public void DeleteCodingBlock(CurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case CurrentLayout.Main:
                if (GameManager.Instance.MainMethodList.Count > 0)
                {
                    AudioManager.Instance.Play_UISFX("DeleteCodingBlock");
                    CodingBlock lastblock = GameManager.Instance.MainMethodList.Last();
                    GameManager.Instance.MainMethodList.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;

            case CurrentLayout.Function:
                if (GameManager.Instance.FunctionMethodList.Count > 0)
                {
                    AudioManager.Instance.Play_UISFX("DeleteCodingBlock");
                    CodingBlock lastblock = GameManager.Instance.FunctionMethodList.Last();
                    GameManager.Instance.FunctionMethodList.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;

            case CurrentLayout.Loop:
                if (GameManager.Instance.LoopMethodList.Count > 0)
                {
                    AudioManager.Instance.Play_UISFX("DeleteCodingBlock");
                    CodingBlock lastblock = GameManager.Instance.LoopMethodList.Last();
                    GameManager.Instance.LoopMethodList.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;
        }
    }

    public void ExecuteCodingBlock()
    {
        GameManager.Instance.IsCompilerRunning = true;

        ExecuteButton.gameObject.SetActive(false);
        AbortButton.gameObject.SetActive(true);
        LockUIElements(true);

        OptionMenuOpenButton.interactable= false;
        OptionMenuOpenButton.transform.DOScale(0, 0.5f).SetEase(Ease.InOutExpo);

        AudioManager.Instance.Play_UISFX("ExecuteButton");
    }

    public void AbortCodingBlock()
    {
        GameManager.Instance.IsCompilerRunning = false;

        GameManager.PlayerManager_Instance.CameraTargetObject.transform.localPosition = GameManager.PlayerManager_Instance.CamTargetStartPosition;

        AudioManager.Instance.Play_UISFX("AbortButton");

        AbortCodingBlocksAnimation();
        GameManager.PlayerManager_Instance.PlayerAnimator.SetBool("WaitEmote", false);
        GameManager.PlayerManager_Instance.ResetPlayerPosition();

        AbortButton.gameObject.SetActive(false);
        UIAnimation.Animation_PlayButtonDelay(ExecuteButton, 1);

        OptionMenuOpenButton.transform.DOScale(1, 0.5f).SetEase(Ease.InOutExpo);
        OptionMenuOpenButton.interactable = true;
    }

    public void ControlTimeScale()
    {
        if (Time.timeScale == 1f)
        {
            TimeControlButton.GetComponent<Image>().sprite = _timeControlOn;
            UIAnimation.Animation_TimeControl(TimeControlButton);
            Time.timeScale = 1.3f;
        }
        else
        {
            TimeControlButton.GetComponent<Image>().sprite = _timeControlOff;
            UIAnimation.Animation_BlockShake(TimeControlButton.gameObject);
            Time.timeScale = 1f;
        }

        AudioManager.Instance.Play_UISFX("TimeControl");
        UIAnimation.Animation_ButtonDelay(TimeControlButton, 1);
    }

    public void ControlLoopCount(bool increase)
    {
        int MinLoopCount = 1;
        int MaxLoopCount = 9;

        switch (increase)
        {
            case true: // LoopReaptCount ++
                if (GameManager.Instance.LoopReaptCount < MaxLoopCount) GameManager.Instance.LoopReaptCount++;
                break;

            case false: // LoopReaptCount --
                if (GameManager.Instance.LoopReaptCount > MinLoopCount) GameManager.Instance.LoopReaptCount--;
                break;
        }

        LoopCountText.text = GameManager.Instance.LoopReaptCount.ToString();
    }

    public void ActiveOption()
    {
        AudioManager.Instance.Play_UISFX("OptionMenuOpen");

        // .. OptionPanel의 현재 활성화 상태를 확인하여 조건을 분기합니다.
        switch (OptionPanel.activeSelf)
        {

            case true:  // OptionPanel이 현재 활성화된 경우 OptionPanel을 비활성화
                OptionMenuOpenButton.interactable = false;
                TuchBlockPanel.SetActive(false);

                OptionPanel.transform.DOScale(0, 0.3f).SetEase(Ease.OutExpo).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        OptionPanel.SetActive(false);
                        OptionMenuOpenButton.interactable = true;
                        IsOptionMenuOpen = false;
                    });
                break;


            case false:  // OptionPanel이 현재 비활성화된 경우 OptionPanel을 활성화
                OptionMenuOpenButton.interactable = false;
                OptionPanel.SetActive(true);
                TuchBlockPanel.SetActive(true);
                OptionPanel.transform.localScale = Vector3.zero;

                OptionPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        OptionMenuOpenButton.interactable = true;
                        IsOptionMenuOpen = true;
                    });
                break;
        }
    }

    public void LockUIElements(bool enable)
    {
        #region Blocks Lock
        ForwardButton.enabled = !enable;
        TurnLeftButton.enabled = !enable;
        TurnRightButton.enabled = !enable;
        FunctionButton.enabled = !enable;
        LoopButton.enabled = !enable;
        #endregion

        #region Layout & Bookmark & Delete Lock
        MainLayout.interactable = !enable;
        FunctionLayout.interactable = !enable;
        LoopLayout.interactable = !enable;

        MainBookmark.interactable = !enable;
        FunctionBookmark.interactable = !enable;
        LoopBookmark.interactable = !enable;

        MainDelete.interactable = !enable;
        FunctionDelete.interactable = !enable;
        LoopDelete.interactable = !enable;

        LoopCountPlus.interactable = !enable;
        LoopCountMinus.interactable = !enable;

        OptionMenuOpenButton.enabled = !enable;
        #endregion
    }

    public void ShakeUIElements()
    {
        UIAnimation.Animation_UIShake(MainLayout.gameObject);
        UIAnimation.Animation_UIShake(MainDelete.gameObject);
        UIAnimation.Animation_UIShake(MainBookmark.gameObject);

        UIAnimation.Animation_UIShake(FunctionLayout.gameObject);
        UIAnimation.Animation_UIShake(FunctionDelete.gameObject);
        UIAnimation.Animation_UIShake(FunctionBookmark.gameObject);

        UIAnimation.Animation_UIShake(LoopLayout.gameObject);
        UIAnimation.Animation_UIShake(LoopDelete.gameObject);
        UIAnimation.Animation_UIShake(LoopBookmark.gameObject);

        UIAnimation.Animation_UIShake(ForwardButton.gameObject);
        UIAnimation.Animation_UIShake(TurnLeftButton.gameObject);
        UIAnimation.Animation_UIShake(TurnRightButton.gameObject);
        UIAnimation.Animation_UIShake(FunctionButton.gameObject);
    }

    public void DisableBlockHighlights()
    {
        if (GameManager.Instance.MainMethodList.Count > 0)
        {
            foreach (CodingBlock block in GameManager.Instance.MainMethodList)
            {
                block.ToggleHighLight(false);
            }

        }

        if (GameManager.Instance.FunctionMethodList.Count > 0)
        {
            foreach (CodingBlock block in GameManager.Instance.FunctionMethodList)
            {
                block.ToggleHighLight(false);
            }
        }

        if (GameManager.Instance.LoopMethodList.Count > 0)
        {
            foreach (CodingBlock block in GameManager.Instance.LoopMethodList)
            {
                block.ToggleHighLight(false);
            }
        }

    }

    public void AbortCodingBlocksAnimation()
    {
        if (GameManager.Instance.MainMethodList.Count > 0)
        {
            foreach (CodingBlock block in GameManager.Instance.MainMethodList)
            {
                UIAnimation.Animation_BlockShake(block.gameObject);
            }
        }

        if (GameManager.Instance.FunctionMethodList.Count > 0)
        {
            foreach (CodingBlock block in GameManager.Instance.FunctionMethodList)
            {
                UIAnimation.Animation_BlockShake(block.gameObject);
            }
        }

        if (GameManager.Instance.LoopMethodList.Count > 0)
        {
            foreach (CodingBlock block in GameManager.Instance.LoopMethodList)
            {
                UIAnimation.Animation_BlockShake(block.gameObject);
            }
        }
    }

    public void Initialize_CodingUIButtonState()
    {
        // 실행 정지 버튼이 표시 상태
        AbortButton.interactable = true;
        AbortButton.gameObject.SetActive(false);
        ExecuteButton.gameObject.SetActive(true);

        // 시간 배속 버튼의 표시 상태
        TimeControlButton.GetComponent<Image>().sprite = _timeControlOff;

        ClearPanel.transform.localPosition = ClearPanelInitPos;

        OptionMenuOpenButton.transform.DOScale(1, 0.5f).SetEase(Ease.InOutExpo);
    }

    public void ActiveStageClearUI()
    {
        UIAnimation.Animation_DelayPopUpButton(ClearBackButton);
        UIAnimation.Animation_DelayPopUpButton(ClearNextButton);
    }


}