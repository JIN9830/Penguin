using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunRight : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        gameManager.playerStatus = GameManager.Status.TurnRight;

    }
}
