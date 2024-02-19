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
        if (GameManager.Instance.playBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            deltaTimeCount = 0;
            IsMoving = false;

            GameManager.Instance.playerAnimator.SetBool("Forward", false);

            blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else PlayerMovement();

        if (deltaTimeCount > 0.7f) {
            GameManager.Instance.playerAnimator.SetBool("Forward", false);

        }else
            GameManager.Instance.playerAnimator.SetBool("Forward", IsMoving);
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(GameManager.Instance.playerObject.transform.localPosition, GameManager.Instance.playerObject.transform.forward, out hit, DISTANCE))
        {
            deltaTimeCount = 0;
            IsMoving = false;
            blockTweener = GameManager.Instance.UI.BlockShakeAnimation(this.gameObject);

            GameManager.Instance.ShakeUI();

            if (hit.collider.CompareTag("Wall"))
            {
                GameManager.Instance.playerAnimator.SetTrigger("WallHit");
                this.GetComponent<CodingBlock>().enabled = false;
            }
            else if (hit.collider.CompareTag("Edge"))
            {
                GameManager.Instance.playerAnimator.SetTrigger("Edge");
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }

    private void PlayerMovement()
    {
        if (!IsMoving) {
            IsMoving = true;
            blockTweener = GameManager.Instance.UI.ForwardBlock_PlayAnimation(this.gameObject);          
        }

        if (IsMoving == true)
        {
            deltaTimeCount += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(GameManager.Instance.playerStartPos, GameManager.Instance.playerNewPos, 1.5f * deltaTimeCount);
            GameManager.Instance.playerObject.transform.localPosition = newPos;



            if (deltaTimeCount > 1)
            {
                deltaTimeCount = 0;
                IsMoving = false;
                GameManager.Instance.playerAnimator.SetBool("Forward", false);
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }


}
