using DG.Tweening;
using UnityEngine;
using static GameManager;

public class Forward : CodingBlock
{
    public bool IsForwarding { get; private set; } = false;
    private float fixedDeltaTimeCount = 0;
    private readonly float PLAYER_MOVESPEED = 1.5f;

    private RaycastHit hit;
    private readonly float DISTANCE = 0.6f;

    private void OnEnable()
    {
        fixedDeltaTimeCount = 0;
    }

    private void Update()
    {
        if (GameManager_Instance.PlayToggle == false) // 정지 버튼을 누르면 실행
        {
            IsForwarding = false;
            PlayerManager_Instance.PlayerAnimator.SetBool("Forward", false);
            blockTweener.Kill();
            transform.localScale = Vector3.one;
            fixedDeltaTimeCount = 0;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if(IsForwarding == true)
        {
            PlayerMove();

            if (fixedDeltaTimeCount > 0.65f)
            {
                PlayerManager_Instance.PlayerAnimator.SetBool("Forward", false);
            }
            else
                PlayerManager_Instance.PlayerAnimator.SetBool("Forward", IsForwarding);
        }
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(PlayerManager_Instance.playerObject.transform.localPosition, PlayerManager_Instance.playerObject.transform.forward, out hit, DISTANCE))
        {
            IsForwarding = false;
            blockTweener = UIManager_Instance.UIAnimation.Animation_BlockShake(this.gameObject);

            if (hit.collider.CompareTag("Wall"))
            {
                PlayerManager_Instance.PlayerAnimator.SetTrigger("WallHit");
                UIManager_Instance.Shake_UIElements();
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

    private void PlayerMove()
    {
        if (IsForwarding == true)
        {
            fixedDeltaTimeCount += Time.fixedDeltaTime;
            Vector3 newPos = Vector3.Lerp(PlayerManager_Instance.PlayerStartPos, PlayerManager_Instance.PlayerNewPos, PLAYER_MOVESPEED * fixedDeltaTimeCount);
            PlayerManager_Instance.playerObject.transform.localPosition = newPos;

            if (fixedDeltaTimeCount > 1)
            {
                IsForwarding = false;
                PlayerManager_Instance.PlayerAnimator.SetBool("Forward", false);
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }


}
