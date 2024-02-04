using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunRight : CodingBlock
{
    public override IEnumerator MoveOrder()
    {
        ToggleHighLight(true);
        yield return null;

    }
}
