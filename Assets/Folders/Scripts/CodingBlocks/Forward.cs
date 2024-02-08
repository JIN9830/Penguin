using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Forward : CodingBlock
{
    private float deltaTimeCount = 0;
    private readonly float DISTANCE = 0.6f;
    private RaycastHit hit;

    private void Start()
    {
        deltaTimeCount = 0;
    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(GM.playerObject.transform.localPosition, GM.playerObject.transform.forward, out hit, DISTANCE) && hit.collider.CompareTag("Wall")) // 스위치로 바꾸기
        {
            deltaTimeCount = 0;
            GM.isMoving = false;
            GM.playerAnimator.SetTrigger("Wall");
            this.GetComponent<CodingBlock>().enabled = false;
        }

        if (Physics.Raycast(GM.playerObject.transform.localPosition, GM.playerObject.transform.forward, out hit, DISTANCE) && hit.collider.CompareTag("Edge"))
        {
            deltaTimeCount = 0;
            GM.isMoving = false;
            GM.playerAnimator.SetTrigger("Edge");
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }

    private void Update()
    {
        if (GM.playBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            deltaTimeCount = 0;
            GM.isMoving = false;
            this.GetComponent<CodingBlock>().enabled = false;
        }


            if (!GM.isMoving)
            {
                GM.playerAnimator.SetTrigger("Forward");
                GM.isMoving = true;
            }

            if (GM.isMoving == true)
            {

                deltaTimeCount += Time.deltaTime;
                Vector3 newPos = Vector3.Lerp(GM.playerStartPos, GM.playerNewPos, 1.5f * deltaTimeCount);
                GM.playerObject.transform.localPosition = newPos;

                if (deltaTimeCount >= 1)
                {
                    deltaTimeCount = 0;
                    GM.isMoving = false;
                    this.GetComponent<CodingBlock>().enabled = false;
                }
            }
        
    }
}
