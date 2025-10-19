using DG.Tweening;
using UnityEngine;

public class Function : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        BlockTweener = CodingUIManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        BlockCodingManager.ECurrentMethod = BlockCodingManager.CurrentMethod.Function;
        CodingUIManager.Instance.SelectMethod(CodingUIManager.CurrentLayout.Function);

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
