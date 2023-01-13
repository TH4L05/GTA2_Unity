/// <author>Thoams Krahl</author>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    #region Fields

    [SerializeField] private Color gizmoColor = new Color(0.25f, 0.45f, 0.65f, 0.55f);
    [SerializeField] private bool drawWired;

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = gizmoColor;

        Collider coll = GetComponent<Collider>();
        switch (coll)
        {
            case BoxCollider:
                var boxColl = coll as BoxCollider;

                if (drawWired)
                {
                    Gizmos.DrawWireCube(Vector3.zero, boxColl.size);
                }
                else
                {
                    Gizmos.DrawCube(Vector3.zero, boxColl.size);
                }
                break;
   

            case SphereCollider:
                var sphereColl = coll as SphereCollider;

                if (drawWired)
                {
                    Gizmos.DrawWireSphere(Vector3.zero, sphereColl.radius);
                }
                else
                {
                    Gizmos.DrawSphere(Vector3.zero, sphereColl.radius);
                }
                break;


            case CapsuleCollider:
                var capsuleColl = coll as CapsuleCollider;

                if (drawWired)
                {
                    Gizmos.DrawWireSphere(Vector3.zero, capsuleColl.radius);
                }
                else
                {
                    Gizmos.DrawSphere(Vector3.zero, capsuleColl.radius);
                }
                break;

            case MeshCollider:
                var meshColl = coll as MeshCollider;

                Gizmos.DrawMesh(meshColl.sharedMesh);
                break;


            default:

                if (drawWired)
                {
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                }
                else
                {
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                }
                break;
        }
    }
}
