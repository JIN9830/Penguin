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

    public bool isMoving { get; private set; } = false;

    private void OnEnable()
    {
        deltaTimeCount = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.playBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            deltaTimeCount = 0;
            isMoving = false;
            blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else PlayerMovement();
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(GameManager.Instance.playerObject.transform.localPosition, GameManager.Instance.playerObject.transform.forward, out hit, DISTANCE))
        {
            deltaTimeCount = 0;
            isMoving = false;
            blockTweener = GameManager.Instance.UI.BlockShakeAnimation(this.gameObject);

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
        if (!isMoving) {
            GameManager.Instance.playerAnimator.SetTrigger("Forward");
            blockTweener = GameManager.Instance.UI.ForwardBlock_PlayAnimation(this.gameObject);
            isMoving = true;
        }

        if (isMoving == true)
        {
            deltaTimeCount += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(GameManager.Instance.playerStartPos, GameManager.Instance.playerNewPos, 1.5f * deltaTimeCount);
            GameManager.Instance.playerObject.transform.localPosition = newPos;

            if (deltaTimeCount > 1)
            {
                deltaTimeCount = 0;
                isMoving = false;
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }


}
