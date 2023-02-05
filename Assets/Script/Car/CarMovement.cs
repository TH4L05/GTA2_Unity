/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace ProjectGTA2_Unity.Cars
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarMovement : MonoBehaviour
    {
        public enum MovementDirection
        {
            Forward,
            Backward
        }

        public enum CarState
        {
            Invalid = -1,
            Parked,
            Idle,
            Brake,
            DriveForward,
            DriveBackward,
            Rotate,
            OnTrafficLightJunction,
        }

        #region SerializedFields

        [Header("Base")]
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected bool isActive = false;
        [SerializeField] protected bool playerControlled;
        [SerializeField] protected bool npcControlled;
        [SerializeField] protected CarState currentState;

        [Header("Values")]
        [SerializeField] protected float accerlation;
        [SerializeField] protected float brakePower;
        [SerializeField] protected float turnRatio;
        [SerializeField] protected float maxForwardSpeed;
        [SerializeField] protected float maxBackwardSpeed;
        [SerializeField] protected float gravityFactor = 15f;
        [SerializeField] protected Gear[] gears = { new Gear(0.33f), new Gear(0.55f), new Gear(1f) };

        [Header("NPC")]
        [SerializeField] protected float turnRatioNpcNormal = 2.2f;
        [SerializeField] protected float turnSpeed = 1.5f;
        [SerializeField] protected int maxGear = 1;
        [SerializeField] private Vector3 obstacleCheckBoxHalfExtents = new Vector3(0.50f, 0.075f, 0.20f);
        [SerializeField] protected Color color;
        [SerializeField] protected Transform obstacleCheckOrigin;

        #endregion

        #region PrivateFields

        protected Rigidbody rb;

        protected bool onGround;
        protected bool onRedTrafficLight;
        protected MovementDirection movementDirection;
        protected float currentSpeed;
        protected float currentBrakePower;
        protected int currentGear = 0;
        protected SurfaceType surfaceType;


        //player
        protected Vector2 input = Vector2.zero;

        //npc
        protected RoadDirection[] roadDirections = new RoadDirection[0];    
        protected RoadDirection lastRoadDirection;
        protected bool pathIsBlocked;
        protected bool onTurn;
        protected Vector3 dirVec = Vector3.zero;
        protected Vector3 dirVecLast;
        protected Vector3 tilePos;
        protected Vector3 tilePosLast;
        protected Vector3 direction;

        #endregion

        #region PublicFields

        public (float, float, float, MovementDirection) MovementValues => (currentSpeed, maxForwardSpeed, maxBackwardSpeed, movementDirection);
        
        #endregion

        #region UnityFunctions

        protected void Awake()
        {
            Initialize();
        }

        protected void Update()
        {
            OnUpdate();
        }

        private void OnDrawGizmosSelected()
        {
            if (obstacleCheckOrigin == null) return;
            Gizmos.matrix = obstacleCheckOrigin.localToWorldMatrix;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, obstacleCheckBoxHalfExtents * 2);
        }

        #endregion

        #region SetupAndUpdate

        public void SetActive(bool player, bool active)
        {
            isActive = active;
            if (player)
            {
                playerControlled = isActive;
                npcControlled = false;
            }
            else
            {
                playerControlled = false;
                npcControlled = isActive;
            }

            if (!isActive)
            {
                currentState = CarState.Parked;
            }

        }

        private void Initialize()
        {
            rb = GetComponent<Rigidbody>();
            if (!isActive)
            {
                currentState = CarState.Parked;
            }
            else
            {
                currentState = CarState.Idle;
            }

        }

        private void OnUpdate()
        {
            GroundCheck();

            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

            if (!isActive) return;
            ControlCheck();
            UpdateState();
        }

        private void ControlCheck()
        {
            if (playerControlled)
            {
                PlayerInputCheck();
                Rotation();
            }
            else if (npcControlled)
            {
                NpcControl();
            }
        }

        private void UpdateState()
        {
            switch (currentState)
            {
                case CarState.Invalid:
                    break;

                case CarState.Parked:
                    OnParked();
                    break;

                case CarState.Idle:
                    OnIdle();
                    break;

                case CarState.Brake:
                    OnBrake();
                    break;

                case CarState.DriveForward:
                    OnDriveForward();
                    break;

                case CarState.DriveBackward:
                    OnDriveBackward();
                    break;

                case CarState.Rotate:
                    OnRotation();
                    break;
                default:
                    break;
            }
        }

        public void OnTrafficLightJunction(TrafficLight.TrafficLightState state)
        {
            switch (state)
            {
                case TrafficLight.TrafficLightState.Disabled:
                    return;

                case TrafficLight.TrafficLightState.Green:
                    onRedTrafficLight = false;
                    break;
                case TrafficLight.TrafficLightState.Red:
                    onRedTrafficLight = true;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region States

        private void OnParked()
        {
            currentSpeed = 0;
            currentGear = 0;
        }

        private void OnIdle()
        {
            currentBrakePower = brakePower;
            currentSpeed = 0;
            currentGear = 0;
        }

        private void OnBrake()
        {
            Brake(currentBrakePower);

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
                currentGear = 0;
                currentState = CarState.Idle;
            }
 
            if (playerControlled)
            {
                switch (movementDirection)
                {
                    case MovementDirection.Forward:
                        ForwardMove();
                        break;

                    case MovementDirection.Backward:
                        BackwardMove();
                        break;

                    default:
                        break;
                }
            }     
        }

        private void OnDriveForward()
        {
            float currentMax = maxForwardSpeed * gears[currentGear].speedOffset;
            Accerlate(currentMax);
            ForwardMove();   
        }

        private void OnDriveBackward()
        {
            float currentMax = maxBackwardSpeed;
            Accerlate(currentMax);
            BackwardMove();
        }

        private void OnRotation()
        {
            if (npcControlled)
            {
                if (currentSpeed > turnSpeed)
                {
                    Brake(brakePower * 3);
                    //return;
                }
                else
                {
                    NpcRotate(dirVec);
                }

                ForwardMove();
            }
        }
        
        #endregion

        #region Main

        private void Accerlate(float currentmaxSpeed)
        {
            currentSpeed += Time.deltaTime * accerlation;

            if (currentSpeed >= currentmaxSpeed)
            {
                currentSpeed = currentmaxSpeed;

                if (movementDirection != MovementDirection.Forward) return;

                if (playerControlled && currentGear < gears.Length - 1)
                {
                    IncreaseGear();
                    return;
                }

                if (npcControlled && currentGear < maxGear)
                {
                    IncreaseGear();
                }
            }
        }

        private void IncreaseGear()
        {
            currentGear++;

            if (currentGear >= gears.Length)
            {
                currentGear = gears.Length - 1;
            }
        }

        private void DecraseGear()
        {         
            if (currentGear > 0)
            {
                currentGear--;
            }
        }

        private void Brake(float brakePower)
        {
            currentSpeed -= Time.deltaTime * brakePower;

            //Debug.Log("BRAKE -> " + currentSpeed);

            float currentMax = maxForwardSpeed * gears[currentGear].speedOffset;
            if (movementDirection == MovementDirection.Forward && currentSpeed < currentMax)
            {              
                DecraseGear();
            }


        }

        private void ForwardMove()
        {
            //rb.velocity = currentSpeed * transform.forward;          
            transform.Translate(Time.deltaTime * currentSpeed * transform.forward, Space.World);
            movementDirection = MovementDirection.Forward;
        }

        private void BackwardMove()
        {
            //rb.velocity = currentSpeed * -transform.forward;
            transform.Translate(Time.deltaTime * currentSpeed * -transform.forward, Space.World);
            movementDirection = MovementDirection.Backward;
        }
        
        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
            tilePosLast = tilePos;

            Vector3 rayOrigin = groundCheck.position;
            Vector3 rayDirection = Vector3.down;
            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 0.2f, groundLayer))
            {
                //SlopeCheck(hit);
                var tile = hit.collider.gameObject.GetComponent<Tile>();
                tilePos = tile.transform.position;
                
                roadDirections = tile.GetRoadDirections();
                surfaceType = tile.GetSurfaceType();
            }
        }

        #endregion

        #region Player

        protected void PlayerInputCheck()
        {
            if (!isActive) return;

            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            if (input.y > 0)
            {
                if (currentState == CarState.DriveBackward)
                {
                    currentState = CarState.Brake;
                    Brake(brakePower * 1.5f);
                    if (currentSpeed < 0.1f) currentState = CarState.DriveForward;
                }
                 currentState = CarState.DriveForward;
            }
            else if (input.y < 0)
            {
                if (currentState == CarState.DriveForward)
                {
                    currentState= CarState.Brake;
                    if (currentSpeed < 0.1f) currentState = CarState.DriveBackward;

                }
                currentState = CarState.DriveBackward;

            }
            else if(currentSpeed >= 0.1f)
            {
                currentBrakePower = brakePower * currentSpeed;
                currentState = CarState.Brake;
            }
            else
            {
                currentState = CarState.Idle;
            }
        }

        protected void Rotation()
        {
            if (!onGround) return;
            if (currentSpeed < 0.65f) return;

            var rotationY = 0f;

            if (currentState == CarState.DriveForward)
            {
                rotationY = input.x * turnRatio;
            }
            else if(currentState == CarState.DriveBackward)
            {
                rotationY = (input.x * -1) * turnRatio;
            }
      
            rotationY *= Time.deltaTime;
            transform.rotation *= Quaternion.AngleAxis(rotationY, transform.up);

        }

        #endregion

        #region NPC

        protected void NpcControl()
        {
            pathIsBlocked = IsPathBlocked();

            if (pathIsBlocked || onRedTrafficLight)
            {
                currentState = CarState.Brake;
                return;
            }

            if (tilePosLast != tilePos || dirVecLast == Vector3.zero)
            {
                if (roadDirections.Length == 0)
                {
                    dirVec = GetRoadDirectionVector(RoadDirection.None);
                    lastRoadDirection = RoadDirection.None;
                    return;
                }

                if (roadDirections.Length == 1)
                {
                    dirVec = GetRoadDirectionVector(roadDirections[0]);
                    lastRoadDirection = roadDirections[0];
                }
                else
                {
                    int z = Util.RandomIntNumber(0, 100);

                    if (z > 66)
                    {
                        int r = Util.RandomIntNumber(0, roadDirections.Length);
                        dirVec = GetRoadDirectionVector(roadDirections[r]);
                        lastRoadDirection = roadDirections[r];
                    }
                    else
                    {
                        dirVec = Vector3.zero;

                        for (int i = 0; i < roadDirections.Length; i++)
                        {
                            if (roadDirections[i] == lastRoadDirection)
                            {
                                dirVec = GetRoadDirectionVector(lastRoadDirection);
                                break;
                            }
                        }

                        if (dirVec == Vector3.zero)
                        {
                            int r = Util.RandomIntNumber(0, roadDirections.Length);
                            dirVec = GetRoadDirectionVector(roadDirections[r]);
                            lastRoadDirection = roadDirections[r];
                        }
                    }
                }

                dirVecLast = dirVec;              
            }
                        
            if (dirVec == Vector3.zero)
            {
                onTurn = false;
                currentState = CarState.Brake;
                return;
            }
            else if (transform.forward.normalized != dirVec)
            {
                currentState = CarState.Rotate;
                return;
            }

            currentState = CarState.DriveForward;           
        }

        protected Vector3 GetRoadDirectionVector(RoadDirection direction)
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

            return vec;
        }

        protected void NpcRotate(Vector3 vec)
        {
            float step = Time.deltaTime * turnRatioNpcNormal;
            direction = Vector3.RotateTowards(transform.forward.normalized, vec, step, 0f);
            transform.rotation = Quaternion.LookRotation(direction);

            if (transform.forward.normalized == vec)
            {
                currentState = CarState.DriveForward;
            }
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

        public LayerMask carLayer;
        public Collider[] pathBlocker;
        public List<Collider> relevantColliders = new List<Collider>();
        private bool IsPathBlocked()
        {
            //Vector3 rayOrigin = obstacleCheckOrigin.position;         
            //Vector3 rayDirection = transform.forward;
            //Ray ray= new Ray(rayOriginCenter, rayDirection);
            //RaycastHit hit;

            bool blocked = false;
            relevantColliders.Clear();
            //pathBlocker = Physics.OverlapBox(obstacleCheckOrigin.position, obstacleCheckBoxHalfExtents, transform.rotation, ~ carLayer);
            pathBlocker = Physics.OverlapBox(obstacleCheckOrigin.position, obstacleCheckBoxHalfExtents, transform.rotation);
          
            for (int i = 0; i < pathBlocker.Length; i++)
            {
                if (pathBlocker[i].gameObject.layer == LayerMask.NameToLayer("Trigger")) continue;
                if (pathBlocker[i].gameObject.layer == LayerMask.NameToLayer("TrafficLightTrigger")) continue;

                relevantColliders.Add(pathBlocker[i]);
            }

            //Debug.Log(relevantColliders.Count);
            if (relevantColliders.Count != 0)
            {
                blocked = true;
            }

            /*if (Physics.BoxCast(obstacleCheckOrigin.position, obstacleCheckRayOffset, rayDirection, out hit, transform.rotation, obstacleCheckDistance))
            {
                if (hit.collider.gameObject.name != gameObject.name)
                {
                    blocked = true;
                }
            }*/

            color = blocked ? new Color(1f, 0f, 0f, 0.65f) : new Color(0f, 1f, 0f, 0.65f);
            return blocked;
        }

        #endregion      
    }

    [System.Serializable]
    public struct Gear
    {
        public Gear(float sp)
        {
            speedOffset = sp;
        }
        public float speedOffset;
    }
}

