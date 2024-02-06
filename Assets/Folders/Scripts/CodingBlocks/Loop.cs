using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);

    }
    public override IEnumerator Subroutine()
    {
        yield return null;
    }
}
