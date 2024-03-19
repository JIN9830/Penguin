using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerManager PlayerManager { get; private set; } = null;
    public UIManager UIManager { get; private set; } = null;

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Function { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Loop { get; private set; } = new List<CodingBlock>();

    public bool PlayToggle = false;
    public bool IsBlocksRunning = false;

    private Coroutine blockCompiler;

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

        waitUntilPlayToggle = new WaitUntil(() => PlayToggle == true);
    }

    private void Start()
    {
        blockCompiler = StartCoroutine(BlockCompiler_Co());
    }

    public IEnumerator BlockCompiler_Co()
    {
        while (true)
        {
            if (!PlayToggle) yield return waitUntilPlayToggle;

            if (!IsBlocksRunning)
            {
                IsBlocksRunning = true;
                UIManager.stopButton.gameObject.SetActive(true);
                UIManager.Lock_UIElements(true);

                foreach (CodingBlock block in MainMethod)
                {
                    if (!IsBlocksRunning)
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


                    if (IsBlocksRunning) yield return waitForPointEightSeconds;
                }

                if (IsBlocksRunning) yield return waitForSeconds;

                PlayToggle = false;
                IsBlocksRunning = false;

                UIManager.DisableBlockHighlights();
                UIManager.Lock_UIElements(false);
            }
        }
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
    public void Get_UIManager(GameObject obj)
    {
        UIManager = obj.GetComponent<UIManager>();
    }
    public void Get_PlayerManager(GameObject obj)
    {
        PlayerManager = obj.GetComponent<PlayerManager>();
    }
}