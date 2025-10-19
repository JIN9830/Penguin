using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Loop : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        BlockTweener = CodingUIManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        BlockCodingManager.ECurrentMethod = BlockCodingManager.CurrentMethod.Loop;
        CodingUIManager.Instance.SelectMethod(CodingUIManager.CurrentLayout.Loop);

        BlockCodingManager.SubBlockCompiler = StartCoroutine(BlockCodingManager.SubBlockCompiler_Co());
    }
    private void Update()
    {
        if (BlockCodingManager.IsCompilerRunning == false)
        {
            BlockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
}
