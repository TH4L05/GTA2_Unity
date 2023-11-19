/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ProjectGTA2_Unity.Tiles;
using ProjectGTA2_Unity.Characters.Data;
using ProjectGTA2_Unity.Cars;
using ProjectGTA2_Unity.Audio;
using ProjectGTA2_Unity.Weapons;

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

        [Header("Ref")]
        [SerializeField] private CharacterData charData;
        [SerializeField] private Rigidbody rb;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] private Armoury weaponBelt;
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent navAgent;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask npcLayer;
        [SerializeField] private AudioEventList audioEvents;

        [Space(2f), Header("Settings")]
        [SerializeField] private bool canFight;
        [SerializeField] private float minFleeDistance = 10f;
        [SerializeField] private bool playerIsEnemy;
        [Range(1.0f, 10.0f), SerializeField] private float enemyDetectRange = 5f;       
        [SerializeField, Range(0.5f, 10.0f)] private float weaponGunAttackDistance = 3f;
        [SerializeField] private float weaponFistAttackDistance = 0.35f;
        [SerializeField] private float obstacleCheckDistance = 1f;
        [SerializeField] private Vector3 obstacleCheckRayOffset = new Vector3(0f, 0.2f, 0.5f);

        #endregion

        #region PrivateFields

        private Transform target;
        private RaycastHit hit;
        private SurfaceType surfaceType;
        private float currentSpeed;
        private float attackDistance;

        private bool onGround;
        private bool onSlope;
        private bool onJump;
        private bool onCar;

        private float startFallHeight;
        private bool isFalling => !onGround && rb.velocity.y < 0;
        private bool wasFalling;
        private Color gizmoColor = new Color(0f,1f,0f,0.35f);
        private float currentIdleTime;
        private float maxIdleTime = 10f;

        #endregion

        #region PublicFields
        public bool IsDead { get; set; }
        public bool InCar { get; set; }

        [Header("Info And Test")]
        public State currentstate;
        public Vector3 destination;
        public bool hasDestination;
        public bool hasDestinationPoint;
        public bool onRotation;
        public bool onChase;
        public bool runningAway;
        public bool pathIsBlocked;
        public Quaternion desiredR;
        public List<Tile> groundTiles = new List<Tile>();

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
            bool wasGrounded = onGround;
            GroundCheck();

            if (IsDead) return;
            if (!wasFalling && isFalling) startFallHeight = transform.position.y;
            FallDamageCheck(wasGrounded);
            wasFalling = isFalling;

            AggroCHeck();
            UpdateState();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(destination, new Vector3(0.1f, 0.1f, 0.1f));
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

       
        private void IdleState()
        {
            if (currentIdleTime >= maxIdleTime)
            {
                destination = GetNearestDestinationInRange(5.5f);

                if (destination != Vector3.zero)
                {
                    currentIdleTime = 0f;
                    hasDestinationPoint = true;
                }

                return;
            }

            navAgent.isStopped = true;
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            currentSpeed = 0f;

            if(!hasDestinationPoint)
            {
                GetDestination();
                currentIdleTime += Time.deltaTime;
            }
            else
            {
                pathIsBlocked = false;
                SetDestination(destination);
            }

            if (hasDestination && !onRotation) currentstate = State.Walk;
        }

        private void WalkState()
        {
            navAgent.isStopped = false;
            currentIdleTime = 0f;

            if (weaponBelt != null) animator.SetBool("GunEquiped", weaponBelt.GunEquipped);
            animator.SetBool("Walk", true);

            LookAtTarget(destination, charData.RotationSensitivity);

            pathIsBlocked = IsPathBlocked();
            Color color = pathIsBlocked ? Color.red : Color.green;
            Debug.DrawRay(transform.position, transform.forward * obstacleCheckDistance, color);
            if (pathIsBlocked)
            {
                hasDestination = false;
                currentstate = State.Idle;
                return;
            }

            currentSpeed = charData.Walkspeed;
            MoveToDestination(destination, currentSpeed);

            var distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.25f)
            {
                hasDestination = false;
                currentstate = State.Idle;
            }
        }

        private void RunState()
        {
            if(weaponBelt != null) animator.SetBool("GunEquiped", weaponBelt.GunEquipped);
            currentIdleTime = 0f;
            animator.SetBool("Run", true);

            LookAtTarget(destination, charData.RotationSensitivity);
          
            pathIsBlocked = IsPathBlocked();
            Color color = pathIsBlocked ? Color.red : Color.green;
            Debug.DrawRay(transform.position, transform.forward * obstacleCheckDistance, color);
            if (pathIsBlocked)
            {
                hasDestination = false;
                if (runningAway)
                {
                    currentstate = State.Flee;
                    return;
                }

                currentstate = State.Idle;
                return;
            }

            currentSpeed = charData.RunSpeed;
            MoveToDestination(destination, currentSpeed);

            var distance = Vector3.Distance(transform.position, destination);
            if (distance <= 0.15f)
            {
                if (runningAway)
                {
                    currentstate = State.Flee;
                    return;
                }
                hasDestination = false;
                currentstate = State.Idle;
            }
        }

        private void ChaseState()
        {
            currentIdleTime = 0f;
            if (!onChase) onChase = true;

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

        private void AttackState()
        {
            currentIdleTime = 0f;
            if (InAttackRange())
            {
                weaponBelt.AttackWithCurrentEquippedWeapon();
            }
            else
            {
                currentstate = State.Chase;
            }
        }

        private void FleeState()
        {
            currentIdleTime = 0f;
            float distance = Util.GetDistance(transform.position, target.position);
            if (distance > minFleeDistance)
            {
                runningAway = false;
                hasDestination = false;
                hasDestinationPoint = false;
                currentstate = State.Idle;
                target = null;
                return;
            }

            if (!runningAway) runningAway = true;
            if (target == null)
            {
                currentstate = State.Idle;
                return;
            }

            Vector3 dirToTarget = transform.position - target.position;
            destination = dirToTarget + transform.position;
            //destination = GetRandomDestination();
            currentstate = State.Run;
        }

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

            if(!pathIsBlocked) CheckForward();

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
            if (onSlope)
            {
                pointForward =  Vector3.ProjectOnPlane(pointForward, hit.normal);
            }

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
            if (onSlope)
            {
                pointBackward = Vector3.ProjectOnPlane(pointBackward, hit.normal);
            }

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
            if (onSlope)
            {
                pointLeft = Vector3.ProjectOnPlane(pointLeft, hit.normal);
            }

            pointLeft.x += Util.RandomFloatNumber(-0.25f, 0.25f);
            pointLeft.z += Util.RandomFloatNumber(-0.25f, 0.25f);

            if (onSlope)
            {
                pointLeft.y += 0.85f;
            }
            else
            {
                pointLeft.y += 0.5f;
            }
           
            hasDestinationPoint = CheckTile(pointLeft);
            pointLeft.y -= 0.5f;
            destination = pointLeft;
            return hasDestinationPoint;
        }

        private bool CheckRight()
        {
            Vector3 pointRight = transform.position + (transform.right * 1.1f);
            if (onSlope)
            {
                pointRight = Vector3.ProjectOnPlane(pointRight, hit.normal);
            }

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
            Vector3 rayOrigin = new Vector3(transform.position.x + obstacleCheckRayOffset.x, transform.position.y + obstacleCheckRayOffset.y, transform.position.z + obstacleCheckRayOffset.z);
            Vector3 rayDirection = transform.forward;
            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, obstacleCheckDistance))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Trigger")) return false;
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("TrafficLightTrigger")) return false;
                //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Car")) return false;
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

        private Vector3 GetRandomDestination()
        {
            Vector3 position = UnityEngine.Random.insideUnitSphere * 5;
            position += transform.position;

            NavMesh.SamplePosition(position, out NavMeshHit hit, 20, 1);

            return hit.position;
        }

        private Vector3 GetNearestDestinationInRange(float range)
        {
            Vector3 destination = Vector3.zero;
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, groundLayer);
            List<Tile> tiles = new List<Tile>();

            foreach (var coll in colliders)
            {
                Tile tile = coll.GetComponent<Tile>();
                if (tile.GetTileTypeA() == TileTypeMain.Floor && tile.GetTileTypeB() == TileTypeSecond.Pavement)
                {
                    tiles.Add(tile);
                }
            }

            if(tiles.Count == 0) return destination;

            float distance = 999f;
            int index = 0;

            for (int i =0; i < tiles.Count; i++)
            {
                float tileDistance = Util.GetDistance(tiles[i].transform.position, transform.position);
                if (tileDistance >= distance) continue;
                distance = tileDistance;
                index = i;
            }

            destination = tiles[index].transform.position;
            return destination;


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
            direction = new Vector3(direction.x, 0, direction.z);
            if (direction == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
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

            if (Physics.Raycast(ray, out hit, 0.6f, groundLayer) && pos != Vector3.zero)
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

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void GunWasFiredNearby(GameObject initiator, Weapon.AttackTypes attackType)
        {
            string name = initiator.gameObject.name;
            SetTarget(initiator.transform);

            if (canFight)
            {
                if (initiator.gameObject.name.Contains("Player") && !playerIsEnemy) return;                 
                currentstate = State.Chase;
            }
            else
            {
                currentstate = State.Flee;
            }

            if (!runningAway)
            {
                int x = Util.RandomIntNumber(0, 100);

                if (x > 66)
                {
                    int r = Util.RandomIntNumber(0, 2);

                    if (r == 0)
                    {
                        audioEvents.PlayAudioEventOneShotAttached("CharacterScreamGun", gameObject);
                    }
                    else
                    {
                        audioEvents.PlayAudioEventOneShotAttached("CharacterScreamHelp", gameObject);
                    }
                }               
            }
        }

        public void ExplosionNearBy(GameObject initiator)
        {

        }

        #endregion
    }
}

