using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [field: SerializeField] public GameObject[] CoinGameObjects { get; private set; }
    private int _collectedCoinCount;
    [SerializeField] private GameObject _coinCounterUIObject;
    private TextMeshProUGUI _coinCountText;

    public Vector3 StageClearPanelInitPos { get; private set; }

    [Header("카메라 조작 범위 값 (Offset)")]
    [SerializeField] private Vector3 _minCamPanOffset;
    [SerializeField] private Vector3 _maxCamPanOffset;

    [Header("카메라 이동 속도")]
    [SerializeField] private float _cameraPanSpeed = 0.35f;
    
    private const float COIN_RESET_Y_POSITION = 0.2f;

    private BlockCodingManager _blockCodingManager => GameManager.Instance.BlockCodingManager;
    private CodingUIManager _codingUIManager => GameManager.Instance.CodingUIManager;
    private PlayerManager _playerManager => GameManager.Instance.PlayerManager;


    private void Awake()
    {
        // 게임 매니저에 StageManager를 등록합니다.
        GameManager.Instance.Register_StageManager(this.gameObject);
    }

    private void Start()
    {
        InitializeStage();

        AudioManager.Instance.Play_Music("CityTheme");
    }

    private void Update()
    {
        if (_blockCodingManager.IsCompilerRunning || _codingUIManager.IsOptionMenuOpen)
            return;

        HandleCameraPan();
    }

    private void InitializeStage()
    {
        // .. 스테이지별 코인 갯수로 코인 정보를 갱신
        _collectedCoinCount = 0;

        // .. 카메라 팬 값 예외 처리
        if (_minCamPanOffset == Vector3.zero || _maxCamPanOffset == Vector3.zero)
        {
            Debug.LogWarning("카메라 팬 Min/Max Offset 값이 설정되지 않았습니다!");
        }

        // .. 코인 카운터 UI 초기화
        if (_coinCounterUIObject == null)
        {
            _coinCounterUIObject = _codingUIManager.CoinCounter;
        }
        var coinTextObject = _coinCounterUIObject.transform.GetChild(1).gameObject;
        _coinCountText = coinTextObject.GetComponent<TextMeshProUGUI>();
        _coinCountText.text = $"{_collectedCoinCount} / {CoinGameObjects.Length}";
    }

    // CameraTarget 오브젝트를 움직여서 카메라의 각도를 조절합니다.
    private void HandleCameraPan()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // 터치 x입력으로 카메라의 x, z축을 함께 움직여 입체적인 이동 효과를 줍니다.
            Vector3 moveDirection = new Vector3(touchDeltaPosition.x, -touchDeltaPosition.y, -touchDeltaPosition.x);
            Vector3 newPosition = _playerManager.CameraTargetObject.transform.position + moveDirection * _cameraPanSpeed * Time.deltaTime;

            // 카메라 이동 범위 제한
            Vector3 startPos = _playerManager.CamTargetStartWorldPosition;
            Vector3 minBounds = startPos + _minCamPanOffset;
            Vector3 maxBounds = startPos + _maxCamPanOffset;

            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
            newPosition.z = Mathf.Clamp(newPosition.z, minBounds.z, maxBounds.z);

            _playerManager.CameraTargetObject.transform.position = newPosition;
        }
    }

    // Coin.cs에서 코인이 플레이어와 충돌했을 때 호출됩니다.
    public void UpdateCoin() 
    {
        _collectedCoinCount++;
        _coinCountText.text = $"{_collectedCoinCount} / {CoinGameObjects.Length}";

        if (_collectedCoinCount >= CoinGameObjects.Length)
        {
            StageClear();
        }
    }

    public void ResetCoin()
    {
        if (_collectedCoinCount == 0)
            return;

        _collectedCoinCount = 0;
        _coinCountText.text = $"{_collectedCoinCount} / {CoinGameObjects.Length}";

        foreach (GameObject coin in CoinGameObjects)
        {
            Vector3 coinPos = coin.transform.localPosition;
            coinPos.y = COIN_RESET_Y_POSITION;
            coin.transform.localPosition = coinPos;
            coin.transform.DOScale(1.2f, 1);
        }
    }

    public void StageClear()
    {
        _codingUIManager.StopButton.interactable = false;
        _codingUIManager.OptionMenuOpenButton.interactable = true;
        _blockCodingManager.IsStageClear = true;

        UnlockNewLevel();

        Time.timeScale = 1;

        _codingUIManager.ClearPanel.transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutExpo);
        _codingUIManager.ActiveStageClearUI();
    }

    public void UnlockNewLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", currentSceneIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}