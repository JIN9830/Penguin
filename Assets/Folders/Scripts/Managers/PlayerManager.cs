using UnityEngine;
using DG.Tweening;
using static GameManager;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerActionState
    {
        None,
        Forward,
        Left,
        Right,
    }

    public PlayerActionState playerActionState { get; private set; } = PlayerActionState.None;

    [Header("플레이어 정보")]
    [SerializeField]
    public GameObject playerObject;

    public GameObject CameraTargetObject;
    public Animator PlayerAnimator { get; private set; }
    public Vector3 PlayerStartPos { get; private set; }
    public Vector3 PlayerNewPos { get; private set; }
    public Vector3 PlayerRestPos { get; private set; }
    public Quaternion PlayerRestRot { get; private set; }

    private void Start()
    {
        GameManager_Instance.Get_PlayerManager(this.gameObject);

        PlayerAnimator = playerObject.GetComponent<Animator>();

        PlayerRestPos = playerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        PlayerRestRot = playerObject.transform.rotation;

        CameraTargetObject.transform.localPosition = new Vector3(0, 2.5f, 0);
        CameraTargetObject.transform.DOLocalMoveY(0, 0.8f);
    }

    public void InitPlayerMoveVector()
    {
        PlayerStartPos = playerObject.transform.localPosition;
        PlayerNewPos = PlayerStartPos + playerObject.transform.forward;
    }

    public void ResetPlayerPosition()
    {
        playerObject.transform.DOMove(PlayerRestPos, 0.3f);
        playerObject.transform.DORotateQuaternion(PlayerRestRot, 1f);
        PlayerAnimator.SetTrigger("Reset");
    }
}
