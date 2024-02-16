using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunRight : CodingBlock
{
    Quaternion playerStartRot;
    Quaternion playerEndRot;
    private void Update()
    {
        if (GameManager.Instance.playBlockToggle == true) return;

        blockTweener.Kill();
        transform.localScale = Vector3.one;
        this.GetComponent<CodingBlock>().enabled = false;

    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);

        playerStartRot = GameManager.Instance.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0, 90, 0);

        GameManager.Instance.playerAnimator.SetTrigger("Turn");
        GameManager.Instance.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);
    }
}
