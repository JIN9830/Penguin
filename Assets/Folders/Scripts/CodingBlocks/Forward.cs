using DG.Tweening;
using UnityEngine;
using static GameManager;

public class Forward : CodingBlock
{
    private readonly float _PLAYER_MOVESPEED = 1.5f;
    private readonly float _DISTANCE = 0.6f;

    private RaycastHit _hit;
    private float _deltaTimeCount = 0;

    private void OnEnable()
    {
        _deltaTimeCount = 0;
    }

    private void OnDisable() 
    {
        // .. 블록 실행이 종료될 때 초기화해야 할 작업들
        transform.localScale = Vector3.one;

        PlayerManager_Instance.playerState = PlayerManager.PlayerState.None;

        // .. 앞으로 전진하는 애니메이션의 싱크를 맞추기위한 설정
        _deltaTimeCount = 0.67f;
        PlayerManager_Instance.PlayerAnimator.SetFloat("Forward", _deltaTimeCount);
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        // .. 플레이어 앞에 장애물이 있다면 알맞는 애니메이션을 재생하고 블록 스크립트를 비활성화 하여 블록 실행을 종료합니다.
        if (Physics.Raycast(PlayerManager_Instance.playerObject.transform.localPosition, PlayerManager_Instance.playerObject.transform.forward, out _hit, _DISTANCE))
        {
            BlockTweener = CodingUIManager_Instance.UIAnimation.Animation_BlockShake(this.gameObject);

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
        // ... 장애물이 없다면 Update에서 앞으로 전진하는 PlayerMove를 호출하도록 PlayerState를 Forwarding로 변경
        else
        {
            BlockTweener = CodingUIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
            PlayerManager_Instance.playerState = PlayerManager.PlayerState.Forwarding;
            //AudioManager.Instance.PlayerSFX("Walking");
        }
    }

    private void PlayerMove()
    {
        PlayerManager_Instance.PlayerAnimator.SetFloat("Forward", _deltaTimeCount);

        _deltaTimeCount += Time.deltaTime;
        Vector3 newPos = Vector3.Lerp(PlayerManager_Instance.PlayerStartPos, PlayerManager_Instance.PlayerNewPos, _deltaTimeCount * _PLAYER_MOVESPEED);
        PlayerManager_Instance.playerObject.transform.localPosition = newPos;

        if (_deltaTimeCount > 1)
        {
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }

    private void Update()
    {
        // .. 플레이어가 정지 버튼을 누르면 블록의 애니메이션을 종료하고 블록 스크립트를 비활성화
        if (GameManager_Instance.IsCompilerRunning == false)
        {
            BlockTweener.Kill();
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else if (PlayerManager_Instance.playerState == PlayerManager.PlayerState.Forwarding)
        {
            PlayerMove();
        }
    }


}
