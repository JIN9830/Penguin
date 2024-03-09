using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    [Header("플레이어 정보")]
    [SerializeField]
    public GameObject playerObject;
    public Animator PlayerAnimator { get; private set; }
    public Vector3 PlayerStartPos { get; private set; }
    public Vector3 PlayerNewPos { get; private set; }

    public Vector3 PlayerRestPos { get; private set; }
    public Quaternion PlayerRestRot { get; private set; }

    private void Awake()
    {
        PlayerAnimator = playerObject.GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.Initialize_PlayerManager(this.gameObject);

        PlayerRestPos = playerObject.transform.position; // 플레이어 위치 초기화 코드 상황에 맞게 초기화 하는 함수로 이동
        PlayerRestRot = playerObject.transform.rotation;
    }

    public void InitializePlayerMoveVector()
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
