using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static GameManager;

public class CodingUIManager : MonoBehaviour
{
    public enum ECurrentLayout
    {
        Main,
        Function,
        Loop,
    }

    public ECurrentLayout currentLayout = ECurrentLayout.Main;
    public UIAnimation UIAnimation { get; private set; } = new UIAnimation();

    [field: Header("비활성화된 오브젝트 풀 오브젝트")]
    [field: SerializeField] public GameObject ReleasedBlocks { get; private set; }


    [field: Header("그리드 레이아웃 오브젝트")]
    [field: SerializeField] public GameObject MainLayout { get; private set; }
    [field: SerializeField] public GameObject FunctionLayout { get; private set; }
    [field: SerializeField] public GameObject LoopLayout { get; private set; }

    private Image _mainLayoutImage;
    private Image _functionLayoutImage;
    private Image _loopLayoutImage;

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

    [field: Header("플레이 & 정지, 스피드업 버튼")]
    [field: SerializeField] public GameObject ExecutionButton { get; private set; }
    [field: SerializeField] public GameObject StopButton { get; private set; }
    [field: SerializeField] public GameObject TimeControlButton { get; private set; }
    [SerializeField] private Sprite _timeControlOn;
    [SerializeField] private Sprite _timeControlOff;

    [field: Header("코딩블럭 버튼 오브젝트")]
    [field: SerializeField] public GameObject ForwardButton { get; private set; }
    [field: SerializeField] public GameObject TurnLeftButton { get; private set; }
    [field: SerializeField] public GameObject TurnRightButton { get; private set; }
    [field: SerializeField] public GameObject FunctionButton { get; private set; }
    [field: SerializeField] public GameObject LoopButton { get; private set; }

    [field: Header("코딩블럭 프리팹")]
    [field: SerializeField] public GameObject ForwardPrefab { get; private set; }
    [field: SerializeField] public GameObject TurnLeftPrefab { get; private set; }
    [field: SerializeField] public GameObject TurnRightPrefab { get; private set; }
    [field: SerializeField] public GameObject FunctionPrefab { get; private set; }
    [field: SerializeField] public GameObject LoopPrefab { get; private set; }

    public CodingBlock LayoutSpawnBlock { get; private set; }


    // 테스트용 코드 (레이아웃 UI(Main, Fucn, Loop) 터치 할 때 팝업 애니메이션 벡터 값)
    private Vector3 initScale = new Vector3(0.9f, 0.9f, 0.9f);
    private Vector3 targetScale = new Vector3(1f, 1f, 1f);

    private Vector3 funcinitScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 functargetScale = new Vector3(0.75f, 0.75f, 0.75f);

