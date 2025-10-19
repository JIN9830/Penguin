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

        BlockCodingManager.Instance.ECurrentMethod = BlockCodingManager.CurrentMethod.Function;
        CodingUIManager.Instance.SelectMethod(CodingUIManager.CurrentLayout.Function);

        BlockCodingManager.Instance.SubBlockCompiler = StartCoroutine(BlockCodingManager.Instance.SubBlockCompiler_Co());
    }
    private void Update()
    {
        if (BlockCodingManager.Instance.IsCompilerRunning == false)
        {
            BlockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
}
