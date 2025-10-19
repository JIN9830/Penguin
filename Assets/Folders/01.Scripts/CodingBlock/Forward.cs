using DG.Tweening;
using UnityEngine;

public class Forward : CodingBlock
{
    private readonly float _PLAYER_MOVESPEED = 1.5f;
    private readonly float _DISTANCE = 0.6f;

    private RaycastHit _hit;
    private float _deltaTimeCount = 0;
    public override void MoveOrder()
    {
        ToggleHighLight(true);

        // 플레이어 앞에 장애물이 있다면 알맞는 애니메이션을 재생하고 블록 스크립트를 비활성화 하여 블록 실행을 종료합니다.
        if (Physics.Raycast(GameManager.Instance.PlayerManager.PlayerObject.transform.localPosition, GameManager.Instance.PlayerManager.PlayerObject.transform.forward, out _hit, _DISTANCE))
        {
            BlockTweener = CodingUIManager.Instance.UIAnimation.Animation_BlockShake(this.gameObject);

            if (_hit.collider.CompareTag("Wall"))
            {
                AudioManager.Instance.Play_PlayerSFX("HitTheWall");
                GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("HitTheWall");
                CodingUIManager.Instance.ShakeUIElements();
            }
            else if (_hit.collider.CompareTag("Edge"))
            {
                AudioManager.Instance.Play_PlayerSFX("ReachTheEdge");
                GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("ReachTheEdge");
            }

            this.enabled = false;
        }
        // 장애물이 없다면 Update에서 앞으로 전진하는 PlayerMove 메서드를 호출 하도록 PlayerState를 Forwarding로 변경
        else
        {
            BlockTweener = CodingUIManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
            GameManager.Instance.PlayerManager.EPlayerState = PlayerManager.PlayerState.Forwarding;
            GameManager.Instance.PlayerManager.FootStepDust.Play();
        }
    }

    private void Update()
    {
        if (BlockCodingManager.IsCompilerRunning == false)
        {
            if (BlockTweener != null) BlockTweener.Kill();
            this.enabled = false;
        }
        else if (PlayerManager.EPlayerState == PlayerManager.PlayerState.Forwarding)
        {
            MovePlayerForward();
        }
    }

    private void OnEnable()
    {
        _deltaTimeCount = 0;

        PlayerManager.Initialize_PlayerForwardVector();
    }

    private void OnDisable() // 전진 블록 실행이 종료될 때 초기화 해야 할 작업들
    {
        transform.localScale = Vector3.one;
        PlayerManager.EPlayerState = PlayerManager.PlayerState.None;

        // 앞으로 전진하는 애니메이션의 발 움직임 싱크를 맞추기 위한 설정
       _deltaTimeCount = 0.70f;
        PlayerManager.PlayerAnimator.SetFloat("Forward", _deltaTimeCount);
    }

    
    private void MovePlayerForward()
    {
        PlayerManager.PlayerAnimator.SetFloat("Forward", _deltaTimeCount);

        _deltaTimeCount += Time.deltaTime;
        Vector3 newPos = Vector3.Lerp(PlayerManager.PlayerStartPos, PlayerManager.PlayerNewPos, _deltaTimeCount * _PLAYER_MOVESPEED);
        PlayerManager.PlayerObject.transform.localPosition = newPos;

        if (_deltaTimeCount > 1)
        {
            this.enabled = false;
        }

    }

}