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

    [Header("플레이어 정보")]
    [SerializeField]
    public GameObject playerObject;

    public GameObject cameraTargetObject; // TODO: 테스트용 코드 (플레이어가 카메라를 조작할때 움직이는 오브젝트)
    public Vector3 cameraTargetObjectInitPos;
    public Animator PlayerAnimator { get; private set; }
    public Vector3 PlayerStartPos { get; private set; }
    public Vector3 PlayerNewPos { get; private set; }
    public Vector3 PlayerResetPos { get; private set; }
    public Quaternion PlayerResetRot { get; private set; }

    public Tweener PlayerMoveTween { get; private set; }
    public Tweener PlayerRotateTween { get; private set; }

    private void Start()
    {
        GameManager_Instance.Register_PlayerManager(this.gameObject);

        PlayerAnimator = playerObject.GetComponent<Animator>();

        PlayerResetPos = playerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        PlayerResetRot = playerObject.transform.rotation;

        cameraTargetObjectInitPos = cameraTargetObject.transform.localPosition;
        cameraTargetObject.transform.localPosition = new Vector3(0, 2.5f, 0);
        cameraTargetObject.transform.DOLocalMoveY(0, 0.8f);
    }

    public void InitPlayerMoveVector()
    {
        PlayerStartPos = playerObject.transform.localPosition;
        PlayerNewPos = PlayerStartPos + playerObject.transform.forward;
    }

    public void ResetPlayerPosition()
    {
        PlayerAnimator.SetTrigger("Reset");
        playerObject.transform.DOMove(PlayerResetPos, 0.5f).OnComplete(() => StageManager_Instance.ResetCoin());
        playerObject.transform.DORotateQuaternion(PlayerResetRot, 1f);
    }
}
