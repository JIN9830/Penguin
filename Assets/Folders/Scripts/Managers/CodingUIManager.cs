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

    [field: Header("캔버스 오브젝트")]
    [field: SerializeField] public GameObject canvas { get; private set; }


    [field: Header("그리드 레이아웃 오브젝트")]
    [field: SerializeField] public GameObject mainLayout { get; private set; }
    [field: SerializeField] public GameObject functionLayout { get; private set; }
    [field: SerializeField] public GameObject loopLayout { get; private set; }

    private Image _mainLayoutImage;
    private Image _functionLayoutImage;
    private Image _loopLayoutImage;

    private Image _mainselectedImage;
    private Image _functionselectedImage;
    private Image _loopselectedImage;

    [field: Header("블럭 삭제 버튼")]
    [field: SerializeField] public GameObject mainDelete { get; private set; }
    [field: SerializeField] public GameObject functionDelete { get; private set; }
    [field: SerializeField] public GameObject loopDelete { get; private set; }

    [field: Header("북마크 오브젝트")]
    [field: SerializeField] public GameObject mainBookmark { get; private set; }
    [field: SerializeField] public GameObject functionBookmark { get; private set; }
    [field: SerializeField] public GameObject loopBookmark { get; private set; }

    [field: Header("플레이 & 정지, 스피드업 버튼")]
    [field: SerializeField] public GameObject executionButton { get; private set; }
    [field: SerializeField] public GameObject stopButton { get; private set; }
    [field: SerializeField] public GameObject timeControlButton { get; private set; }

    [field: Header("코딩블럭 버튼 오브젝트")]
    [field: SerializeField] public GameObject forwardButton { get; private set; }
    [field: SerializeField] public GameObject turnLeftButton { get; private set; }
    [field: SerializeField] public GameObject turnRightButton { get; private set; }
    [field: SerializeField] public GameObject functionButton { get; private set; }
    [field: SerializeField] public GameObject loopButton { get; private set; }

    [field: Header("코딩블럭 프리팹")]
    [field: SerializeField] public GameObject forwardPrefab { get; private set; }
    [field: SerializeField] public GameObject turnLeftPrefab { get; private set; }
    [field: SerializeField] public GameObject turnRightPrefab { get; private set; }
    [field: SerializeField] public GameObject functionPrefab { get; private set; }
    [field: SerializeField] public GameObject loopPrefab { get; private set; }

    private Vector3 initScale = new Vector3(0.9f, 0.9f, 0.9f);
    private Vector3 targetScale = new Vector3(1f, 1f, 1f);

    private Vector3 funcinitScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 functargetScale = new Vector3(0.75f, 0.75f, 0.75f);



    private void Start()
    {
        GameManager_Instance.Get_UIManager(this.gameObject);

        #region Coding blocks onClickAddListener
        forwardButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(forwardPrefab));
        turnLeftButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(turnLeftPrefab));
        turnRightButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(turnRightPrefab));
        functionButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(functionPrefab));
        loopButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(loopPrefab));
        #endregion

        #region Layout activate onClickAddListener
        mainLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Main));
        functionLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Function));
        loopLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Loop));

        mainBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Main));
        functionBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Loop));
        loopBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(ECurrentLayout.Function));
        #endregion

        #region block delete OnClickAddListener
        mainDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = ECurrentLayout.Main; DeleteBlock(currentLayout); });
        functionDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = ECurrentLayout.Function; DeleteBlock(currentLayout); });
        loopDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = ECurrentLayout.Loop; DeleteBlock(currentLayout); });
        #endregion

        #region Play, Stop & TimeControl OnClickAddListener
        executionButton.GetComponent<Button>().onClick.AddListener(() => GameManager_Instance.Set_ExecutionToggle(true));
        stopButton.GetComponent<Button>().onClick.AddListener(() => StopBlock());
        timeControlButton.GetComponent<Button>().onClick.AddListener(() => TimeScaleButton());
        #endregion

        GameManager_Instance.Initialize_CodingMethod();
    }

    public void SelectedMethods(ECurrentLayout selectMethod)
    {
        switch (selectMethod)
        {
            case ECurrentLayout.Main:
                currentLayout = ECurrentLayout.Main;

                mainLayout.transform.parent.transform.localScale = initScale;
                mainLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                break;

            case ECurrentLayout.Function:
                currentLayout = ECurrentLayout.Function;

                functionButton.gameObject.SetActive(true);
                loopButton.gameObject.SetActive(false);
                functionButton.transform.localScale = funcinitScale;
                functionButton.transform.DOScale(functargetScale, 0.3f).SetEase(Ease.OutBack);

                functionLayout.transform.parent.gameObject.SetActive(true);
                loopLayout.transform.parent.gameObject.SetActive(false);
                functionLayout.transform.parent.transform.localScale = initScale;
                functionLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                break;

            case ECurrentLayout.Loop:
                currentLayout = ECurrentLayout.Loop;

                loopButton.gameObject.SetActive(true);
                functionButton.gameObject.SetActive(false);
                loopButton.transform.localScale = funcinitScale;
                loopButton.transform.DOScale(functargetScale, 0.3f).SetEase(Ease.OutBack);

                loopLayout.transform.parent.gameObject.SetActive(true);
                functionLayout.transform.parent.gameObject.SetActive(false);
                loopLayout.transform.parent.transform.localScale = initScale;
                loopLayout.transform.parent.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
                break;
        }
    }
    public void InsertBlock(GameObject prefab)
    {
        if (prefab == functionPrefab || prefab == loopPrefab)
        {
            if (GameManager_Instance.MainMethod.Count < 10)
            {
                GameManager_Instance.MainMethod.Add(Instantiate(prefab,mainLayout.transform).GetComponent<CodingBlock>());
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
                        GameManager_Instance.MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager_Instance.MainMethod.Last().gameObject);
                    }
                    break;

                case ECurrentLayout.Function:
                    if (GameManager_Instance.FunctionMethod.Count < 10)
                    {
                        GameManager_Instance.FunctionMethod.Add(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager_Instance.FunctionMethod.Last().gameObject);
                    }
                    break;

                case ECurrentLayout.Loop:
                    if (GameManager_Instance.LoopMethod.Count < 10)
                    {
                        GameManager_Instance.LoopMethod.Add(Instantiate(prefab, loopLayout.transform).GetComponent<CodingBlock>());
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
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case ECurrentLayout.Function:
                if (GameManager_Instance.FunctionMethod.Count > 0)
                {
                    CodingBlock lastblock = GameManager_Instance.FunctionMethod.Last();
                    GameManager_Instance.FunctionMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case ECurrentLayout.Loop:
                if (GameManager_Instance.LoopMethod.Count > 0)
                {
                    CodingBlock lastblock = GameManager_Instance.LoopMethod.Last();
                    GameManager_Instance.LoopMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;
        }
    }

    public void StopBlock()
    {
        ResetBlockAnimation();

        GameManager_Instance.Set_ExecutionToggle(false);
        GameManager_Instance.Set_IsMainMethodRunning(false);

        PlayerManager_Instance.ResetPlayerPosition();

        UIAnimation.Animation_PlayBlockDelay(executionButton, 1);

        stopButton.gameObject.SetActive(false);
        executionButton.gameObject.SetActive(true);
    }
    public void TimeScaleButton()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 1.4f;
        }
        else
        {
            UIAnimation.Animation_BlockShake(timeControlButton);
            Time.timeScale = 1f;
        }
    }

    public void LockUIElements(bool enable)
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
    public void ShakeUIElements()
    {
        UIAnimation.Animation_UIShake(mainLayout);
        UIAnimation.Animation_UIShake(mainDelete);
        UIAnimation.Animation_UIShake(mainBookmark);

        UIAnimation.Animation_UIShake(functionLayout);
        UIAnimation.Animation_UIShake(functionDelete);
        UIAnimation.Animation_UIShake(functionBookmark);

        UIAnimation.Animation_UIShake(loopLayout);
        UIAnimation.Animation_UIShake(loopDelete);
        UIAnimation.Animation_UIShake(loopBookmark);

        UIAnimation.Animation_UIShake(forwardButton);
        UIAnimation.Animation_UIShake(turnLeftButton);
        UIAnimation.Animation_UIShake(turnRightButton);
        UIAnimation.Animation_UIShake(functionButton);
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
