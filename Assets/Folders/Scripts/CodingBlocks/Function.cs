using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Function : CodingBlock
{
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        BlockTweener = CodingUIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        GameManager_Instance.currentMethod = ECurrentMethod.Function;
        CodingUIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Function);
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
