using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // => 해당 스크립트는 기본 베이스를 프리팹으로 만들고 내부 데이터를 각 스테이지별로 인스펙터에서 관리한다
    [field: SerializeField] public GameObject[] CoinObject { get; private set; }
    [field: SerializeField] public int CoinCount { get; private set; }

    public Vector3 StageClearPanelInitPos { get; private set; }

    [Header("카메라 조작 범위 값")]

    [Header("X 축")]
    [SerializeField] private float _camPanMinValueX;
    [SerializeField] private float _camPanMaxValueX;

    [Header("Y 축")]
    [SerializeField] private float _camPanMinValueY;
    [SerializeField] private float _camPanMaxValueY;

    [Header("Z 축")]
    [SerializeField] private float _camPanMinValueZ;
    [SerializeField] private float _camPanMaxValueZ;

    [Header("카메라 이동 속도")]
    [SerializeField] private float CameraPanSpeed = 0.4f;

    private BlockCodingManager _gameManager;
    private CodingUIManager _codingUIManager;
    private PlayerManager _playerManager;

    private void Start()
    {
        _gameManager = BlockCodingManager.Instance;
        _codingUIManager= CodingUIManager.Instance;

        // .. 게임 매니저에 StageManager 등록
        BlockCodingManager.Instance.Register_StageManager(this.gameObject);

        // .. 스테이지별 코인 갯수로 코인 정보를 갱신
        CoinCount = CoinObject.Length;

        // .. 카메라 팬 예외처리
        if (_camPanMinValueX == 0 || _camPanMaxValueX == 0)
        {
            Debug.Log("카메라 팬 X축의 Min, Max 값이 초기화되어 있지 않음!");
        }

        if (_camPanMinValueY == 0 || _camPanMaxValueY == 0)
        {
            Debug.Log("카메라 팬 Y축의 Min, Max 값이 초기화되어 있지 않음!");
        }

        if (_camPanMinValueZ == 0 || _camPanMaxValueZ == 0)
        {
            Debug.Log("카메라 팬 Z축의 Min, Max 값이 초기화되어 있지 않음!");
        }

        AudioManager.Instance.Play_Music("CityTheme");
        // .. 오디오 매니저 스크립트의 변수를 참조해서 조건문으로 스테이지 배경음을 재생  
        // .. 이미 스테이지 배경음악이 재생중이라면 bool 변수를 통해서 스테이지 씬 일때만 이어서 재생
        // .. 배경음악이 재생 중인 상태에서 다른 씬으로 넘어가면 배경음악 소리가 서서히 작아 졌다가 커지는 효과
        // .. 스테이지 씬에서 레벨 셀렉션으로 넘어가면 다른 배경음악 재생 시작
    }

    public void Update()
    {
        if (_gameManager.IsCompilerRunning || _codingUIManager.IsOptionMenuOpen)
            return;

        CameraPan();
    }


    public void CameraPan() // CameraTarget을 화면 터치로 움직여서 카메라의 각도를 조절
    {
        if (_playerManager == null) 
            _playerManager = BlockCodingManager.PlayerManager_Instance;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // .. Vector2의 화면 터치값을 참조하여 계산함으로 타겟의 움직임도 Vector2 처럼 2개의 좌표로만 움직여야 한다.
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector3 newPosition = _playerManager.CameraTargetObject.transform.position + new Vector3(touchDeltaPosition.x, -touchDeltaPosition.y, -touchDeltaPosition.x) * CameraPanSpeed * Time.deltaTime;

            newPosition.x = Mathf.Clamp(newPosition.x, _playerManager.CamTargetStartWorldPosition.x + _camPanMinValueX,
                _playerManager.CamTargetStartWorldPosition.x + _camPanMaxValueX);

            newPosition.y = Mathf.Clamp(newPosition.y, _playerManager.CamTargetStartWorldPosition.y + _camPanMinValueY,
                _playerManager.CamTargetStartWorldPosition.y + _camPanMaxValueY);

            newPosition.z = Mathf.Clamp(newPosition.z, _playerManager.CamTargetStartWorldPosition.z + _camPanMinValueZ,
                _playerManager.CamTargetStartWorldPosition.z + _camPanMaxValueZ);

            _playerManager.CameraTargetObject.transform.position = newPosition;
        }
    }

    public void UpdateCoin() // Coin.cs에서 코인이 플레이어 콜라이더에 닿아 코인 오브젝트가 비활성화 될 때 호출
    {
        if (CoinCount != 0)
            CoinCount--;

        if (CoinCount == 0)
            StageClear();
    }

    public void ResetCoin()
    {
        if (CoinCount == CoinObject.Length)
            return;

        CoinCount = CoinObject.Length;

        foreach (GameObject coin in CoinObject)
        {
            coin.gameObject.SetActive(true);
        }
    }

    public void StageClear()
    {
        CodingUIManager.Instance.StopButton.interactable = false;

        CodingUIManager.Instance.OptionMenuOpenButton.interactable = true;

        BlockCodingManager.Instance.IsStageClear = true;

        UnlockNewLevel();

        Time.timeScale = 1;

        CodingUIManager.Instance.ClearPanel.transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutExpo);
        CodingUIManager.Instance.ActiveStageClearUI();

    }

    public void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
