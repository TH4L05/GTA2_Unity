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

        #endregion

        #region Setup

        public virtual void SetActive(bool active)
        {
            isActive = active;
        }

        protected virtual void Initialize()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected virtual void OnUpdate()
        {
            GroundCheck();

            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

            if (!isActive) return;
            UpdatePosition();                 
        }

        #endregion

        protected virtual void Accerlate()
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

        protected virtual void Brake(float brakePower)
        {
            currentSpeed -= Time.deltaTime * brakePower;

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
                currentGear = 0;
            }
        }

        protected virtual void EngineBreak()
        {
            currentSpeed -= Time.deltaTime * brakePower * 2;

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
                currentGear = 0;
            }
        }

        protected virtual void UpdatePosition()
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

        protected virtual void ForwardMove()
        {
            //rb.velocity = currentSpeed * transform.forward;          
            transform.Translate(Time.deltaTime * currentSpeed * transform.forward, Space.World);
        }

        protected virtual void BackwardMove()
        {
            //rb.velocity = currentSpeed * -transform.forward;
            transform.Translate(Time.deltaTime * currentSpeed * -transform.forward, Space.World);
        }
        
        protected virtual void GroundCheck()
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
                surfaceType = tile.GetSurfaceType();
            }
        }
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

