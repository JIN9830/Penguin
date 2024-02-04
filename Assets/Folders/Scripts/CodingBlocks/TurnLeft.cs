using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : CodingBlock
{
    public override IEnumerator MoveOrder()
    {
        ToggleHighLight(true);
        yield return null;

    }
}
