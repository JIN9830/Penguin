using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function : CodingBlock
{
    private void Update()
    {
        if (GameManager.Instance.PlayBlockToggle == true) return;

        blockTweener.Kill();
        transform.localScale = Vector3.one;
        this.GetComponent<CodingBlock>().enabled = false;

    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        blockTweener = GameManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
        GameManager.Instance.SelectedMethods(UIManager.CurrentLayout.Function);
    }

    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in GameManager.Instance.Function)
        {
            if (GameManager.Instance.PlayBlockToggle == false)
                break;

            yield return GameManager.Instance.waitForHalfSeconds;

            GameManager.Instance.PlayerManager.InitializePlayerMoveVector();
            block.GetComponent<CodingBlock>().enabled = true;
            block.MoveOrder();

            if (GameManager.Instance.PlayBlockToggle == true) yield return GameManager.Instance.waitForHalfSeconds;
        }
        if (GameManager.Instance.PlayBlockToggle == true) yield return GameManager.Instance.waitForHalfSeconds;

        GameManager.Instance.SelectedMethods(UIManager.CurrentLayout.Main);
    }
}
