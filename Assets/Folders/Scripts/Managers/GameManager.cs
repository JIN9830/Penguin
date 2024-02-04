using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum CurrentLayout
    {
        Main,
        Function,
    }
    public enum ProcessingStatus
    {
        Stop,
        Playing,
    }
    public static GameManager Instance { get; private set; }

    public List<CodingBlock> MainMethod { get; private set; } = new List<CodingBlock>();
    public List<CodingBlock> Function = new List<CodingBlock>();
    public List<CodingBlock> Loop = new List<CodingBlock>();

    [Header("현재 플레이어의 상태")]
    [SerializeField]
    public GameObject playerObject;
    public CurrentLayout currentLayout = CurrentLayout.Main;
    public ProcessingStatus currentProcessing = ProcessingStatus.Stop;

    [Header("캔버스 오브젝트")]
    public GameObject Canvas;

    [Header("그리드 레이아웃 오브젝트")]
    public GameObject mainLayout;
    public GameObject functionLayout;
    private Button mainLayoutButton;
    private Button functionLayoutButton;

    [Header("블럭 삭제 버튼")]
    public Button mainDelete;
    public Button functionDelete;

    [Header("플레이 & 정지, 스피드업 버튼")]
    public Button playButton;
    public Sprite stopImage;
    public Button speedUpButton;

    [Header("코딩블럭 버튼 오브젝트")]
    public Button forwardButton;
    public Button turnLeftButton;
    public Button turnRightButton;
    public Button functionButton;


    [Header("코딩블럭 프리팹")]
    public GameObject forwardPrefab;
    public GameObject turnLeftPrefab;
    public GameObject turnRightPrefab;
    public GameObject functionPrefab;

    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("싱글톤 게임매니저 생성!");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("중복된 싱글톤 게임매니저 삭제");
        }
        #endregion

        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        MainMethod.Clear();
        Function.Clear();

        // TODO: 추후에 Awake 메서드로 가야할지 정하기
        #region Codingblocks onClickAddListener
        // : each buttons link to each Prefab
        forwardButton.onClick.AddListener(() => InsertBlock(forwardPrefab));
        turnLeftButton.onClick.AddListener(() => InsertBlock(turnLeftPrefab));
        turnRightButton.onClick.AddListener(() => InsertBlock(turnRightPrefab));
        functionButton.onClick.AddListener(() => InsertBlock(functionPrefab));
        #endregion

        #region Layout activate onClickAddListener
        mainLayoutButton = mainLayout.GetComponent<Button>();
        functionLayoutButton = functionLayout.GetComponent<Button>();

        mainLayoutButton.onClick.AddListener(() => currentLayout = CurrentLayout.Main);
        functionLayoutButton.onClick.AddListener(() => currentLayout = CurrentLayout.Function);
        #endregion

        #region block delete OnClickAddListener
        mainDelete.onClick.AddListener(() => { currentLayout = CurrentLayout.Main; DeleteBlock(currentLayout); });
        functionDelete.onClick.AddListener(() => { currentLayout = CurrentLayout.Function; DeleteBlock(currentLayout); });
        #endregion

        playButton.onClick.AddListener(() => StartCoroutine(PlayBlock()));
        //stopButton.onClick.AddListener(() => { });
        //speedUpButton.onClick.AddListener(() => { });

    }

    public void InsertBlock(GameObject prefab)
    {
        if (prefab == functionPrefab)
        {
            if (MainMethod.Count < 10)
            {
                MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
            }
        }
        else
        {
            switch (currentLayout)
            {
                case CurrentLayout.Main:
                    if (MainMethod.Count < 10) MainMethod.Add(Instantiate(prefab, mainLayout.transform).GetComponent<CodingBlock>());
                    break;

                case CurrentLayout.Function:
                    if (Function.Count < 10) Function.Add(Instantiate(prefab, functionLayout.transform).GetComponent<CodingBlock>());
                    break;
            }
        }
    }

    public void DeleteBlock(CurrentLayout currentLayout)
    {
        switch (currentLayout)
        {
            case CurrentLayout.Main:
                if (MainMethod.Count > 0)
                {
                    CodingBlock lastblock = MainMethod.Last();
                    MainMethod.Remove(lastblock);
                    Destroy(lastblock.gameObject);
                }
                break;

            case CurrentLayout.Function:
                if (Function.Count > 0)
                {
                    CodingBlock lastblock = Function.Last();
                    Function.Remove(lastblock);
                    Destroy(lastblock.gameObject);
                }
                break;
        }
    }

    public IEnumerator PlayBlock()
    {
        if (MainMethod != null)
        {
            // TODO: 버튼 비활성화 코드 넣기
            foreach (CodingBlock block in MainMethod)
            {
                yield return StartCoroutine(block.MoveOrder());
            }
        }
        BlockHighLightOff();
    }

    public void BlockHighLightOff()
    {
        foreach (CodingBlock block in MainMethod)
        {
            block.ToggleHighLight(false);
        }
        foreach (CodingBlock block in Function)
        {
            block.ToggleHighLight(false);
        }
    }

    public void ButtonsOnOff(bool enable)
    {

    }
}
