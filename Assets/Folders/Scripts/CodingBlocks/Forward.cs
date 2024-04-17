using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using static GameManager;

public class Forward : CodingBlock
{
    private float _deltaTimeCount = 0;
    private readonly float _PLAYER_MOVESPEED = 1.5f;

    private RaycastHit _hit;
    private readonly float _DISTANCE = 0.6f;

    private void OnEnable()
    {
        _deltaTimeCount = 0;
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
        PlayerManager_Instance.playerState = PlayerManager.PlayerState.None;
        _deltaTimeCount = 0.67f;
        PlayerManager_Instance.PlayerAnimator.SetFloat("Forward", _deltaTimeCount);
    }

    private void Update()
    {
        if (GameManager_Instance.ExecutionToggle == false) // 정지 버튼을 누르면 실행
        {
            _blockTweener.Kill();
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else PlayerMove();
    }

    private void PlayerMove()
    {
        PlayerManager_Instance.PlayerAnimator.SetFloat("Forward", _deltaTimeCount);

        if (PlayerManager_Instance.playerState == PlayerManager.PlayerState.Forwarding)
        {
            _deltaTimeCount += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(PlayerManager_Instance.PlayerStartPos, PlayerManager_Instance.PlayerNewPos, _PLAYER_MOVESPEED * _deltaTimeCount);
            PlayerManager_Instance.playerObject.transform.localPosition = newPos;

            if (_deltaTimeCount > 1)
            {
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(PlayerManager_Instance.playerObject.transform.localPosition, PlayerManager_Instance.playerObject.transform.forward, out _hit, _DISTANCE))
        {
            _blockTweener = CodingUIManager_Instance.UIAnimation.Animation_BlockShake(this.gameObject);

            if (_hit.collider.CompareTag("Wall"))
            {
                PlayerManager_Instance.PlayerAnimator.SetTrigger("HitTheWall");
                CodingUIManager_Instance.ShakeUIElements();
                this.GetComponent<CodingBlock>().enabled = false;
            }
            else if (_hit.collider.CompareTag("Edge"))
            {
                PlayerManager_Instance.PlayerAnimator.SetTrigger("ReachTheEdge");
                this.GetComponent<CodingBlock>().enabled = false;
            }

        }
        else
        {
            _blockTweener = CodingUIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
            PlayerManager_Instance.playerState = PlayerManager.PlayerState.Forwarding;
        }
    }
}
