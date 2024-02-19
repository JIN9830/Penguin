using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Loop : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        blockTweener = GameManager.Instance.UI.ForwardBlock_PlayAnimation(this.gameObject);
        GameManager.Instance.SelectedMethods(CurrentLayout.Loop);
    }

    private void Update()
    {
        if (GameManager.Instance.playBlockToggle == true) return;

        blockTweener.Kill();
        transform.localScale = Vector3.one;
        this.GetComponent<CodingBlock>().enabled = false;
    }

    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in GameManager.Instance.Loop)
        {
            if (GameManager.Instance.playBlockToggle == false)
                break;

            yield return GameManager.Instance.waitForHalfSeconds;

            GameManager.Instance.PlayerMoveVectorInit();
            block.GetComponent<CodingBlock>().enabled = true;
            block.MoveOrder();

            if (GameManager.Instance.playBlockToggle == true) yield return GameManager.Instance.waitForHalfSeconds;
        }
        if (GameManager.Instance.playBlockToggle == true) yield return GameManager.Instance.waitForHalfSeconds;

        GameManager.Instance.SelectedMethods(CurrentLayout.Main);
    }
}
