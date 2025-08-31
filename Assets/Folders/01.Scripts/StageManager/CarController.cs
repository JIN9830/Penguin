using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarController : MonoBehaviour
{
    public bool isMoving = false;

    public void MoveCar(float moveDistance, float moveSpeed, Transform StartPos)
    {
        transform.position = StartPos.position;
        transform.rotation = StartPos.rotation;

        isMoving = true;

        Vector3 targetPosition = transform.position + transform.forward * moveDistance;

        transform.DOMove(targetPosition, moveSpeed).SetSpeedBased().OnComplete(() =>
        {
            isMoving = false;
            transform.position = StartPos.position;
            transform.rotation = StartPos.rotation;
            transform.gameObject.SetActive(false);
        });

    }

}
