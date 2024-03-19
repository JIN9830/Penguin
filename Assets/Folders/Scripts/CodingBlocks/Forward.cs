using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Forward : CodingBlock
{
    public bool IsMoving { get; private set; } = false;
    public bool ToggleForward { get; private set; } = false;

    private float fixedDeltaTimeCount = 0;

    private readonly float DISTANCE = 0.6f;
    private RaycastHit hit;

    private void OnEnable()
    {
        fixedDeltaTimeCount = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.PlayToggle == false) // 정지 버튼을 누르면 실행
        {
            IsMoving = false;
            GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", false);
            blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if(IsMoving == true)
        {
            PlayerMove();
        }
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(GameManager.Instance.PlayerManager.playerObject.transform.localPosition, GameManager.Instance.PlayerManager.playerObject.transform.forward, out hit, DISTANCE))
        {
            IsMoving = false;
            blockTweener = GameManager.Instance.UIManager.UIAnimation.Animation_BlockShake(this.gameObject);

            if (hit.collider.CompareTag("Wall"))
            {
                GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("WallHit");
                GameManager.Instance.UIManager.Shake_UIElements();
                this.GetComponent<CodingBlock>().enabled = false;
            }
            else if (hit.collider.CompareTag("Edge"))
            {
                GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("Edge");
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
        else
        {
            blockTweener = GameManager.Instance.UIManager.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
            IsMoving = true;
        }
    }

    private void PlayerMove()
    {
        if (IsMoving == true)
        {
            fixedDeltaTimeCount += Time.fixedDeltaTime;
            Vector3 newPos = Vector3.Lerp(GameManager.Instance.PlayerManager.PlayerStartPos, GameManager.Instance.PlayerManager.PlayerNewPos, 1.5f * fixedDeltaTimeCount);
            GameManager.Instance.PlayerManager.playerObject.transform.localPosition = newPos;

            if (fixedDeltaTimeCount > 0.65f)
            {
                GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", false);
            }
            else
                GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", IsMoving);

            if (fixedDeltaTimeCount > 1)
            {
                IsMoving = false;
                GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", false);
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }


}
