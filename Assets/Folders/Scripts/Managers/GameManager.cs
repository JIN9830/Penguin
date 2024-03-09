using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerManager playerManager;
    public UIManager UI;
    public UIAnimation UIAnimation { get; private set; } = new UIAnimation();

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Function = new List<CodingBlock>();
    public List<CodingBlock> Loop = new List<CodingBlock>();

    public bool playBlockToggle { get; private set; } = false;
    private bool isPlayBlockRunning = false;
    private Coroutine playBlock;
    public readonly WaitForSeconds waitForSeconds = new(1.0f);
    public readonly WaitForSeconds waitForHalfSeconds = new(0.5f);
    public readonly WaitForSeconds waitForPointEightSeconds = new(0.8f);
    public WaitUntil waitUntilPlay;

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

        waitUntilPlay = new WaitUntil(() => playBlockToggle == true);
    }

    private void Start()
    {
        MainMethod.Clear(); // OnSceneLoad 델리게이트 체인을 걸어서 사용하기, 새로운 스테이지 마다 블록 초기화
        Function.Clear();   // 레이아웃 내부에 블록 프리팹도 Destroy 하기
        Loop.Clear();

        playBlock = StartCoroutine(PlayBlock_Coroutine()); // 메인 게임이 시작될때만 동작 메뉴씬 이라면 코루틴 중지
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
        playBlockToggle = true;
    }
    public IEnumerator PlayBlock_Coroutine()
    {
        while (true)
        {
            if (!playBlockToggle) yield return waitUntilPlay;

            if (!isPlayBlockRunning)
            {
                isPlayBlockRunning = true;
                UI.stopButton.gameObject.SetActive(true);
                Lock_UIElements(true);

                foreach (CodingBlock block in MainMethod)
                {
                    if (!isPlayBlockRunning)
                        break;

                    yield return waitForHalfSeconds;

                    playerManager.InitializePlayerMoveVector();
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


                    if (isPlayBlockRunning) yield return waitForPointEightSeconds;
                }

                if (isPlayBlockRunning) yield return waitForSeconds;

                playBlockToggle = false;
                isPlayBlockRunning = false;

                DisableBlockHighlights();
                Lock_UIElements(false);
            }
        }
    }
    public void StopBlock()
    {
        playBlockToggle = false;
        isPlayBlockRunning = false;

        playerManager.ResetPlayerPosition();

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
}