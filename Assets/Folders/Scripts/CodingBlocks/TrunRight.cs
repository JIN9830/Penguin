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
        playerStartRot = Gm.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0, 90, 0);
        ToggleHighLight(true);
        Gm.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);
        Gm.playerAnimator.SetTrigger("TurnRight");
    }
}
