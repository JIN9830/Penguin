using UnityEngine;
using DG.Tweening;
using static GameManager;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        None,
        Forwarding,
        TurnLeft,
        TurnRight,
    }
    public PlayerState playerState = PlayerState.None;

    [field: Header("플레이어 오브젝트")]
    [field: SerializeField] public GameObject PlayerObject { get; private set; }

    [field: SerializeField] public GameObject CameraTargetObject { get; private set; } // TODO: 테스트용 코드 (플레이어가 카메라를 조작할때 움직이는 오브젝트)
    public Vector3 InitCameraTargetPosition { get; private set; }

    public Animator PlayerAnimator { get; private set; }
    public Vector3 PlayerStartPos { get; private set; }
    public Vector3 PlayerNewPos { get; private set; }
    public Vector3 PlayerResetPos { get; private set; }
    public Quaternion PlayerResetRot { get; private set; }


    private void Awake()
    {
        PlayerAnimator = PlayerObject.GetComponent<Animator>();

        PlayerResetPos = PlayerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        PlayerResetRot = PlayerObject.transform.rotation;

        InitCameraTargetPosition = CameraTargetObject.transform.localPosition;
    }

    private void Start()
    {
        // .. 게임 매니저에 PlayerManager 등록
        GameManager_Instance.Register_PlayerManager(this.gameObject);

        // 코딩시티 씬이 시작될때 카메라 무빙을 시직 (테스트용 코드)
        CameraTargetObject.transform.localPosition = new Vector3(0, 2.5f, 0);
        CameraTargetObject.transform.DOLocalMoveY(0, 0.8f);
    }

    public void InitPlayerMoveVector() // .. 현재 플레이어의 포지션, 전진 벡터 값을 갱신
    {
        PlayerStartPos = PlayerObject.transform.localPosition;
        PlayerNewPos = PlayerStartPos + PlayerObject.transform.forward;
    }

    public void ResetPlayerPosition()
    {
        PlayerAnimator.SetTrigger("Reset");
        PlayerObject.transform.DOMove(PlayerResetPos, 0.5f).OnComplete(() => StageManager_Instance.ResetCoin());
        PlayerObject.transform.DORotateQuaternion(PlayerResetRot, 0.5f);
    }
}
