using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : CodingBlock
{
    public override void MoveOrder()
    {
        gameManager.playerStatus = GameManager.Status.TurnLeft;

    }
}
