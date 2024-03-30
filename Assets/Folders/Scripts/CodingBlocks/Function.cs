using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Function : CodingBlock
{
    private void Update()
    {
        if (GameManager_Instance.PlayToggle == false)
        {
            blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        blockTweener = UIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
        UIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Function);
        GameManager_Instance.currentMethod = ECurrentMethod.Function;
    }
}
