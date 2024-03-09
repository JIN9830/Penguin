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
        if (GameManager.Instance.PlayBlockToggle == true) return;

        blockTweener.Kill();
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        this.GetComponent<CodingBlock>().enabled = false;

    }
    public override void MoveOrder()
    {
        ToggleHighLight(true);

        blockTweener = GameManager.Instance.UIAnimation.Animation_RightBlockPlay(this.gameObject);

        playerStartRot = GameManager.Instance.PlayerManager.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0, 90, 0);

        GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("Turn");
        GameManager.Instance.PlayerManager.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);
    }
}