    private void Awake()
    {
        #region Coding blocks onClickAddListener
        ForwardButton.GetComponent<Button>().onClick.AddListener(() => { InsertBlock(ForwardPrefab); UIAnimation.Animation_CodingBlockPop(ForwardButton); });
        TurnLeftButton.GetComponent<Button>().onClick.AddListener(() => { InsertBlock(TurnLeftPrefab); UIAnimation.Animation_CodingBlockPop(TurnLeftButton); });
        TurnRightButton.GetComponent<Button>().onClick.AddListener(() => { InsertBlock(TurnRightPrefab); UIAnimation.Animation_CodingBlockPop(TurnRightButton); });
        FunctionButton.GetComponent<Button>().onClick.AddListener(() => { InsertBlock(FunctionPrefab); UIAnimation.Animation_CodingBlockPop(FunctionButton); });
        LoopButton.GetComponent<Button>().onClick.AddListener(() => { InsertBlock(LoopPrefab); UIAnimation.Animation_CodingBlockPop(FunctionButton); });
        #endregion

        #region Layout activate onClickAddListener
        MainLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Main));
        FunctionLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Function));
        LoopLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Loop));

        MainBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Main));
        FunctionBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Loop));
        LoopBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Function));
        #endregion

        #region block delete OnClickAddListener
        MainDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = ECurrentLayout.Main; DeleteBlock(currentLayout); });
        FunctionDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = ECurrentLayout.Function; DeleteBlock(currentLayout); });
        LoopDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = ECurrentLayout.Loop; DeleteBlock(currentLayout); });
        #endregion

        #region Play, Stop & TimeControl OnClickAddListener
        ExecutionButton.GetComponent<Button>().onClick.AddListener(() => GameManager_Instance.Set_ExecutionToggle(true));
        StopButton.GetComponent<Button>().onClick.AddListener(() => StopBlock());
        TimeControlButton.GetComponent<Button>().onClick.AddListener(() => TimeScaleControl());
        #endregion

        MainLayout.TryGetComponent<Image>(out _mainLayoutImage);
        FunctionLayout.TryGetComponent<Image>(out _functionLayoutImage);
        LoopLayout.TryGetComponent<Image>(out _loopLayoutImage);
    }

    private void Start()
    {
        GameManager_Instance.Initialize_CodingMethod();

        GameManager_Instance.Get_UIManager(this.gameObject);

        SelectedMethods(ECurrentLayout.Main);
    }

    public void SelectedMethods(ECurrentLayout selectMethod)
    {
        switch (selectMethod)
        {
            #region MainLayout Select Code
            case ECurrentLayout.Main:
                currentLayout = ECurrentLayout.Main;

                // .. MainLayout 컬러 변경
                _mainLayoutImage.color = _GREEN_LAYOUT_COLOR;
                _functionLayoutImage.color = _GREY_LAYOUT_COLOR;
                _loopLayoutImage.color = _GREY_LAYOUT_COLOR;

                // .. MainLayout 활성화 애니메이션
                MainLayout.transform.parent.transform.localScale = initScale;
                MainLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                break;
            #endregion

            #region FunctionLayout Select Code
            case ECurrentLayout.Function:
                currentLayout = ECurrentLayout.Function;

                // .. FunctionLayout 컬러 변경
                _functionLayoutImage.color = _PURPLE_LAYOUT_COLOR;
                _mainLayoutImage.color = _GREY_LAYOUT_COLOR;
                _loopLayoutImage.color = _GREY_LAYOUT_COLOR;

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
                break;
            #endregion

            #region LoopLayout Select Code
            case ECurrentLayout.Loop:
                currentLayout = ECurrentLayout.Loop;

                // .. LoopLayout 컬러 변경
                _loopLayoutImage.color = _ORANGE_LAYOUT_COLOR;
                _mainLayoutImage.color = _GREY_LAYOUT_COLOR;
                _functionLayoutImage.color = _GREY_LAYOUT_COLOR;

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
                break;
                #endregion
        }
    }
    public void InsertBlock(GameObject prefab)
    {
        LayoutSpawnBlock = prefab.GetComponent<CodingBlock>();

        if (prefab == FunctionPrefab || prefab == LoopPrefab)
        {
            if (GameManager_Instance.MainMethod.Count < 10)
            {
                GameManager_Instance.MainMethod.Add(Instantiate(prefab,MainLayout.transform).GetComponent<CodingBlock>());
                prefab.GetComponent<CodingBlock>().enabled = false;
                UIAnimation.Animation_BlockPop(GameManager_Instance.MainMethod.Last().gameObject);
            }
        }
        else
        {
            switch (currentLayout)
            {
                case ECurrentLayout.Main:
                    if (GameManager_Instance.MainMethod.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 Main 레이아웃에 블록을 넣어줍니다.
                        LayoutSpawnBlock = ObjectPoolManager_Instance.CodingBlockPool.Get();
                        LayoutSpawnBlock.transform.SetParent(MainLayout.transform);

                        GameManager_Instance.MainMethod.Add(LayoutSpawnBlock);
                        LayoutSpawnBlock.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager_Instance.MainMethod.Last().gameObject);
                    }
                    break;

                case ECurrentLayout.Function:
                    if (GameManager_Instance.FunctionMethod.Count < 10)
                    {
                        // .. ObjectPool에서 블록을 가져오고 Function 레이아웃에 블록을 넣어줍니다.
                        LayoutSpawnBlock = ObjectPoolManager_Instance.CodingBlockPool.Get();
                        LayoutSpawnBlock.transform.SetParent(FunctionLayout.transform);

                        GameManager_Instance.FunctionMethod.Add(LayoutSpawnBlock);
                        LayoutSpawnBlock.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager_Instance.FunctionMethod.Last().gameObject);
                    }
                    break;

                case ECurrentLayout.Loop:
                    if (GameManager_Instance.LoopMethod.Count < 10)
                    {
                        GameManager_Instance.LoopMethod.Add(Instantiate(prefab, LoopLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager_Instance.LoopMethod.Last().gameObject);
                    }
                    break;
            }
        }
    }

    
    public void DeleteBlock(ECurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case ECurrentLayout.Main:
                if (GameManager_Instance.MainMethod.Count > 0)
                {
                    CodingBlock lastblock = GameManager_Instance.MainMethod.Last();
                    GameManager_Instance.MainMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;

            case ECurrentLayout.Function:
                if (GameManager_Instance.FunctionMethod.Count > 0)
                {
                    CodingBlock lastblock = GameManager_Instance.FunctionMethod.Last();
                    GameManager_Instance.FunctionMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;

            case ECurrentLayout.Loop:
                if (GameManager_Instance.LoopMethod.Count > 0)
                {
                    CodingBlock lastblock = GameManager_Instance.LoopMethod.Last();
                    GameManager_Instance.LoopMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => lastblock.ReleaseBlock());
                }
                break;
        }
    }

    public void StopBlock()
    {
        ResetBlockAnimation();
        PlayerManager_Instance.PlayerAnimator.SetBool("WaitEmote", false);

        GameManager_Instance.Set_ExecutionToggle(false);
        GameManager_Instance.Set_IsMainMethodRunning(false);

        PlayerManager_Instance.ResetPlayerPosition();

        UIAnimation.Animation_PlayButtonDelay(ExecutionButton, 1);

        StopButton.gameObject.SetActive(false);
        ExecutionButton.gameObject.SetActive(true);
    }
    public void TimeScaleControl()
    {
        UIAnimation.Animation_ButtonDelay(TimeControlButton, 1);

        if (Time.timeScale == 1f)
        {
            TimeControlButton.GetComponent<Image>().sprite = _timeControlOn;
            UIAnimation.Animation_TimeControl(TimeControlButton);
            Time.timeScale = 1.3f;
        }
        else
        {
            TimeControlButton.GetComponent<Image>().sprite = _timeControlOff;
            UIAnimation.Animation_BlockShake(TimeControlButton);
            Time.timeScale = 1f;
        }
    }

    public void LockUIElements(bool enable)
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
        foreach (CodingBlock block in GameManager_Instance.MainMethod)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in GameManager_Instance.FunctionMethod)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in GameManager_Instance.LoopMethod)
        {
            block.ToggleHighLight(false);
        }
    }
    public void ResetBlockAnimation()
    {
        foreach (CodingBlock block in GameManager_Instance.MainMethod)
        {
            UIAnimation.Animation_BlockShake(block.gameObject);
        }
    }
}
