using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Loop : CodingBlock
{
    private void OnEnable()
    {
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1f, 0.3f);
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);
        GM.SelectedMethods(CurrentLayout.Loop);
    }
    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in GM.Loop)
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
