using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forward : CodingBlock
{
    public override void MoveOrder()
    {
        gameManager.PlayerStatus = GameManager.Status.Forward;

    }
}
