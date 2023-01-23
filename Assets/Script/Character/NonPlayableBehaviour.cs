using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectGTA2_Unity.Characters;
using ProjectGTA2_Unity.Characters.Data;

namespace ProjectGTA2_Unity
{
    public class NonPlayableBehaviour : MonoBehaviour
    {
        public enum State
        {
            Invalid = -1,
            Idle,
            Walk,
            Run,
            Chase,
            Attack,
            Flee,
        }

        [SerializeField] private CharacterData charData;
        [SerializeField] private WeaponBelt weaponBelt;
        [SerializeField] private Animator animator;

        [SerializeField] private bool canFight;
        [SerializeField] private bool playerIsEnemy;
        [SerializeField] private float enemyDetectRange = 5f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask npcLayer;
        [SerializeField] private float weaponGunAttackDistance = 3f;
        [SerializeField] private float weaponFistAttackDistance = 0.55f;

        private float attackDistance;
 
        [Header("Info")]
        public State currentstate;
        public Vector3 destination;
        public bool hasDestination;
        public bool hasDestinationPoint;
        public bool onRotation;
        public bool onChase;
        public bool onFlee;

        public float speed;
        public Quaternion desiredR;

        public List<Tile> groundTiles = new List<Tile>();
        public Color gizmoColor;
        public Transform target;

        #region UnityFunctions

        private void Awake()
        {
            currentstate = State.Idle;
        }

        private void Start()
        {
        }

        private void Update()
        {
            AggroCHeck();
            UpdateState();
        }

        #endregion

        private void UpdateState()
        {
            switch (currentstate)
            {
                case State.Invalid:
                    break;

                case State.Idle:
                    IdleState();
                    break;

                case State.Walk:
                    WalkState();
                    break;

                case State.Run:
                    RunState();
                    break;

                case State.Chase:
                    ChaseState();
                    break;

                case State.Attack:
                    AttackState();
                    break;

                case State.Flee:
                    FleeState();
                    break;

                default:
                    break;
            }
        }

        private void AggroCHeck()
        {
            if (onChase) return;

            Collider[] playerCollider = Physics.OverlapSphere(transform.position, enemyDetectRange, playerLayer);

            if (playerCollider.Length > 0 && playerIsEnemy)
            {
                target = playerCollider[0].transform;
                currentstate = State.Chase;
                onChase = true;
                return;
            }

            Collider[] npcCollider = Physics.OverlapSphere(transform.position, enemyDetectRange, npcLayer);

        }

        private bool InAttackRange()
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackDistance)
            {
                return true;
            }
            return false;
        }

        #region States

        #region Idle

        private void IdleState()
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            speed = 0f;

            if(!hasDestinationPoint)
            {
                GetDestination();
            }
            else
            {
                SetDestination(destination);
            }

            if (hasDestination) currentstate = State.Walk;
        }

        #endregion

        #region Walk

        private void WalkState()
        {
            if (weaponBelt != null) animator.SetBool("GunEquiped", weaponBelt.GunEquipped);
            animator.SetBool("Walk", true);

            speed = charData.Walkspeed;
            MoveToDestination(destination, speed);

            var distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.25f)
            {
                hasDestination = false;
                currentstate = State.Idle;
            }
        }

        #endregion

        #region Run

        private void RunState()
        {
            if(weaponBelt != null) animator.SetBool("GunEquiped", weaponBelt.GunEquipped);
            animator.SetBool("Run", true);

            speed = charData.RunSpeed;
            MoveToDestination(destination, speed);

            if (target != null)
            {
                destination = target.position;
            }

            var distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.25f)
            {
                hasDestination = false;
                currentstate = State.Idle;
            }
        }

        #endregion

        #region Chase

        private void ChaseState()
        {
            if (!InAttackRange())
            {
                destination = target.position;
                MoveToDestination(destination, charData.RunSpeed);
            }
            else
            {
                currentstate = State.Attack;
            }
        }


        #endregion

        #region Attack

        private void AttackState()
        {
            if (InAttackRange())
            {
                weaponBelt.AttackWithCurrentEquippedWeapon();
            }
            else
            {
                currentstate = State.Chase;
            }
        }

        #endregion

        #region Flee

        private void FleeState()
        {

        }

        #endregion

        #endregion

        #region Destination

        private void MoveToDestination(Vector3 destination, float speed)
        {
            Debug.Log("MoveNpc");
            transform.Translate(Time.deltaTime * speed * transform.forward, Space.World);
        }

        private void GetDestination()
        {
            if (hasDestinationPoint) return;

            groundTiles.Clear();
            destination = Vector3.zero;

            CheckForward();

            if (!hasDestinationPoint)
            {
                int x = Util.RandomIntNumber(0, 2);

                if (x == 0)
                {
                    CheckLeft();
                }
                else
                {
                    CheckRight();
                }

                if (!hasDestinationPoint)
                {
                    CheckBackward();
                }
            }
        }

        private bool CheckForward()
        {
            Vector3 pointForward = transform.position + (transform.forward * 1.1f);
            pointForward.y = 1f;
            hasDestinationPoint = CheckTile(pointForward);
            pointForward.y = 0f;
            destination = pointForward;
            return hasDestinationPoint;

        }

        private bool CheckBackward()
        {
            Vector3 pointBackward = transform.position + (-transform.forward * 1.1f);
            pointBackward.y = 1f;
            hasDestinationPoint = CheckTile(pointBackward);
            pointBackward.y = 0f;
            destination = pointBackward;
            return hasDestinationPoint;
        }

        private bool CheckLeft()
        {
            Vector3 pointLeft = transform.position + (-transform.right * 1.1f);
            pointLeft.y = 1f;
            hasDestinationPoint = CheckTile(pointLeft);
            pointLeft.y = 0f;
            destination = pointLeft;
            return hasDestinationPoint;
        }

        private bool CheckRight()
        {
            Vector3 pointRight = transform.position + (transform.right * 1.1f);
            pointRight.y = 1f;
            hasDestinationPoint = CheckTile(pointRight);
            pointRight.y = 0f;
            destination = pointRight;
            return hasDestinationPoint;
        }

        private void SetDestination(Vector3 destination)
        {
            if (OnRotation(destination))
            {
                onRotation = true;
                LookAtTarget(destination, charData.RotationSensitivity);
            }
            else
            {
                onRotation = false;
                hasDestinationPoint = false;
                hasDestination = true;
            }
        }

        #endregion

        #region Other

        private void LookAtTarget(Vector3 targetposition)
        {
            transform.LookAt(targetposition);
        }

        private void LookAtTarget(Vector3 targetposition, float ratio)
        {
            Vector3 direction = (targetposition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ratio);
        }

        private bool OnRotation(Vector3 destination)
        {
            Vector3 direction = Vector3.Normalize(destination - transform.position);
            desiredR = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            if (transform.rotation != desiredR)
            {
                return true;
            }
            return false;
        }

        private bool CheckTile(Vector3 pos)
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

        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(destination, new Vector3(0.3f, 0.3f, 0.3f));
        }
    }
}

