/// <author>Thoams Krahl</author>

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

        #region SerializedFields

        [Header("Base")]
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;

        [Header("Values")]
        [SerializeField] protected float accerlation;
        [SerializeField] protected float brakePower;
        [SerializeField] protected float turnRatio;
        [SerializeField] protected float maxForwardSpeed;
        [SerializeField] protected float maxBackwardSpeed;    
        [SerializeField] protected float gravityFactor = 15f;
        [SerializeField] protected Gear[] gears = {new Gear(0.33f), new Gear(0.55f), new Gear(1f) };

        [Header("NPC")]
        [SerializeField] protected float turnRatioNpcNormal = 2.2f;
        [SerializeField, Range(0.1f, 2.0f)] private float obstacleCheckDistance = 0.55f;
        [SerializeField] private Vector3 obstacleCheckRayOffset = new Vector3(0f, 0.2f, 0.5f);
        public bool pathIsBlocked;
        public Color color;
        public Transform obstacleCheckOrigin;

        #endregion

        #region PrivateFields

        protected Rigidbody rb;
        [SerializeField] protected bool isActive = false;
        protected float currentSpeed;
        protected int currentGear = 0;        
        protected MovementDirection movementDirection;
        [SerializeField] protected bool onGround;
        [SerializeField] protected RoadDirection[] roadDirections;      
        [SerializeField] protected SurfaceType surfaceType;

        [SerializeField] protected bool playerControlled;
        [SerializeField] protected bool npcControlled;
        protected Vector2 input = Vector2.zero;

        Vector3 vec = Vector3.zero;
        Vector3 vecLast;

        Vector3 tilePos;
        Vector3 tileLast;

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
            Gizmos.DrawCube(Vector3.zero, obstacleCheckRayOffset);
        }

        #endregion

        #region Setup

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

        }

        private void Initialize()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnUpdate()
        {
            GroundCheck();

            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

            if (!isActive) return;
            UpdatePosition();  
            
            if (playerControlled)
            {
                Rotation();
                InputCheck();
                return;
            }

            if(npcControlled)
            {
                pathIsBlocked = IsPathBlocked();
                if (pathIsBlocked)
                {
                    Brake(brakePower);
                    return;
                }
                NpcControl();
            }
        }

        #endregion

        #region Main

        private void Accerlate()
        {
            currentSpeed += Time.deltaTime * accerlation;
 
            switch (movementDirection)
            {
                case MovementDirection.Forward:

                    float currentMax = maxForwardSpeed * gears[currentGear].speedOffset;

                    if(currentSpeed >= currentMax)
                    {
                        currentSpeed = currentMax;
                    }
                    break;

                case MovementDirection.Backward:
                    if (currentSpeed >= maxBackwardSpeed)
                    {
                        currentSpeed = maxBackwardSpeed;
                    }
                    break;

                default:
                    break;
            }
        }

        protected void IncreaseGear()
        {
            currentGear++;
            
            if (currentGear >= gears.Length)
            {
                currentGear = gears.Length - 1;
            }
        }

        private void Brake(float brakePower)
        {
            currentSpeed -= Time.deltaTime * brakePower;

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
                currentGear = 0;
            }
        }

        private void EngineBreak()
        {
            currentSpeed -= Time.deltaTime * brakePower * 2;

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
                currentGear = 0;
            }
        }

        private void UpdatePosition()
        {
            if (currentSpeed != 0)
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

        private void ForwardMove()
        {
            //rb.velocity = currentSpeed * transform.forward;          
            transform.Translate(Time.deltaTime * currentSpeed * transform.forward, Space.World);
        }

        private void BackwardMove()
        {
            //rb.velocity = currentSpeed * -transform.forward;
            transform.Translate(Time.deltaTime * currentSpeed * -transform.forward, Space.World);
        }
        
        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
            tileLast = tilePos;

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

        protected void InputCheck()
        {
            if (!isActive) return;
            //move = Vector3.zero;

            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            if (input.y > 0)
            {
                if (movementDirection == MovementDirection.Backward)
                {
                    Brake(brakePower * 1.5f);
                    if (currentSpeed < 0.1f) movementDirection = MovementDirection.Forward;
                }
                else
                {
                    Accerlate();
                }
            }
            else if (input.y < 0)
            {
                if (movementDirection == MovementDirection.Forward)
                {
                    Brake(brakePower);
                    if (currentSpeed < 0.1f) movementDirection = MovementDirection.Backward;

                }
                else
                {
                    Accerlate();
                }
            }
            else
            {
                EngineBreak();
            }
        }

        protected void Rotation()
        {
            //if (!onGround) return;
            if (currentSpeed < 0.65f) return;

            var rotationY = 0f;

            switch (movementDirection)
            {
                case MovementDirection.Forward:
                    rotationY = input.x * turnRatio;
                    break;

                case MovementDirection.Backward:
                    rotationY = (input.x * -1) * turnRatio;
                    break;

                default:
                    break;
            }

            //rotation = new Vector3(0f, rotationY, 0f);
            //rotation *= Time.deltaTime;
            rotationY *= Time.deltaTime;

            //transform.Rotate(rotation);
            transform.rotation *= Quaternion.AngleAxis(rotationY, transform.up);

        }

        #endregion

        #region NPC

        protected void NpcControl()
        {
            if (tileLast != tilePos || vecLast == Vector3.zero)
            {
                if (roadDirections.Length == 0)
                {
                    vec = CheckRoadDirections(RoadDirection.None);
                    return;
                }

                if (roadDirections.Length == 1)
                {
                    vec = CheckRoadDirections(roadDirections[0]);
                }
                else
                {
                    int r = Util.RandomIntNumber(0, roadDirections.Length);
                    vec = CheckRoadDirections(roadDirections[r]);
                }
                vecLast = vec;
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

        protected Vector3 CheckRoadDirections(RoadDirection direction)
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

