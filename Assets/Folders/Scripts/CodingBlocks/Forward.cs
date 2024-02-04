using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Forward : CodingBlock
{
    private GameObject playerObject;

    private Vector3 playerStartPos;
    private Vector3 playerNewPos;

    public bool isMoving = false;
    public float deltaTimeCount = 0;

    private readonly float DISTANCE = 0.6f;

    private void Start()
    {
        playerObject = gameManager.playerObject;
    }


    public override IEnumerator MoveOrder()
    {
        ToggleHighLight(true);

        if (Physics.Raycast(playerObject.transform.localPosition, playerObject.transform.forward, out RaycastHit hit, DISTANCE))
        {

        }
        else
        {
            isMoving = true;
            playerStartPos = playerObject.transform.localPosition;
            playerNewPos = playerStartPos + playerObject.transform.forward;

            while (isMoving)
            {
                deltaTimeCount += Time.deltaTime;
                Vector3 newPos = Vector3.Lerp(playerStartPos, playerNewPos, 1.5f * deltaTimeCount);
                playerObject.transform.localPosition = newPos;

                if (deltaTimeCount >= 1)
                {
                    deltaTimeCount = 0;
                    isMoving = false;
                }
                yield return null;
            }
        }

    }
}
