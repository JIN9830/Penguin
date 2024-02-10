using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    public GameObject CameraTarget { get; private set; }
    private float moveSpeed = 0.5f;
    private Vector3 touchStartPos;
    private Vector3 offset;

    private Touch touch;
    //private void Update()
    //{
        
    //}
}
