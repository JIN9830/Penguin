using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Loop : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        Gm.SelectedMethods(CurrentLayout.Loop);
    }
    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in Gm.Loop)
        {
            if (Gm.playBlockToggle == false)
                break;

            yield return Gm.waitForHalfSeconds;

            Gm.PlayerMoveVectorInit();
            block.GetComponent<CodingBlock>().enabled = true;
            block.MoveOrder();

            if (Gm.playBlockToggle == true) yield return Gm.waitForHalfSeconds;
        }
        if (Gm.playBlockToggle == true) yield return Gm.waitForHalfSeconds;

        Gm.SelectedMethods(CurrentLayout.Main);
    }
}
