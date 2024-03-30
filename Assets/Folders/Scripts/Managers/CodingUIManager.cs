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

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("그리드 레이아웃 오브젝트")]
    public GameObject mainLayout;
    public GameObject functionLayout;
    public GameObject loopLayout;

    [Header("블럭 삭제 버튼")]
    public GameObject mainDelete;
    public GameObject functionDelete;
    public GameObject loopDelete;

    [Header("북마크 오브젝트")]
    public GameObject mainBookmark;
    public GameObject functionBookmark;
    public GameObject loopBookmark;

    [Header("플레이 & 정지, 스피드업 버튼")]
    public GameObject playButton;
    public GameObject stopButton;
    public GameObject timeControlButton;

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
        playButton.GetComponent<Button>().onClick.AddListener(() => GameManager_Instance.Set_PlayToggle(true));
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
                break;

            case ECurrentLayout.Function:
                currentLayout = ECurrentLayout.Function;
                functionButton.gameObject.SetActive(true);
                loopButton.gameObject.SetActive(false);
                functionLayout.transform.parent.gameObject.SetActive(true);
                loopLayout.transform.parent.gameObject.SetActive(false);
                break;

            case ECurrentLayout.Loop:
                currentLayout = ECurrentLayout.Loop;
                loopButton.gameObject.SetActive(true);
                functionButton.gameObject.SetActive(false);
                loopLayout.transform.parent.gameObject.SetActive(true);
                functionLayout.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
    public void InsertBlock(GameObject prefab)
    {
        if (prefab == functionPrefab || prefab == loopPrefab)
        {
            if (GameManager_Instance.MainMethod.Count < 10)
            {
                GameManager_Instance.MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
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

        GameManager_Instance.Set_PlayToggle(false);
        GameManager_Instance.Set_IsMainMethodRunning(false);

        PlayerManager_Instance.ResetPlayerPosition();

        UIAnimation.Animation_PlayBlockDelay(playButton, 1);

        stopButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
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
