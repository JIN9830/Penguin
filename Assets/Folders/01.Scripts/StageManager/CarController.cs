using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarController : MonoBehaviour
{
    public bool isMoving = false;

    public void MoveCar(int endValue, int time)
    {
        Vector3 initPos = transform.position;

        isMoving = true;

        transform.DOMoveZ(endValue, time).OnComplete(() =>
        {
            isMoving = false;
            transform.position = initPos;
            transform.gameObject.SetActive(false);
        });
    }

}
