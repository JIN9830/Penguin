using DG.Tweening;
using System.Collections;
using UnityEngine;
using static GameManager;

public class Loop : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        BlockTweener = CodingUIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        GameManager_Instance.currentMethod = ECurrentMethod.Loop;
        CodingUIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Loop);
        
    }
    private void Update()
    {
        if (GameManager_Instance.IsCompilerRunning == false)
        {
            BlockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
}
