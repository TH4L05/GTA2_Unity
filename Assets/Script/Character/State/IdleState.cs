

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ProjectGTA2_Unity.Characters.AI;
using Unity.VisualScripting;

namespace ProjectGTA2_Unity
{
    public class IdleState : BaseState
    {
        private NonPlayerStateMachine sm;
        private bool hasNewDestination;

        public IdleState(string name, NonPlayerStateMachine stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter()
        {
            sm.ShowMessage("Enter " + name + " State");
            //sm.navAgent.isStopped = true;
            
            sm.ParseMono(this);         
            hasNewDestination = false;
        }

        public override void UpdateState()
        {
            sm.hasDestination = hasNewDestination;
            if (!hasNewDestination) return;

            sm.ChangeState(sm.walkingState);

            /*if (sm.destination != Vector3.zero)
            {
                sm.ChangeState(sm.walkingState);
            }*/
        }

        public override void Exit()
        {
            sm.transform.LookAt(sm.destination);
            //sm.navAgent.isStopped = false;
        }

        IEnumerator ShortWait()
        {
            yield return new WaitForSeconds(2f);
            GetDestination();
            //sm.destination = GetNewDestination();
        }

        public override void ParseMono(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(ShortWait());
        }

        #region Destination Test 1

        private Vector3 GetNewDestination()
        {       
            Collider[] tiles = Physics.OverlapBox(new Vector3(sm.transform.position.x, sm.transform.position.y, sm.transform.position.z + 0.5f), new Vector3(1f, 1f, 2.5f), sm.transform.rotation, sm.groundLayer);
            List<Tile> groundTiles = new List<Tile>();
           
            foreach (var collider in tiles)
            {
                Tile tile = collider.GetComponent<Tile>();

                if(tile ==null) continue;

                if (tile.GetTileTypeA() == TileTypeA.Floor && tile.GetTileTypeB() == TileTypeB.Pavement)
                {
                    groundTiles.Add(tile);
                }
            }

            sm.groundTiles = groundTiles;
            if (groundTiles.Count == 0) return Vector3.zero;

            int index = Util.RandomIntNumber(0, groundTiles.Count);

            return groundTiles[index].transform.position;
        }

        protected Vector3 RandomTargetPosition()
        {
            Vector3 position = UnityEngine.Random.insideUnitSphere * 5;
            position += sm.gameObject.transform.position;

            NavMesh.SamplePosition(position, out NavMeshHit hit, 20, 1);

            return hit.position;
        }

        #endregion


        private void GetDestination()
        {
            hasNewDestination = false;
            Debug.Log("GetNewDestination");
            sm.groundTiles.Clear();
            sm.destination = Vector3.zero;

            Vector3 newDestination = sm.transform.position + (sm.transform.forward * 1.1f);

            newDestination = new Vector3(newDestination.x, 0.5f, newDestination.z);
            sm.destination = newDestination;

            CheckDestinationTile();
        }

        private void CheckDestinationTile()
        {
            RaycastHit hit;
            Ray ray = new Ray(sm.destination, Vector3.down);
            Debug.DrawRay(sm.destination, Vector3.down * 2f, Color.red);

            if (Physics.Raycast(ray, out hit, 2f) && sm.destination != Vector3.zero)
            {
                var tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    Debug.Log("Found new Destination Tile _> " + tile.gameObject.name);
                    if (tile.GetTileTypeA() == TileTypeA.Floor && tile.GetTileTypeB() == TileTypeB.Pavement)
                    {
                        Debug.Log("Found new Walkable Destination Tile");
                        sm.groundTiles.Add(tile);
                        hasNewDestination = true;
                        sm.lastDestination = sm.destination;

                        //sm.ParseMono(this);

                    }
                    else
                    {
                        sm.ParseMono(this);
                        //GetDestination();
                    }
                }
                else
                {
                    sm.ParseMono(this);
                    //GetDestination();
                }
            }
            else
            {
                sm.ParseMono(this);
                //GetDestination();
            }
        }
    }
}

