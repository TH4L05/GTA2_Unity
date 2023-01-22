using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity
{
    public class NonPlayableBehaviour : MonoBehaviour
    {
        private enum State
        {
            Invalid = -1,
            Idle,
            Walk,
            Run,
            Chase,
            Flee,
        }

        [SerializeField] private CharacterData charData;
        [SerializeField] private bool canFlee;
        private State currentstate;


        public Vector3 pointForward;
        public Vector3 pointBackward;
        public Vector3 pointLeft;
        public Vector3 pointRight;

        public bool tileForward;
        public bool tileBackward;
        public bool tileLeft;
        public bool tileRight;

        public Vector3 destination;
        public bool hasDestination;
        public float speed;

        public List<Tile> groundTiles = new List<Tile>();
        public Color gizmoColor;

        private void Awake()
        {
            currentstate = State.Idle;
        }

        private void Start()
        {
            GetDestination();
        }

        private void Update()
        {
            if(!hasDestination)
            {
                speed = 0f;  
                return;
            }

            speed = charData.Walkspeed;
            transform.Translate(Time.deltaTime * speed *  transform.forward, Space.World);

            var distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.25f)
            {
                hasDestination = false;
                GetDestination();
            }
        }



        public void GetDestination()
        {
            groundTiles.Clear();    

            pointForward = transform.position + (transform.forward * 1.1f);
            pointForward.y = 1f;
            tileForward = CheckTile(pointForward);
            pointForward.y = 0f;

            pointBackward = transform.position + (-transform.forward * 1.1f);
            pointBackward.y = 1f;
            tileBackward = CheckTile(pointBackward);
            pointBackward.y = 0f;

            pointLeft = transform.position + (-transform.right * 1.1f);
            pointLeft.y = 1f;
            tileLeft = CheckTile(pointLeft);
            pointLeft.y = 0f;

            pointRight = transform.position + (transform.right * 1.1f);
            pointRight.y = 1f;
            tileRight = CheckTile(pointRight);
            pointRight.y = 0f;

            SetDestination();
        }

        public void SetDestination()
        {
            if (tileForward)
            {
                destination = pointForward;
                hasDestination = true;
                transform.LookAt(pointForward);
                return;
            }
            else
            {
                int x = UnityEngine.Random.Range(0, 2);
                
                switch (x)
                {
                    case 0:
                        if (tileLeft)
                        {
                            destination = pointLeft;
                            transform.LookAt(pointLeft);
                            hasDestination = true;
                            return;
                        }
                        break;
                    
                    case 1:
                        if (tileRight)
                        {
                            destination = pointRight;
                            transform.LookAt(pointRight);
                            hasDestination = true;
                            return;
                        }
                        break;                    
                }
            }

            if(tileBackward)
            {
                destination = pointBackward;
                transform.LookAt(pointBackward);
                hasDestination = true;
                return;
            }

            Debug.Log("NO DESTINATION");
            //GetDestination();
        }

        public bool CheckTile(Vector3 pos)
        {
            RaycastHit hit;
            Ray ray = new Ray(pos, Vector3.down);
            Debug.DrawRay(pos, Vector3.down * 2f, Color.red);

            if (Physics.Raycast(ray, out hit, 2f) && pos != Vector3.zero)
            {
                var tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile.GetTileTypeA() == TileTypeA.Floor && tile.GetTileTypeB() == TileTypeB.Pavement)
                    {
                        Debug.Log("Found new Destination Tile _> " + tile.gameObject.name);
                        groundTiles.Add(tile);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;

            Gizmos.DrawCube(pointForward, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.DrawCube(pointBackward, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.DrawCube(pointLeft, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.DrawCube(pointRight, new Vector3(0.3f, 0.3f, 0.3f));
        }
    }
}

