using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using static GameManager;

public class Forward : CodingBlock
{
    public bool IsForwarding { get; private set; } = false;
    private float deltaTimeCount = 0;
    private readonly float PLAYER_MOVESPEED = 1.5f;

    private RaycastHit hit;
    private readonly float DISTANCE = 0.6f;

    private void OnEnable()
    {
        deltaTimeCount = 0;
    }

    private void OnDisable()
    {
        IsForwarding = false;
        deltaTimeCount = 0.67f;
        PlayerManager_Instance.PlayerAnimator.SetFloat("Forward", deltaTimeCount);
    }

    private void Update()
    {
        if (GameManager_Instance.PlayToggle == false) // 정지 버튼을 누르면 실행
        {
            blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else
            PlayerMove();
    }
    private void PlayerMove()
    {
        PlayerManager_Instance.PlayerAnimator.SetFloat("Forward", deltaTimeCount);

        if (IsForwarding == true)
        {
            deltaTimeCount += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(PlayerManager_Instance.PlayerStartPos, PlayerManager_Instance.PlayerNewPos, PLAYER_MOVESPEED * deltaTimeCount);
            PlayerManager_Instance.playerObject.transform.localPosition = newPos;

            if (deltaTimeCount > 1)
            {
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(PlayerManager_Instance.playerObject.transform.localPosition, PlayerManager_Instance.playerObject.transform.forward, out hit, DISTANCE))
        {
            blockTweener = UIManager_Instance.UIAnimation.Animation_BlockShake(this.gameObject);

            if (hit.collider.CompareTag("Wall"))
            {
                PlayerManager_Instance.PlayerAnimator.SetTrigger("WallHit");
                UIManager_Instance.ShakeUIElements();
                this.GetComponent<CodingBlock>().enabled = false;
            }
            else if (hit.collider.CompareTag("Edge"))
            {
                PlayerManager_Instance.PlayerAnimator.SetTrigger("Edge");
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
        else
        {
            blockTweener = UIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
            IsForwarding = true;
        }
    }




}
