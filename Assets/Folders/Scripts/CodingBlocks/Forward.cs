using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class Forward : CodingBlock
{
    private float deltaTimeCount = 0;
    private readonly float DISTANCE = 0.6f;

    private void Update()
    {
        if (Physics.Raycast(GameManager.Instance.playerObject.transform.localPosition, GameManager.Instance.playerObject.transform.forward, out RaycastHit hit, DISTANCE))
        {
            return;
        }
        else
        {
            if (GameManager.Instance.isMoving == true)
            {
                deltaTimeCount += Time.deltaTime;
                Vector3 newPos = Vector3.Lerp(GameManager.Instance.playerStartPos, GameManager.Instance.playerNewPos, 1.5f * deltaTimeCount);
                GameManager.Instance.playerObject.transform.localPosition = newPos;

                if (deltaTimeCount >= 1)
                {
                    deltaTimeCount = 0;
                    GameManager.Instance.isMoving = false;
                    this.GetComponent<CodingBlock>().enabled = false;
                }
            }
        }
    }


    public override void MoveOrder()
    {
        ToggleHighLight(true);
        GameManager.Instance.isMoving = true;
    }

}
