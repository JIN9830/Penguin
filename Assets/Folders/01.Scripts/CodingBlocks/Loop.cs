using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Loop : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        BlockTweener = CodingUIManager.Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        GameManager.Instance.currentMethod = GameManager.CurrentMethod.Loop;
        CodingUIManager.Instance.SelectMethod(CodingUIManager.ECurrentLayout.Loop);
        
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
