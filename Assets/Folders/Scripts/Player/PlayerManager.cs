using UnityEngine;
using DG.Tweening;
using static GameManager;
using Unity.VisualScripting;

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
    public Animator PlayerAnimator { get; private set; }
    public Vector3 PlayerStartPos { get; private set; }
    public Vector3 PlayerNewPos { get; private set; }
    public Vector3 PlayerRestPos { get; private set; }
    public Quaternion PlayerRestRot { get; private set; }

    public Tweener PlayerMoveTween { get; private set; }
    public Tweener PlayerRotateTween { get; private set; }

    private void Start()
    {
        GameManager_Instance.Register_PlayerManager(this.gameObject);

        PlayerAnimator = playerObject.GetComponent<Animator>();

        PlayerRestPos = playerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        PlayerRestRot = playerObject.transform.rotation;

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
        playerObject.transform.DOMove(PlayerRestPos, 0.3f).OnComplete(() => StageManager_Instance.ResetCoin());
        playerObject.transform.DORotateQuaternion(PlayerRestRot, 1f);
        PlayerAnimator.SetTrigger("Reset");
    }
}
