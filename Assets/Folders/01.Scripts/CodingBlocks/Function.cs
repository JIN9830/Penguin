using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        BlockTweener = CodingUIManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        GameManager.Instance.ECurrentMethod = GameManager.CurrentMethod.Function;
        CodingUIManager.Instance.SelectMethod(CodingUIManager.CurrentLayout.Function);
    }
    private void Update()
    {
        if (GameManager.Instance.IsCompilerRunning == false)
        {
            BlockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
}
