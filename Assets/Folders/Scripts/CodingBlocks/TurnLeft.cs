using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        Gm.playerObject.transform.rotation *= Quaternion.Euler(0, -90, 0);
    }
}