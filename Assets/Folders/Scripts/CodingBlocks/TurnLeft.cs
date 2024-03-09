using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : CodingBlock
{
    Quaternion playerStartRot;
    Quaternion playerEndRot;

    private void Update()
    {
        if (GameManager.Instance.playBlockToggle == true) return;

        blockTweener.Kill();
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        this.GetComponent<CodingBlock>().enabled = false;

    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        blockTweener = GameManager.Instance.UIAnimation.Animation_LeftBlockPlay(this.gameObject);

        playerStartRot = GameManager.Instance.playerManager.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0, -90, 0);

        GameManager.Instance.playerManager.PlayerAnimator.SetTrigger("Turn");
        GameManager.Instance.playerManager.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);

    }
}