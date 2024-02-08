using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject CameraTarget;

    private void Awake()
    {
        //CameraTarget = GameManager.Instance.playerObject.transform.GetChild(2).gameObject;
    }

}
