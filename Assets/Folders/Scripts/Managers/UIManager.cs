using UnityEngine;
using UnityEngine.UI;

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
        #region Coding blocks onClickAddListener
        forwardButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.InsertBlock(GameManager.Instance.forwardPrefab));
        turnLeftButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.InsertBlock(GameManager.Instance.turnLeftPrefab));
        turnRightButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.InsertBlock(GameManager.Instance.turnRightPrefab));
        functionButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.InsertBlock(GameManager.Instance.functionPrefab));
        loopButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.InsertBlock(GameManager.Instance.loopPrefab));
        #endregion

        #region Layout activate onClickAddListener
        mainLayout.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SelectedMethods(CurrentLayout.Main));
        functionLayout.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SelectedMethods(CurrentLayout.Function));
        loopLayout.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SelectedMethods(CurrentLayout.Loop));

        mainBookmark.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SelectedMethods(CurrentLayout.Main));
        functionBookmark.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SelectedMethods(CurrentLayout.Loop));
        loopBookmark.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.SelectedMethods(CurrentLayout.Function));
        #endregion

        #region block delete OnClickAddListener
        mainDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Main; GameManager.Instance.DeleteBlock(currentLayout); });
        functionDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Function; GameManager.Instance.DeleteBlock(currentLayout); });
        loopDelete.GetComponent<Button>().onClick.AddListener(() => { currentLayout = CurrentLayout.Loop; GameManager.Instance.DeleteBlock(currentLayout); });
        #endregion

        #region Play, Stop & TimeControl OnClickAddListener
        playButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.PlayBlock());
        stopButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.StopBlock());
        speedUpButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.ToggleTimeScale());
        #endregion
    }
}
