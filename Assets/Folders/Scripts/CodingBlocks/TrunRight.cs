using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunRight : CodingBlock
{
    Quaternion playerStartRot;
    Quaternion playerEndRot;
    public override void MoveOrder()
    {
        ToggleHighLight(true);

        playerStartRot = GM.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0, 90, 0);

        GM.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);
        GM.playerAnimator.SetTrigger("Turn");
    }
}
