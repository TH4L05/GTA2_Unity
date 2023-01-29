/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity.Cars
{
    [System.Serializable]
    public struct Gear
    {
        public Gear(float sp)
        {
            speedOffset = sp;
        }
        public float speedOffset;
    }



    [RequireComponent(typeof(Rigidbody))]
    public class CarMovement : MonoBehaviour
    {
        public enum MovementDirection
        {
            Forward,
            Backward
        }

        #region SerializedFields

        //[SerializeField] private float mass;
        [SerializeField] private float accerlation;
        [SerializeField] private float brakePower;
        [SerializeField] private float turnRatio;
        [SerializeField] private float maxForwardSpeed;
        [SerializeField] private float maxBackwardSpeed;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected float gravityFactor = 15f;
        [SerializeField] protected Gear[] gears = {new Gear(0.33f), new Gear(0.55f), new Gear(1f) };

        #endregion

        #region PrivateFields

        private Rigidbody rb;
        [SerializeField] private bool isActive = false;
        private float currentSpeed;
        private int currentGear = 0;
        private Vector2 input = Vector2.zero;
        private MovementDirection movementDirection;
        [SerializeField] private bool onGround;
        [SerializeField] private bool isPlayerControlled;
        [SerializeField] private RoadDirection[] roadDirections;
        [SerializeField] private Vector3 destination;
        [SerializeField] private SurfaceType surfaceType;

        #endregion

        #region PublicFields

        public (float, float, float, MovementDirection) MovementValues => (currentSpeed, maxForwardSpeed, maxBackwardSpeed, movementDirection);
        
        #endregion

        #region UnityFunctions

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            GroundCheck();
           
            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

            if (!isActive) return;

            UpdatePosition();
            Rotation();

            if (!isPlayerControlled)
            {
                NpcControl();
                return;
            }
            InputCheck();           
        }

        /*private void FixedUpdate()
        {
            if (!isActive) return;
            //UpdatePosition();
        }*/

        #endregion

        public void SetActive(bool active, bool playerControlled)
        {
            isActive = active;
            isPlayerControlled = playerControlled;
        }

        private void NpcControl()
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

        private void CheckRoadDirections(RoadDirection direction)
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

            if(vec == Vector3.zero)
            {
                Brake(brakePower);
                return;
            }

            if (transform.forward.normalized == vec)
            {
                Debug.Log("forwardDirection");
                Accerlate();
            }
            else
            {
                NpcRotate(vec);
            }
        }

        private void NpcRotate(Vector3 vec)
        {
            float step = Time.deltaTime * 2.2f;
            Vector3 direction = Vector3.RotateTowards(transform.forward.normalized, vec, step, 0f);
            transform.rotation = Quaternion.LookRotation(direction);
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
        }

        private void InputCheck()
        {
            if (!isPlayerControlled) return;
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

        private void Accerlate()
        {
            currentSpeed += Time.deltaTime * accerlation;
 
            switch (movementDirection)
            {
                case MovementDirection.Forward:

                    float currentMax = maxForwardSpeed * gears[currentGear].speedOffset;

                    if(isPlayerControlled && currentGear< gears.Length - 1 && currentSpeed >= currentMax)
                    {
                        IncreaseGear();
                    }
                    else if(currentSpeed >= currentMax)
                    {
                        currentSpeed = currentMax;
                    }

                    /*if (currentSpeed >= maxForwardSpeed)
                    {
                        currentSpeed = maxForwardSpeed;
                    }*/
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

        private void IncreaseGear()
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

        private void Rotation()
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

        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
          
            Vector3 rayOrigin = groundCheck.position;
            Vector3 rayDirection = Vector3.down;
            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 0.2f, groundLayer))
            {
                //SlopeCheck(hit);

                var tile = hit.collider.gameObject.GetComponent<Tile>();
                roadDirections = tile.GetRoadDirections();
                //if (tile == null) return;
                surfaceType = tile.GetSurfaceType();
            }
        }
    }
}

