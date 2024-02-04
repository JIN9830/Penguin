using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forward : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        gameManager.isMoving = true;
        gameManager.playerStatus = GameManager.Status.Forward;
        StartCoroutine(gameManager.PlayerMove());
    }
}
