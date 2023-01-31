using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace ProjectGTA2_Unity.Cars
{
    public class CarMovementNpc : CarMovement
    {
        [SerializeField] protected float turnRatioNpcNormal = 2.2f;
        [SerializeField, Range(0.1f, 2.0f)] private float obstacleCheckDistance = 0.55f;
        [SerializeField] private Vector3 obstacleCheckRayOffset = new Vector3(0f, 0.2f, 0.5f);
        public bool pathIsBlocked;
        public Color color;

        protected override void OnUpdate()
        {
            base.OnUpdate();

            pathIsBlocked = IsPathBlocked();
            if (pathIsBlocked)
            {
                Brake(brakePower);
                return;
            }                   
            NpcControl();
        }

        protected void NpcControl()
        {
            if (roadDirections.Length == 0)
            {
                CheckRoadDirections(RoadDirection.None);
                return;
            }

            if (roadDirections.Length < 2)
            {
                CheckRoadDirections(roadDirections[0]);
            }
            else
            {
                int r = Util.RandomIntNumber(0, roadDirections.Length);
                CheckRoadDirections(roadDirections[r]);
            }
        }

        protected void CheckRoadDirections(RoadDirection direction)
        {
            Vector3 vec = Vector3.zero;

            switch (direction)
            {
                case RoadDirection.Invalid:
                    vec = Vector3.zero;
                    break;
                case RoadDirection.None:
                    vec = Vector3.zero;
                    break;
                case RoadDirection.Up:
                    vec = Vector3.forward;
                    break;
                case RoadDirection.Down:
                    vec = -Vector3.forward;
                    break;
                case RoadDirection.Left:
                    vec = -Vector3.right;
                    break;
                case RoadDirection.Right:
                    vec = Vector3.right;
                    break;
                default:
                    break;
            }

            if (vec == Vector3.zero)
            {
                Brake(brakePower);
                return;
            }

            if (transform.forward.normalized == vec)
            {
                //Debug.Log("forwardDirection");
                Accerlate();
            }
            else
            {
                NpcRotate(vec);
            }
        }

        protected void NpcRotate(Vector3 vec)
        {
            float step = Time.deltaTime * turnRatioNpcNormal;
            Vector3 direction = Vector3.RotateTowards(transform.forward.normalized, vec, step, 0f);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        /*protected bool CheckTile(Vector3 pos)
        {
            RaycastHit hit;
            Ray ray = new Ray(pos, Vector3.down);
            //Debug.DrawRay(pos, Vector3.down * 0.5f, Color.red);

            if (Physics.Raycast(ray, out hit, 0.5f) && pos != Vector3.zero)
            {
                var tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile.GetTileTypeA() == TileTypeMain.Floor && (tile.GetTileTypeB() == TileTypeSecond.Road || tile.GetTileTypeB() == TileTypeSecond.RoadJunction))
                    {
                        //Debug.Log("Found new Destination Tile _> " + tile.gameObject.name);                       
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }*/


        public Transform obstacleCheckOrigin;
        private bool IsPathBlocked()
        {
            //Vector3 rayOrigin = obstacleCheckOrigin.position;         
            Vector3 rayDirection = transform.forward;
            //Ray ray= new Ray(rayOriginCenter, rayDirection);
            RaycastHit hit;

            bool blocked = false;
            

            if (Physics.BoxCast(obstacleCheckOrigin.position, obstacleCheckRayOffset, rayDirection, out hit, transform.rotation, obstacleCheckDistance))
            {
                if (hit.collider.gameObject.name != gameObject.name)
                {
                    blocked = true;
                }
            }        

            color = blocked? new Color(1f,0f,0f,0.65f) : new Color(0f, 1f, 0f, 0.65f);
            return blocked;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = obstacleCheckOrigin.localToWorldMatrix;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, obstacleCheckRayOffset);
        }
    }
}

