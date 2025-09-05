using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        None,
        Forwarding,
        TurnLeft,
        TurnRight,
    }
    public PlayerState EPlayerState = PlayerState.None;

    [field: Header("플레이어 오브젝트")]
    [field: SerializeField] public GameObject PlayerObject { get; private set; }

    [field: SerializeField] public GameObject CameraTargetObject { get; private set; }
    public Vector3 CamTargetStartPosition { get; private set; }
    public Vector3 CamTargetStartWorldPosition { get; private set; }


    public Animator PlayerAnimator { get; private set; }
    public Vector3 PlayerStartPos { get; private set; }
    public Vector3 PlayerNewPos { get; private set; }
    public Vector3 PlayerResetPos { get; private set; }
    public Quaternion PlayerResetRot { get; private set; }

    [field: SerializeField] public ParticleSystem FootStepDust { get; private set; }
    [field: SerializeField] public ParticleSystem LandingDust { get; private set; }


    private void Awake()
    {
        PlayerAnimator = PlayerObject.GetComponent<Animator>();

        PlayerResetPos = PlayerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        PlayerResetRot = PlayerObject.transform.rotation;

        CamTargetStartPosition = CameraTargetObject.transform.localPosition;
        CamTargetStartWorldPosition = CameraTargetObject.transform.position;
    }

    private void Start()
    {
        // .. 게임 매니저에 PlayerManager 등록
        BlockCodingManager.Instance.Register_PlayerManager(this.gameObject);

        // 코딩시티 씬이 시작될때 카메라 무빙을 시작 (테스트용 코드)
        CameraTargetObject.transform.localPosition = new Vector3(0, 2.5f, 0);
        CameraTargetObject.transform.DOLocalMoveY(0, 0.8f).SetDelay(0.2f).OnComplete(()=> CodingUIManager.Instance.CityNameObj.text = SceneManager.GetActiveScene().name);
    }

    public void Initialize_PlayerForwardVector() // .. 현재 플레이어의 포지션 값을 이용하여 앞으로 전진 좌표값을 갱신
    {
        PlayerStartPos = PlayerObject.transform.localPosition;
        PlayerNewPos = PlayerStartPos + PlayerObject.transform.forward;
    }

    public void ResetPlayerPosition()
    {
        PlayerAnimator.SetTrigger("Reset");
        PlayerObject.transform.DOMove(PlayerResetPos, 0.5f).OnComplete(() => BlockCodingManager.StageManager_Instance.ResetCoin());
        PlayerObject.transform.DORotateQuaternion(PlayerResetRot, 0.5f);
    }
}
