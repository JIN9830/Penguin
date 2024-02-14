using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : CodingBlock
{
    Quaternion playerStartRot;
    Quaternion playerEndRot;

    public override void MoveOrder()
    {
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1f, 0.3f);

        ToggleHighLight(true);

        playerStartRot = GM.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0,-90,0);

        GM.playerAnimator.SetTrigger("Turn");
        GM.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);
        
    }
}