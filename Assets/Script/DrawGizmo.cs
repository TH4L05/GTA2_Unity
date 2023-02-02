/// <author>Thoams Krahl</author>

using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    private enum GizmoType
    {
        UseCollider,
        Cube,
        WireCube,
        Sphere,
        WireSphere,
        Line,
        Icon,
        Mesh
    }

    #region Fields

    [SerializeField] private GizmoType type = GizmoType.UseCollider;
    [SerializeField] private Color gizmoColor = new Color(0.25f, 0.45f, 0.65f, 0.55f);
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Header("Collider")]
    [SerializeField] private bool drawWired;

    [Header("Cube")]
    [SerializeField] private Vector3 size = Vector3.one;


    [Header("Sphere")]
    [Range(0.1f, 10f)][SerializeField] private float radius = 0.5f;

    [Header("Line")]
    [SerializeField] private Vector3 startPosition = Vector3.zero;
    [SerializeField] private Vector3 endPosition = Vector3.one;

    [Header("Icon")]
    [SerializeField] private string iconName = "Icon";
    [SerializeField] private bool allowScaling = true;
    [SerializeField] private Color tint = Color.white;

    [Header("Mesh")]
    [SerializeField] private Mesh mesh;
    [SerializeField] private Quaternion rotation = Quaternion.Euler(0, 0, 0);
    [SerializeField] private Vector3 scale = Vector3.one;

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = gizmoColor;

        switch (type)
        {
            case GizmoType.UseCollider:
                DrawColliderGizmo();
                break;

            case GizmoType.Cube:
                DrawCubeGizmo();
                break;

            case GizmoType.WireCube:
                DrawCubeGizmoWired();
                break;

            case GizmoType.Sphere:
                DrawSphereGizmo();
                break;

            case GizmoType.WireSphere:
                DrawSphereGizmoWired();
                break;

            case GizmoType.Line:
                DrawLineGizmo();
                break;

            case GizmoType.Icon:
                DrawIconGizmo();
                break;

            case GizmoType.Mesh:
                DrawMeshGizmo();
                break;

            default:
                break;
        }

    }

    private void DrawColliderGizmo()
    {
        Collider coll = GetComponent<Collider>();
        switch (coll)
        {
            case BoxCollider:
                var boxColl = coll as BoxCollider;

                if (drawWired)
                {
                    Gizmos.DrawWireCube(offset, boxColl.size);
                }
                else
                {
                    Gizmos.DrawCube(offset, boxColl.size);
                }
                break;


            case SphereCollider:
                var sphereColl = coll as SphereCollider;

                if (drawWired)
                {
                    Gizmos.DrawWireSphere(offset, sphereColl.radius);
                }
                else
                {
                    Gizmos.DrawSphere(offset, sphereColl.radius);
                }
                break;


            case CapsuleCollider:
                var capsuleColl = coll as CapsuleCollider;

                if (drawWired)
                {
                    Gizmos.DrawWireSphere(offset, capsuleColl.radius);
                }
                else
                {
                    Gizmos.DrawSphere(offset, capsuleColl.radius);
                }
                break;

            case MeshCollider:
                var meshColl = coll as MeshCollider;

                Gizmos.DrawMesh(meshColl.sharedMesh);
                break;


            default:
                break;
        }
    }

    private void DrawCubeGizmo()
    {
        Gizmos.DrawCube(offset, size);
    }

    private void DrawCubeGizmoWired()
    {
        Gizmos.DrawWireCube(offset, size);
    }

    private void DrawSphereGizmo()
    {
        Gizmos.DrawSphere(offset, radius);
    }

    private void DrawSphereGizmoWired()
    {
        Gizmos.DrawWireSphere(offset, radius);
    }

    private void DrawLineGizmo()
    {
        Gizmos.DrawLine(startPosition, endPosition);
    }

    private void DrawIconGizmo()
    {
        Gizmos.DrawIcon(offset, iconName, allowScaling, tint);
    }

    private void DrawMeshGizmo()
    {
        Gizmos.DrawMesh(mesh, 0, offset, rotation, scale);
    }
}

