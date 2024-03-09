using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Forward : CodingBlock
{
    private float deltaTimeCount = 0;
    private readonly float DISTANCE = 0.6f;
    private RaycastHit hit;
    public bool IsMoving { get; private set; } = false;

    private void OnEnable()
    {
        deltaTimeCount = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.PlayBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            IsMoving = false;
            GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", false);
            blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else PlayerMovement();

        if (deltaTimeCount > 0.65f) {
            GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", false);
        }
        else
            GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", IsMoving);
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(GameManager.Instance.PlayerManager.playerObject.transform.localPosition, GameManager.Instance.PlayerManager.playerObject.transform.forward, out hit, DISTANCE))
        {
            IsMoving = false;
            blockTweener = GameManager.Instance.UIAnimation.Animation_BlockShake(this.gameObject);

            if (hit.collider.CompareTag("Wall"))
            {
                GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("WallHit");
                GameManager.Instance.Shake_UIElements();
                this.GetComponent<CodingBlock>().enabled = false;
            }
            else if (hit.collider.CompareTag("Edge"))
            {
                GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("Edge");
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }

    private void PlayerMovement()
    {
        if (!IsMoving) {
            IsMoving = true;
            blockTweener = GameManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);          
        }

        if (IsMoving == true)
        {
            deltaTimeCount += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(GameManager.Instance.PlayerManager.PlayerStartPos, GameManager.Instance.PlayerManager.PlayerNewPos, 1.5f * deltaTimeCount);
            GameManager.Instance.PlayerManager.playerObject.transform.localPosition = newPos;

            if (deltaTimeCount > 1)
            {
                IsMoving = false;
                GameManager.Instance.PlayerManager.PlayerAnimator.SetBool("Forward", false);
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }


}
