using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarController : MonoBehaviour
{
    public bool isMoving = false;

    public void MoveCar(int endValue, float speed)
    {
        Vector3 initPos = transform.position;

        isMoving = true;

        transform.DOLocalMoveZ(endValue, speed).SetSpeedBased().OnComplete(() =>
        {
            isMoving = false;
            transform.position = initPos;
            transform.gameObject.SetActive(false);
        });

    }

}
