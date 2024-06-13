using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CodingUIManager : MonoBehaviour
{
    public enum ECurrentLayout
    {
        Main,
        Function,
        Loop,
    } 
    public ECurrentLayout currentLayout { get; private set; }


    public static CodingUIManager Instance { get; private set; }

    public UIAnimation UIAnimation { get; private set; } = new UIAnimation();

    [field: SerializeField] public GameObject ReleasedBlocks { get; private set; }

    [field: SerializeField] public GameObject CodingUICanvas { get; private set; }


    [field: Header("그리드 레이아웃 오브젝트")]
    [field: SerializeField] public GameObject MainLayout { get; private set; }
    [field: SerializeField] public GameObject FunctionLayout { get; private set; }
    [field: SerializeField] public GameObject LoopLayout { get; private set; }

    private Image _mainLayoutImageComponent, _functionLayoutImageComponent, _loopLayoutImageComponent;
    private readonly Color _GREY_LAYOUT_COLOR = new Color32(135, 135, 135, 125);
    private readonly Color _GREEN_LAYOUT_COLOR = new Color32(122, 149, 113, 125);
    private readonly Color _PURPLE_LAYOUT_COLOR = new Color32(122, 104, 142, 125);
    private readonly Color _ORANGE_LAYOUT_COLOR = new Color32(186, 150, 118, 125);


    [field: Header("블럭 삭제 버튼")]
    [field: SerializeField] public GameObject MainDelete { get; private set; }
    [field: SerializeField] public GameObject FunctionDelete { get; private set; }
    [field: SerializeField] public GameObject LoopDelete { get; private set; }


    [field: Header("북마크 오브젝트")]
    [field: SerializeField] public GameObject MainBookmark { get; private set; }
    [field: SerializeField] public GameObject FunctionBookmark { get; private set; }
    [field: SerializeField] public GameObject LoopBookmark { get; private set; }
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
    [field: SerializeField] public GameObject ForwardButton { get; private set; }
    [field: SerializeField] public GameObject TurnLeftButton { get; private set; }
    [field: SerializeField] public GameObject TurnRightButton { get; private set; }
    [field: SerializeField] public GameObject FunctionButton { get; private set; }
    [field: SerializeField] public GameObject LoopButton { get; private set; }


    [field: Header("옵션 UI")]
    [field: SerializeField] public Button OptionMenuOpenButton { get; private set; }

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

        MainLayout.TryGetComponent<Image>(out _mainLayoutImageComponent);
        FunctionLayout.TryGetComponent<Image>(out _functionLayoutImageComponent);
        LoopLayout.TryGetComponent<Image>(out _loopLayoutImageComponent);

        // .. 버튼들의 클릭 이벤트 함수 등록
        #region Coding blocks onClickAddListener
        // 유저가 블록을 클릭하면, ObjectPoolManager의 blockCategory가 해당 블록으로 설정되고 InsertCodingBlock 메서드가 실행됩니다.
        ForwardButton.GetComponent<Button>().onClick.AddListener(() => { ObjectPoolManager.Instance.blockCategory = ObjectPoolManager.BlockCategory.Forward; InsertCodingBlock(); });
        TurnLeftButton.GetComponent<Button>().onClick.AddListener(() => { ObjectPoolManager.Instance.blockCategory = ObjectPoolManager.BlockCategory.Left; InsertCodingBlock(); });
        TurnRightButton.GetComponent<Button>().onClick.AddListener(() => { ObjectPoolManager.Instance.blockCategory = ObjectPoolManager.BlockCategory.Right; InsertCodingBlock(); });
        FunctionButton.GetComponent<Button>().onClick.AddListener(() => { ObjectPoolManager.Instance.blockCategory = ObjectPoolManager.BlockCategory.Function; InsertCodingBlock(); });
        LoopButton.GetComponent<Button>().onClick.AddListener(() => { ObjectPoolManager.Instance.blockCategory = ObjectPoolManager.BlockCategory.Loop; InsertCodingBlock(); });
        #endregion

        #region Layout activate onClickAddListener
        MainLayout.GetComponent<Button>().onClick.AddListener(() => SelectMethod(ECurrentLayout.Main));
        FunctionLayout.GetComponent<Button>().onClick.AddListener(() => SelectMethod(ECurrentLayout.Function));
        LoopLayout.GetComponent<Button>().onClick.AddListener(() => SelectMethod(ECurrentLayout.Loop));

        MainBookmark.GetComponent<Button>().onClick.AddListener(() => SelectMethod(ECurrentLayout.Main));
        FunctionBookmark.GetComponent<Button>().onClick.AddListener(() => SelectMethod(ECurrentLayout.Loop));
        LoopBookmark.GetComponent<Button>().onClick.AddListener(() => SelectMethod(ECurrentLayout.Function));
        #endregion

        #region block delete OnClickAddListener
        MainDelete.GetComponent<Button>().onClick.AddListener(() => { SelectMethod(ECurrentLayout.Main); DeleteCodingBlock(currentLayout); });
        FunctionDelete.GetComponent<Button>().onClick.AddListener(() => { SelectMethod(ECurrentLayout.Function); DeleteCodingBlock(currentLayout); });
        LoopDelete.GetComponent<Button>().onClick.AddListener(() => { SelectMethod(ECurrentLayout.Loop); DeleteCodingBlock(currentLayout); });
        #endregion

        #region Execution, Stop & TimeControl & Loop Count Plus, Minus OnClickAddListener
        ExecuteButton.onClick.AddListener(() => ExecuteCodingBlock());
        AbortButton.onClick.AddListener(() => AbortCodingBlock());
        TimeControlButton.onClick.AddListener(() => ControlTimeScale());
        LoopCountPlus.onClick.AddListener(() => ControlLoopCount(true));
        LoopCountMinus.onClick.AddListener(() => ControlLoopCount(false));
        #endregion

        #region Option & Clear OnClickAddListener
        OptionMenuOpenButton.onClick.AddListener(() => ActiveOption());
        OptionMenuExitButton.onClick.AddListener(() => ActiveOption());
        OptionMenuBackButton.onClick.AddListener(() => StartCoroutine(GameSceneManager.Instance.LoadIndexScene(0)));

        ClearBackButton.onClick.AddListener(() => StartCoroutine(GameSceneManager.Instance.LoadIndexScene(0)));
        ClearNextButton.onClick.AddListener(() => StartCoroutine(GameSceneManager.Instance.LoadNextScene()));
        #endregion

        ClearPanelInitPos = ClearPanel.transform.localPosition;

    }

    public void SelectMethod(ECurrentLayout selectMethod)
    {
        if(selectMethod == currentLayout)
            return;

        AudioManager.Instance.Play_UISFX("SelectMethod");

        switch (selectMethod)
        {
            #region MainLayout Select Code
            case ECurrentLayout.Main:

                if (currentLayout != ECurrentLayout.Main)
                {
                    // .. MainLayout 활성화 애니메이션
                    MainLayout.transform.parent.transform.localScale = initScale;
                    MainLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                }

                currentLayout = ECurrentLayout.Main;

                // .. MainLayout 컬러 변경
                _mainLayoutImageComponent.color = _GREEN_LAYOUT_COLOR;
                _functionLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                _loopLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                break;
            #endregion

            #region FunctionLayout Select Code
            case ECurrentLayout.Function:

                if (currentLayout != ECurrentLayout.Function)
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

                currentLayout = ECurrentLayout.Function;

                // .. FunctionLayout 컬러 변경
                _functionLayoutImageComponent.color = _PURPLE_LAYOUT_COLOR;
                _mainLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                _loopLayoutImageComponent.color = _GREY_LAYOUT_COLOR;
                break;
            #endregion

            #region LoopLayout Select Code
            case ECurrentLayout.Loop:

                if (currentLayout != ECurrentLayout.Loop)
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

                currentLayout = ECurrentLayout.Loop;

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
        if (ObjectPoolManager.Instance.blockCategory == ObjectPoolManager.BlockCategory.Function ||  
            ObjectPoolManager.Instance.blockCategory == ObjectPoolManager.BlockCategory.Loop) 
        {
            
            if (GameManager.Instance.MainMethodList.Count < 10)
            {
                SelectMethod(ECurrentLayout.Main);

                CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.blockCategory);
                block.transform.SetParent(MainLayout.transform);

                GameManager.Instance.MainMethodList.Add(block);
                block.enabled = false;
                UIAnimation.Animation_BlockPop(GameManager.Instance.MainMethodList.Last().gameObject);
            }

        }
        else
        {
            // 유저가 클릭한 블록을 오브젝트 풀에서 꺼내어 현재 선택된 레이아웃에 추가합니다.
            switch (currentLayout)
            {
                case ECurrentLayout.Main:
                    if (GameManager.Instance.MainMethodList.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 MainLayout에 블록을 넣어줍니다.
                        CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.blockCategory);
                        block.transform.SetParent(MainLayout.transform);

                        GameManager.Instance.MainMethodList.Add(block);
                        block.enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.MainMethodList.Last().gameObject);
                    }
                    break;

                case ECurrentLayout.Function:
                    if (GameManager.Instance.FunctionMethodList.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 FunctionLayout에 블록을 넣어줍니다.
                        CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.blockCategory);
                        block.transform.SetParent(FunctionLayout.transform);

                        GameManager.Instance.FunctionMethodList.Add(block);
                        block.enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.FunctionMethodList.Last().gameObject);
                    }
                    break;

                case ECurrentLayout.Loop:
                    if (GameManager.Instance.LoopMethodList.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 LoopLayout에 블록을 넣어줍니다.
                        CodingBlock block = ObjectPoolManager.Instance.SelectBlockFromPool(ObjectPoolManager.Instance.blockCategory);
                        block.transform.SetParent(LoopLayout.transform);

                        GameManager.Instance.LoopMethodList.Add(block);
                        block.enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.LoopMethodList.Last().gameObject);
                    }
                    break;
            }
        }
    }

    public void DeleteCodingBlock(ECurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case ECurrentLayout.Main:
                if (GameManager.Instance.MainMethodList.Count > 0)
                {
                    AudioManager.Instance.Play_UISFX("DeleteCodingBlock");
                    CodingBlock lastblock = GameManager.Instance.MainMethodList.Last();
                    GameManager.Instance.MainMethodList.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;

            case ECurrentLayout.Function:
                if (GameManager.Instance.FunctionMethodList.Count > 0)
                {
                    AudioManager.Instance.Play_UISFX("DeleteCodingBlock");
                    CodingBlock lastblock = GameManager.Instance.FunctionMethodList.Last();
                    GameManager.Instance.FunctionMethodList.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;

            case ECurrentLayout.Loop:
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

        switch (OptionPanel.activeSelf)
        {
            // .. 옵션 UI가 켜져 있으면 옵션 UI를 끈다.
            case true:
                OptionMenuOpenButton.interactable = false;
                TuchBlockPanel.SetActive(false);

                OptionPanel.transform.DOScale(0, 0.3f).SetEase(Ease.OutExpo).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        OptionPanel.SetActive(false);
                        OptionMenuOpenButton.interactable = true;
                    });
                break;

            // .. 옵션 UI가 꺼져 있으면 옵션 UI를 킨다.
            case false:
                OptionMenuOpenButton.interactable = false;
                OptionPanel.SetActive(true);
                TuchBlockPanel.SetActive(true);
                OptionPanel.transform.localScale = Vector3.zero;

                OptionPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        OptionMenuOpenButton.interactable = true;
                    });
                break;
        }
    }

    public void LockUIElements(bool enable) // GetComponent 메서드 사용 수정하기
    {
        #region Blocks Lock
        ForwardButton.GetComponent<Button>().enabled = !enable;
        TurnLeftButton.GetComponent<Button>().enabled = !enable;
        TurnRightButton.GetComponent<Button>().enabled = !enable;
        FunctionButton.GetComponent<Button>().enabled = !enable;
        LoopButton.GetComponent<Button>().enabled = !enable;
        #endregion

        #region Layout & Bookmark & Delete Lock
        MainLayout.GetComponent<Button>().interactable = !enable;
        FunctionLayout.GetComponent<Button>().interactable = !enable;
        LoopLayout.GetComponent<Button>().interactable = !enable;

        MainBookmark.GetComponent<Button>().interactable = !enable;
        FunctionBookmark.GetComponent<Button>().interactable = !enable;
        LoopBookmark.GetComponent<Button>().interactable = !enable;

        MainDelete.GetComponent<Button>().interactable = !enable;
        FunctionDelete.GetComponent<Button>().interactable = !enable;
        LoopDelete.GetComponent<Button>().interactable = !enable;

        LoopCountPlus.interactable = !enable;
        LoopCountMinus.interactable = !enable;

        CodingUIManager.Instance.OptionMenuOpenButton.enabled = !enable;
        #endregion
    }

    public void ShakeUIElements()
    {
        UIAnimation.Animation_UIShake(MainLayout);
        UIAnimation.Animation_UIShake(MainDelete);
        UIAnimation.Animation_UIShake(MainBookmark);

        UIAnimation.Animation_UIShake(FunctionLayout);
        UIAnimation.Animation_UIShake(FunctionDelete);
        UIAnimation.Animation_UIShake(FunctionBookmark);

        UIAnimation.Animation_UIShake(LoopLayout);
        UIAnimation.Animation_UIShake(LoopDelete);
        UIAnimation.Animation_UIShake(LoopBookmark);

        UIAnimation.Animation_UIShake(ForwardButton);
        UIAnimation.Animation_UIShake(TurnLeftButton);
        UIAnimation.Animation_UIShake(TurnRightButton);
        UIAnimation.Animation_UIShake(FunctionButton);
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
        AbortButton.GetComponent<Button>().interactable = true;
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