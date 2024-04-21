using DG.Tweening;
using UnityEngine;
using static GameManager;


public class TurnLeft : CodingBlock
{
    private Quaternion _playerStartRot;
    private Quaternion _playerEndRot;

    private void Update()
    {
        if (GameManager.GameManager_Instance.ExecutionToggle == false)
        {
            BlockTweener.Kill();
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        BlockTweener = CodingUIManager_Instance.UIAnimation.Animation_LeftBlockPlay(this.gameObject);

        _playerStartRot = PlayerManager_Instance.playerObject.transform.rotation;
        _playerEndRot = _playerStartRot * Quaternion.Euler(0, -90, 0);

        PlayerManager_Instance.PlayerAnimator.SetTrigger("Turn");
        PlayerManager_Instance.playerObject.transform.DORotateQuaternion(_playerEndRot, 0.3f);

    }
}