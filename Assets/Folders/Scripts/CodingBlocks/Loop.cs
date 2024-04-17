using DG.Tweening;
using System.Collections;
using UnityEngine;
using static GameManager;

public class Loop : CodingBlock
{
    private void Update()
    {
        if (GameManager_Instance.ExecutionToggle == false)
        {
            _blockTweener.Kill();
            transform.localScale = Vector3.one;
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        _blockTweener = CodingUIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);

        GameManager_Instance.currentMethod = ECurrentMethod.Loop;
        CodingUIManager_Instance.SelectedMethods(CodingUIManager.ECurrentLayout.Loop);
        
    }
}
