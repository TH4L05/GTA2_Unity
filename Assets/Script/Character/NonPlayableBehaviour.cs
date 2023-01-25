/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ProjectGTA2_Unity.Characters.Data;
using ProjectGTA2_Unity.Cars;

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

        #region SerializedFields

        [SerializeField] private CharacterData charData;
        [SerializeField] private Rigidbody rb;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] private Armoury weaponBelt;
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent navAgent;

        [SerializeField] private bool canFight;
        [SerializeField] private bool playerIsEnemy;
        [SerializeField] private float enemyDetectRange = 5f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask npcLayer;
        [SerializeField] private float weaponGunAttackDistance = 3f;
        [SerializeField] private float weaponFistAttackDistance = 0.55f;

        #endregion

        #region PrivateFields

        private RaycastHit hit;
        private SurfaceType surfaceType;
        private float attackDistance;
        private bool onGround;
        private bool onSlope;
        private bool onJump;
        private bool onCar;
        private float startFallHeight;
        private bool isFalling => !onGround && rb.velocity.y < 0;
        private bool wasFalling;

        #endregion

        #region PublicFields
        public bool IsDead { get; set; }

        [Header("Info")]
        public State currentstate;
        public Vector3 destination;
        public bool hasDestination;
        public bool hasDestinationPoint;
        public bool onRotation;
        public bool onChase;
        public bool onFlee;
        public bool pathBlocked;

        public float speed;
        public Quaternion desiredR;

        public List<Tile> groundTiles = new List<Tile>();
        public Color gizmoColor;
        public Transform target;

        #endregion

        #region UnityFunctions

        private void Awake()
        {
            currentstate = State.Idle;
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (IsDead) return;

            bool wasGrounded = onGround;

            GroundCheck();
            if (!wasFalling && isFalling) startFallHeight = transform.position.y;
            FallDamageCheck(wasGrounded);
            wasFalling = isFalling;

            AggroCHeck();
            UpdateState();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(destination, new Vector3(0.3f, 0.3f, 0.3f));
        }

        #endregion

        #region States

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

        #region Idle

        private void IdleState()
        {
            navAgent.isStopped = true;
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            speed = 0f;

            if(!hasDestinationPoint)
            {
                GetDestination();
            }
            else
            {
                pathBlocked = false;
                SetDestination(destination);
            }

            if (hasDestination  && !onRotation) currentstate = State.Walk;
        }

        #endregion

        #region Walk

        private void WalkState()
        {
            navAgent.isStopped = false;
            if (weaponBelt != null) animator.SetBool("GunEquiped", weaponBelt.GunEquipped);
            animator.SetBool("Walk", true);

            var color = Color.green;
            pathBlocked = IsPathBlocked();
            color = pathBlocked ? Color.red : color;
            Debug.DrawRay(transform.position, transform.forward * 0.85f, color);
            /*if (pathBlocked)
            {
                currentstate = State.Idle;
                return;
            }*/

            LookAtTarget(destination, charData.RotationSensitivity);
            speed = charData.Walkspeed;
            MoveToDestination(destination, speed);

            var distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.25f)
            {
                navAgent.isStopped = true;
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


            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward * 0.65f, Color.red);
            if (IsPathBlocked())
            {
                speed = 0;
                currentstate = State.Idle;
                return;
            }

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
            //Debug.Log("MoveNpc");
            navAgent.speed = speed;
            navAgent.stoppingDistance = charData.StoppingDistance;
            navAgent.SetDestination(destination);
            //transform.Translate(Time.deltaTime * speed * transform.forward, Space.World);
        }

        private void GetDestination()
        {
            if (hasDestinationPoint) return;

            groundTiles.Clear();
            destination = Vector3.zero;

            if(!pathBlocked) CheckForward();

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

            pointForward.x += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointForward.z += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointForward.y += 0.5f;
            hasDestinationPoint = CheckTile(pointForward);
            pointForward.y -= 0.5f;
            destination = pointForward;
            return hasDestinationPoint;

        }

        private bool CheckBackward()
        {
            Vector3 pointBackward = transform.position + (-transform.forward * 1.1f);

            pointBackward.x += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointBackward.z += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointBackward.y += 0.5f;
            hasDestinationPoint = CheckTile(pointBackward);
            pointBackward.y -= 0.5f;
            destination = pointBackward;
            return hasDestinationPoint;
        }

        private bool CheckLeft()
        {
            Vector3 pointLeft = transform.position + (-transform.right * 1.1f);

            pointLeft.x += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointLeft.z += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointLeft.y += 0.5f;
            hasDestinationPoint = CheckTile(pointLeft);
            pointLeft.y -= 0.5f;
            destination = pointLeft;
            return hasDestinationPoint;
        }

        private bool CheckRight()
        {
            Vector3 pointRight = transform.position + (transform.right * 1.1f);

            pointRight.x += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointRight.z += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointRight.y += 0.5f;
            hasDestinationPoint = CheckTile(pointRight);
            pointRight.y -= 0.5f;
            destination = pointRight;
            return hasDestinationPoint;
        }

        private void SetDestination(Vector3 destination)
        {
            //if (OnRotation(destination))
            //{
            //    onRotation = true;
            //    LookAtTarget(destination, charData.RotationSensitivity);
            //}
            //else
            //{
                onRotation = false;
                hasDestinationPoint = false;
                hasDestination = true;
            //}
        }

        private bool IsPathBlocked()
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 0.85f))
            {
                return true;
            }
            return false;
        }

        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
            animator.SetBool("OnGround", onGround);
            onCar = false;

            Vector3 rayOrigin = groundCheck.position;
            Vector3 rayDirection = Vector3.down;
            Ray ray = new Ray(rayOrigin, rayDirection);

            if (Physics.Raycast(ray, out hit, 0.2f))
            {
                SlopeCheck(hit);

                var tile = hit.collider.gameObject.GetComponent<Tile>();
                if (tile != null)
                {
                    surfaceType = tile.GetSurfaceType();
                    //return;
                }

                var car = hit.collider.gameObject.GetComponent<Car>();
                if (car)
                {
                    onCar = true;
                }
            }

            if (onJump && onGround)
            {
                onJump = false;
                animator.SetBool("Jump", false);
            }
        }

        private void SlopeCheck(RaycastHit hit)
        {
            onSlope = false;
            if (hit.normal != Vector3.up)
            {
                onSlope = true;
            }
        }

        private void FallDamageCheck(bool wasGrounded)
        {
            float fallHeight = startFallHeight + transform.position.y;

            if (!wasGrounded && onGround)
            {
                if (fallHeight > charData.MinFallHeight)
                {
                    //Game.Instance.player.TakeDamage(fallHeight * 10f, DamageType.Normal, gameObject.tag);
                }
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
            if (targetposition == Vector3.zero) return;
            Vector3 direction = (targetposition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ratio);
        }

        private bool OnRotation(Vector3 destination)
        {
            Vector3 direction = Vector3.Normalize(destination - transform.position);
            desiredR = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            if (transform.rotation.eulerAngles.y >= (desiredR.eulerAngles.y - 0.1f) && transform.rotation.eulerAngles.y <= (desiredR.eulerAngles.y + 0.1f))
            {
                return false;
            }
            return true;
        }

        private bool CheckTile(Vector3 pos)
        {
            RaycastHit hit;
            Ray ray = new Ray(pos, Vector3.down);
            //Debug.DrawRay(pos, Vector3.down * 0.5f, Color.red);

            if (Physics.Raycast(ray, out hit, 0.5f) && pos != Vector3.zero)
            {
                var tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile.GetTileTypeA() == TileTypeMain.Floor && (tile.GetTileTypeB() == TileTypeSecond.Pavement || tile.GetTileTypeB() == TileTypeSecond.RoadJunction))
                    {
                        //Debug.Log("Found new Destination Tile _> " + tile.gameObject.name);
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
    }
}

