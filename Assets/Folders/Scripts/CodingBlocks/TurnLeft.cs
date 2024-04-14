using DG.Tweening;
using UnityEngine;
using static GameManager;


public class TurnLeft : CodingBlock
{
    Quaternion playerStartRot;
    Quaternion playerEndRot;

    private void Update()
    {
        if (GameManager.GameManager_Instance.PlayToggle == false)
        {
            blockTweener.Kill();
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        blockTweener = CodingUIManager_Instance.UIAnimation.Animation_LeftBlockPlay(this.gameObject);

        playerStartRot = PlayerManager_Instance.playerObject.transform.rotation;
        playerEndRot = playerStartRot * Quaternion.Euler(0, -90, 0);

        PlayerManager_Instance.PlayerAnimator.SetTrigger("Turn");
        PlayerManager_Instance.playerObject.transform.DORotateQuaternion(playerEndRot, 0.3f);

    }
}