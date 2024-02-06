using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Forward : CodingBlock
{
    private float deltaTimeCount = 0;
    private readonly float DISTANCE = 0.6f;
    private RaycastHit hit;
    public bool test1;

    private void Start()
    {
        deltaTimeCount = 0;
    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        GM.isMoving = true;
    }

    private void Update()
    {
        if(GM.playBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            deltaTimeCount = 0;
            GM.isMoving = false;
            GM.playerAnimator.SetFloat("Walk", deltaTimeCount);
            this.GetComponent<CodingBlock>().enabled = false;
            return;
        }

        if (Physics.Raycast(GM.playerObject.transform.localPosition, GM.playerObject.transform.forward, out hit, DISTANCE))
        {
            return;
        }
        else
        {
            if (GM.isMoving == true)
            {
                GM.playerAnimator.SetFloat("Walk", deltaTimeCount);

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
}
