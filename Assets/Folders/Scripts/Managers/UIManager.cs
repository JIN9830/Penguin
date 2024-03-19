using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public enum CurrentLayout
    {
        Main,
        Function,
        Loop,
    }
    public enum SelectMethod
    {
        Function,
        Loop,
    }
    public CurrentLayout currentLayout = CurrentLayout.Main;

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
    public GameObject speedUpButton;
    public GameObject speedDownButton;

    [Header("코딩블럭 버튼 오브젝트")]
    public GameObject forwardButton;
    public GameObject turnLeftButton;
    public GameObject turnRightButton;
    public GameObject functionButton;
    public GameObject loopButton;

    private void Awake()
    {
        GameManager.Instance.Get_UIManager(this.gameObject);

        #region Coding blocks onClickAddListener
        forwardButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(GameManager.Instance.forwardPrefab));
        turnLeftButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(GameManager.Instance.turnLeftPrefab));
        turnRightButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(GameManager.Instance.turnRightPrefab));
        functionButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(GameManager.Instance.functionPrefab));
        loopButton.GetComponent<Button>().onClick.AddListener(() => InsertBlock(GameManager.Instance.loopPrefab));
        #endregion

        #region Layout activate onClickAddListener
        mainLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Main));
        functionLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Function));
        loopLayout.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Loop));

        mainBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Main));
        functionBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Loop));
        loopBookmark.GetComponent<Button>().onClick.AddListener(() => SelectedMethods(CurrentLayout.Function));
        #endregion

        #region block delete OnClickAddListener
        mainDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Main; DeleteBlock(currentLayout); });
        functionDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Function; DeleteBlock(currentLayout); });
        loopDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Loop; DeleteBlock(currentLayout); });
        #endregion

        #region Play, Stop & TimeControl OnClickAddListener
        playButton.GetComponent<Button>().onClick.AddListener(() => PlayBlock_Button());
        stopButton.GetComponent<Button>().onClick.AddListener(() => StopBlock_Button());
        speedUpButton.GetComponent<Button>().onClick.AddListener(() => ToggleTimeScale_Button());
        speedDownButton.GetComponent<Button>().onClick.AddListener(() => ToggleTimeScale_Button());
        #endregion
    }

    private void Start()
    {
        GameManager.Instance.Initialize_CodingMethod();
    }

    private void Update()
    {
        
    }

    public void SelectedMethods(CurrentLayout selectMethod)
    {
        switch (selectMethod)
        {
            case CurrentLayout.Main:
                currentLayout = CurrentLayout.Main;
                break;

            case CurrentLayout.Function:
                currentLayout = CurrentLayout.Function;
                functionButton.gameObject.SetActive(true);
                loopButton.gameObject.SetActive(false);
                functionLayout.transform.parent.gameObject.SetActive(true);
                loopLayout.transform.parent.gameObject.SetActive(false);
                break;

            case CurrentLayout.Loop:
                currentLayout = CurrentLayout.Loop;
                loopButton.gameObject.SetActive(true);
                functionButton.gameObject.SetActive(false);
                loopLayout.transform.parent.gameObject.SetActive(true);
                functionLayout.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
    public void InsertBlock(GameObject prefab)
    {
        if (prefab == GameManager.Instance.functionPrefab || prefab == GameManager.Instance.loopPrefab)
        {
            if (GameManager.Instance.MainMethod.Count < 10)
            {
                GameManager.Instance.MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                prefab.GetComponent<CodingBlock>().enabled = false;
                UIAnimation.Animation_BlockPop(GameManager.Instance.MainMethod.Last().gameObject);
            }
        }
        else
        {
            switch (currentLayout)
            {
                case CurrentLayout.Main:
                    if (GameManager.Instance.MainMethod.Count < 10)
                    {
                        GameManager.Instance.MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.MainMethod.Last().gameObject);
                    }
                    break;

                case CurrentLayout.Function:
                    if (GameManager.Instance.Function.Count < 10)
                    {
                        GameManager.Instance.Function.Add(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.Function.Last().gameObject);
                    }
                    break;

                case CurrentLayout.Loop:
                    if (GameManager.Instance.Loop.Count < 10)
                    {
                        GameManager.Instance.Loop.Add(Instantiate(prefab, loopLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(GameManager.Instance.Loop.Last().gameObject);
                    }
                    break;
            }
        }
    }
    public void DeleteBlock(CurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case CurrentLayout.Main:
                if (GameManager.Instance.MainMethod.Count > 0)
                {
                    CodingBlock lastblock = GameManager.Instance.MainMethod.Last();
                    GameManager.Instance.MainMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case CurrentLayout.Function:
                if (GameManager.Instance.Function.Count > 0)
                {
                    CodingBlock lastblock = GameManager.Instance.Function.Last();
                    GameManager.Instance.Function.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case CurrentLayout.Loop:
                if (GameManager.Instance.Loop.Count > 0)
                {
                    CodingBlock lastblock = GameManager.Instance.Loop.Last();
                    GameManager.Instance.Loop.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;
        }
    }


    public void PlayBlock_Button()
    {
        GameManager.Instance.PlayToggle = true;
    }
    public void StopBlock_Button()
    {
        GameManager.Instance.PlayToggle = false;
        GameManager.Instance.IsBlocksRunning = false;

        GameManager.Instance.PlayerManager.ResetPlayerPosition();

        DisableBlockHighlights();
        ResetBlockAnimation();
        UIAnimation.Animation_BlockPop(playButton);
        stopButton.gameObject.SetActive(false);
    }
    public void ToggleTimeScale_Button()
    {
        if (Time.timeScale == 1f)
        {
            speedDownButton.SetActive(true);
            Time.timeScale = 1.3f;
        }
        else
        {
            speedDownButton.SetActive(false);
            UIAnimation.Animation_BlockShake(speedUpButton);
            Time.timeScale = 1f;
        }
    }

    public void Lock_UIElements(bool enable)
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
    public void Shake_UIElements()
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
        foreach (CodingBlock block in GameManager.Instance.MainMethod)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in GameManager.Instance.Function)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in GameManager.Instance.Loop)
        {
            block.ToggleHighLight(false);
        }
    }
    public void ResetBlockAnimation()
    {
        foreach (CodingBlock block in GameManager.Instance.MainMethod)
        {
            UIAnimation.Animation_BlockShake(block.gameObject);
        }
    }
}
