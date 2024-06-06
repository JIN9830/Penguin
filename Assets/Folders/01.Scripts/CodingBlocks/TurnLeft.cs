using DG.Tweening;
using UnityEngine;
using static GameManager;


public class TurnLeft : CodingBlock
{
    private Quaternion _playerStartRot;
    private Quaternion _playerEndRot;

    public override void MoveOrder()
    {
        ToggleHighLight(true);

        BlockTweener = CodingUIManager_Instance.UIAnimation.Animation_LeftBlockPlay(this.gameObject);

        _playerStartRot = PlayerManager_Instance.PlayerObject.transform.rotation;
        _playerEndRot = _playerStartRot * Quaternion.Euler(0, -90, 0);

        PlayerManager_Instance.PlayerAnimator.SetTrigger("Turn");
        PlayerTweener = PlayerManager_Instance.PlayerObject.transform.DORotateQuaternion(_playerEndRot, 0.3f);

    }
    private void Update()
    {
        if (GameManager.GameManager_Instance.IsCompilerRunning == false)
        {
            BlockTweener.Kill();
            PlayerTweener.Kill();
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            this.GetComponent<CodingBlock>().enabled = false;
        }
    }
}