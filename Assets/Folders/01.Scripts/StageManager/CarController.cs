using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _childCarObject =new List<GameObject>();

    void Start()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            _childCarObject.Add(transform.GetChild(i).gameObject);
        }
    }


}
