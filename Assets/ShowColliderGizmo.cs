using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowColliderGizmo : MonoBehaviour
{
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.2f); // 반투명 초록색

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // 오브젝트의 Collider 크기와 위치 가져오기
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}
