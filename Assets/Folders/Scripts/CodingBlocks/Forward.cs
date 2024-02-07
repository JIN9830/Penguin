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
    }

    private void Update()
    {
        if(Gm.playBlockToggle == false) // 정지 버튼을 누르면 실행
        {
            deltaTimeCount = 0;
            Gm.isMoving = false;
            this.GetComponent<CodingBlock>().enabled = false;
            return;
        }

        if (Physics.Raycast(Gm.playerObject.transform.localPosition, Gm.playerObject.transform.forward, out hit, DISTANCE))
        {
            deltaTimeCount = 0;
            Gm.isMoving = false;
            this.GetComponent<CodingBlock>().enabled = false;
            return;
        }
        else
        {
            if(!Gm.isMoving)
            {
                Gm.playerAnimator.SetTrigger("Forward");
                Gm.isMoving = true;
            }

            if (Gm.isMoving == true)
            {

                deltaTimeCount += Time.deltaTime;
                Vector3 newPos = Vector3.Lerp(Gm.playerStartPos, Gm.playerNewPos, 1.5f * deltaTimeCount);
                Gm.playerObject.transform.localPosition = newPos;

                if (deltaTimeCount >= 1)
                {
                    deltaTimeCount = 0;
                    Gm.isMoving = false;
                    this.GetComponent<CodingBlock>().enabled = false;
                }
            }
        }
    }
}
