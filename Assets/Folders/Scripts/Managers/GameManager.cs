using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public UIManager UI { get; private set; }
    public UIAnimation UIAnimation { get; private set; } = new UIAnimation();

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Function { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Loop { get; private set; } = new List<CodingBlock>();

    public bool PlayBlockToggle { get; private set; } = false;
    public bool PlayBlockInProgress { get; private set; } = false;

    private Coroutine playBlock;

    public readonly WaitForSeconds waitForSeconds = new(1.0f);
    public readonly WaitForSeconds waitForHalfSeconds = new(0.5f);
    public readonly WaitForSeconds waitForPointEightSeconds = new(0.8f);
    public WaitUntil waitUntilPlayToggle;

    [Header("코딩블럭 프리팹")]
    public GameObject forwardPrefab;
    public GameObject turnLeftPrefab;
    public GameObject turnRightPrefab;
    public GameObject functionPrefab;
    public GameObject loopPrefab;

    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("GameManager is Created!");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("GameManager is Destroyed");
        }
        #endregion

        waitUntilPlayToggle = new WaitUntil(() => PlayBlockToggle == true);

        playBlock = StartCoroutine(PlayBlock_Coroutine());
    }

    public void SelectedMethods(UIManager.CurrentLayout selectMethod)
    {
        switch (selectMethod)
        {
            case UIManager.CurrentLayout.Main:
                UI.currentLayout = UIManager.CurrentLayout.Main;
                break;

            case UIManager.CurrentLayout.Function:
                UI.currentLayout = UIManager.CurrentLayout.Function;
                UI.functionButton.gameObject.SetActive(true);
                UI.loopButton.gameObject.SetActive(false);
                UI.functionLayout.transform.parent.gameObject.SetActive(true);
                UI.loopLayout.transform.parent.gameObject.SetActive(false);
                break;

            case UIManager.CurrentLayout.Loop:
                UI.currentLayout = UIManager.CurrentLayout.Loop;
                UI.loopButton.gameObject.SetActive(true);
                UI.functionButton.gameObject.SetActive(false);
                UI.loopLayout.transform.parent.gameObject.SetActive(true);
                UI.functionLayout.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
    public void InsertBlock(GameObject prefab)
    {
        if (prefab == functionPrefab || prefab == loopPrefab)
        {
            if (MainMethod.Count < 10)
            {
                MainMethod.Add(Instantiate(prefab, UI.mainLayout.transform).GetComponent<CodingBlock>());
                prefab.GetComponent<CodingBlock>().enabled = false;
                UIAnimation.Animation_BlockPop(MainMethod.Last().gameObject);
            }
        }
        else
        {
            switch (UI.currentLayout)
            {
                case UIManager.CurrentLayout.Main:
                    if (MainMethod.Count < 10)
                    {
                        MainMethod.Add(Instantiate(prefab, UI.mainLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(MainMethod.Last().gameObject);
                    }
                    break;

                case UIManager.CurrentLayout.Function:
                    if (Function.Count < 10)
                    {
                        Function.Add(Instantiate(prefab, UI.functionLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(Function.Last().gameObject);
                    }
                    break;

                case UIManager.CurrentLayout.Loop:
                    if (Loop.Count < 10)
                    {
                        Loop.Add(Instantiate(prefab, UI.loopLayout.transform).GetComponent<CodingBlock>());
                        prefab.GetComponent<CodingBlock>().enabled = false;
                        UIAnimation.Animation_BlockPop(Loop.Last().gameObject);
                    }
                    break;
            }
        }
    }
    public void DeleteBlock(UIManager.CurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case UIManager.CurrentLayout.Main:
                if (MainMethod.Count > 0)
                {
                    CodingBlock lastblock = MainMethod.Last();
                    MainMethod.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case UIManager.CurrentLayout.Function:
                if (Function.Count > 0)
                {
                    CodingBlock lastblock = Function.Last();
                    Function.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;

            case UIManager.CurrentLayout.Loop:
                if (Loop.Count > 0)
                {
                    CodingBlock lastblock = Loop.Last();
                    Loop.Remove(lastblock);
                    lastblock.gameObject.transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(lastblock.gameObject));
                }
                break;
        }
    }


    public void PlayBlock()
    {
        PlayBlockToggle = true;
        UI.stopButton.gameObject.SetActive(true);
    }
    public IEnumerator PlayBlock_Coroutine()
    {
        while (true)
        {
            if (!PlayBlockToggle) yield return waitUntilPlayToggle;

            if (!PlayBlockInProgress)
            {
                PlayBlockInProgress = true;
                Lock_UIElements(true);

                foreach (CodingBlock block in MainMethod)
                {
                    if (!PlayBlockInProgress)
                        break;

                    yield return waitForHalfSeconds;

                    PlayerManager.InitializePlayerMoveVector();
                    block.GetComponent<CodingBlock>().enabled = true;

                    if (block.gameObject.tag == "Method")
                    {
                        block.MoveOrder();
                        yield return block.StartCoroutine(block.Subroutine());
                    }
                    else
                    {
                        block.MoveOrder();
                    }


                    if (PlayBlockInProgress) yield return waitForPointEightSeconds;
                }

                if (PlayBlockInProgress) yield return waitForSeconds;

                PlayBlockToggle = false;
                PlayBlockInProgress = false;

                DisableBlockHighlights();
                Lock_UIElements(false);
            }
        }
    }
    public void StopBlock()
    {
        PlayBlockToggle = false;
        PlayBlockInProgress = false;

        PlayerManager.ResetPlayerPosition();

        DisableBlockHighlights();
        ResetBlockAnimation();
        UIAnimation.Animation_BlockPop(UI.playButton);
        UI.stopButton.gameObject.SetActive(false);
    }
    public void ToggleTimeScale()
    {
        if (Time.timeScale == 1f)
        {
            UI.speedDownButton.SetActive(true);
            Time.timeScale = 1.3f;
        }
        else
        {
            UI.speedDownButton.SetActive(false);
            UIAnimation.Animation_BlockShake(UI.speedUpButton);
            Time.timeScale = 1f;
        }
    }


    public void DisableBlockHighlights()
    {
        foreach (CodingBlock block in MainMethod)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in Function)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in Loop)
        {
            block.ToggleHighLight(false);
        }
    }
    public void ResetBlockAnimation()
    {
        foreach (CodingBlock block in MainMethod)
        {
            UIAnimation.Animation_BlockShake(block.gameObject);
        }
    }


    public void Lock_UIElements(bool enable)
    {
        #region Blocks Lock
        UI.forwardButton.GetComponent<Button>().enabled = !enable;
        UI.turnLeftButton.GetComponent<Button>().enabled = !enable;
        UI.turnRightButton.GetComponent<Button>().enabled = !enable;
        UI.functionButton.GetComponent<Button>().enabled = !enable;
        UI.loopButton.GetComponent<Button>().enabled = !enable;
        #endregion

        #region Layout & Bookmark & Delete Lock
        UI.mainLayout.GetComponent<Button>().interactable = !enable;
        UI.functionLayout.GetComponent<Button>().interactable = !enable;
        UI.loopLayout.GetComponent<Button>().interactable = !enable;

        UI.mainBookmark.GetComponent<Button>().interactable = !enable;
        UI.functionBookmark.GetComponent<Button>().interactable = !enable;
        UI.loopBookmark.GetComponent<Button>().interactable = !enable;

        UI.mainDelete.GetComponent<Button>().interactable = !enable;
        UI.functionDelete.GetComponent<Button>().interactable = !enable;
        UI.loopDelete.GetComponent<Button>().interactable = !enable;
        #endregion
    }
    public void Shake_UIElements()
    {
        UIAnimation.Animation_UIShake(UI.mainLayout);
        UIAnimation.Animation_UIShake(UI.mainDelete);
        UIAnimation.Animation_UIShake(UI.mainBookmark);

        UIAnimation.Animation_UIShake(UI.functionLayout);
        UIAnimation.Animation_UIShake(UI.functionDelete);
        UIAnimation.Animation_UIShake(UI.functionBookmark);

        UIAnimation.Animation_UIShake(UI.loopLayout);
        UIAnimation.Animation_UIShake(UI.loopDelete);
        UIAnimation.Animation_UIShake(UI.loopBookmark);

        UIAnimation.Animation_UIShake(UI.forwardButton);
        UIAnimation.Animation_UIShake(UI.turnLeftButton);
        UIAnimation.Animation_UIShake(UI.turnRightButton);
        UIAnimation.Animation_UIShake(UI.functionButton);
    }

    public void Initialize_CodingMethod()
    {
        MainMethod.Clear(); // OnSceneLoad 델리게이트 체인을 걸어서 사용하기, 새로운 스테이지 마다 블록 초기화
        Function.Clear();   // 레이아웃 내부에 블록 프리팹도 Destroy 하기
        Loop.Clear();

        foreach (CodingBlock blockObj in MainMethod)
        {
            Destroy(blockObj.gameObject);
        }
        foreach (CodingBlock blockObj in Function)
        {
            Destroy(blockObj.gameObject);
        }
        foreach (CodingBlock blockObj in Loop)
        {
            Destroy(blockObj.gameObject);
        }
    }
    public void Initialize_UIManager(GameObject obj)
    {
        UI = obj.GetComponent<UIManager>();
    }
    public void Initialize_PlayerManager(GameObject obj)
    {
        PlayerManager = obj.GetComponent<PlayerManager>();
    }
}