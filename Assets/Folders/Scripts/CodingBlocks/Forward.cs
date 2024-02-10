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

    private void Start()
    {
        deltaTimeCount = 0;
    }

    private void Update()
    {
        if (GM.playBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            deltaTimeCount = 0;
            isMoving = false;
            this.GetComponent<CodingBlock>().enabled = false;
        }
        else PlayerMovement();
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(GM.playerObject.transform.localPosition, GM.playerObject.transform.forward, out hit, DISTANCE))
        {
            deltaTimeCount = 0;
            isMoving = false;

            if (hit.collider.CompareTag("Wall"))
            {
                GM.playerAnimator.SetTrigger("WallHit");
                this.GetComponent<CodingBlock>().enabled = false;
            }
            else if (hit.collider.CompareTag("Edge"))
            {
                GM.playerAnimator.SetTrigger("Edge");
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }

    private void PlayerMovement()
    {
        if (!isMoving) {
            GM.playerAnimator.SetTrigger("Forward");
            isMoving = true;
        }

        if (isMoving == true)
        {
            deltaTimeCount += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(GM.playerStartPos, GM.playerNewPos, 1.5f * deltaTimeCount);
            GM.playerObject.transform.localPosition = newPos;

            if (deltaTimeCount > 1)
            {
                deltaTimeCount = 0;
                isMoving = false;
                this.GetComponent<CodingBlock>().enabled = false;
            }
        }
    }
}
