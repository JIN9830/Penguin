using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : CodingBlock
{
    public override void MoveOrder()
    {
        gameManager.PlayerStatus = GameManager.Status.TurnLeft;

    }
}
