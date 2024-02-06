using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
    }

    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in GM.Function)
        {
            if (GM.playBlockToggle == false)
                break;

            yield return GM.waitForHalfSeconds;

            GM.PlayerMoveVectorInit();
            block.GetComponent<CodingBlock>().enabled = true;
            block.MoveOrder();

            if (GM.playBlockToggle == true) yield return GM.waitForHalfSeconds;
        }
        if (GM.playBlockToggle == true) yield return GM.waitForHalfSeconds;
    }
}
