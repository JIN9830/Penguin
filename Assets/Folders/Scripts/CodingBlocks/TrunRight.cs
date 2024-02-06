using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunRight : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        GM.playerObject.transform.rotation *= Quaternion.Euler(0, 90, 0);
    }
}
