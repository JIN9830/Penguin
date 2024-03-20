using DG.Tweening;
using System.Collections;
using UnityEngine;
using static GameManager;

public class Loop : CodingBlock
{
    private void Update()
    {
        if (GameManager_Instance.PlayToggle == true) return;

        blockTweener.Kill();
        transform.localScale = Vector3.one;
        this.GetComponent<CodingBlock>().enabled = false;
    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);
        blockTweener = UIManager_Instance.UIAnimation.Animation_ForwardBlockPlay(this.gameObject);
        UIManager_Instance.SelectedMethods(UIManager.CurrentLayout.Loop);
    }

    public override IEnumerator Subroutine()
    {
        foreach (CodingBlock block in GameManager_Instance.LoopMethod)
        {
            if (GameManager_Instance.PlayToggle == false)
                break;

            yield return GameManager_Instance.waitForSeconds;

            PlayerManager_Instance.InitializePlayerMoveVector();
            block.GetComponent<CodingBlock>().enabled = true;
            block.MoveOrder();

            if (GameManager_Instance.PlayToggle == true) yield return GameManager_Instance.waitForPointEightSeconds;
        }
        //if (GameManager_Instance.PlayToggle == true) yield return GameManager_Instance.waitForHalfSeconds;

        UIManager_Instance.SelectedMethods(UIManager.CurrentLayout.Main);
    }
}
