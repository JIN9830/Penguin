using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : CodingBlock
{
    private void Update()
    {
        if (GameManager.Instance.PlayToggle == true) return;

        blockTweener.Kill();
        transform.localScale = Vector3.one;
        this.GetComponent<CodingBlock>().enabled = false;
    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        blockTweener = GameManager.Instance.UIManager.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
        GameManager.Instance.UIManager.SelectedMethods(UIManager.CurrentLayout.Loop);
    }

    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in GameManager.Instance.Loop)
        {
            if (GameManager.Instance.PlayToggle == false)
                break;

            yield return GameManager.Instance.waitForHalfSeconds;

            GameManager.Instance.PlayerManager.InitializePlayerMoveVector();
            block.GetComponent<CodingBlock>().enabled = true;
            block.MoveOrder();

            if (GameManager.Instance.PlayToggle == true) yield return GameManager.Instance.waitForHalfSeconds;
        }
        if (GameManager.Instance.PlayToggle == true) yield return GameManager.Instance.waitForHalfSeconds;

        GameManager.Instance.UIManager.SelectedMethods(UIManager.CurrentLayout.Main);
    }
}
