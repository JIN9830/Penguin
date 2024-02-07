using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Function : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        GM.SelectedMethods(CurrentLayout.Function);
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

        GM.SelectedMethods(CurrentLayout.Main);
    }
}
