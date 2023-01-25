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

        //[SerializeField] private float mass;
        [SerializeField] private float accerlation;
        [SerializeField] private float brakePower;
        [SerializeField] private float turnRatio;
        [SerializeField] private float maxForwardSpeed;
        [SerializeField] private float maxBackwardSpeed;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected float gravityFactor = 15f;

        #endregion

        #region PrivateFields

        private Rigidbody rb;
        private bool isActive = false;
        private float currentSpeed;
        private Vector2 input = Vector2.zero;
        private MovementDirection movementDirection;
        private bool onGround;

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
            UpdatePosition();
            Rotation();

            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

            if (!isActive) return;
            InputCheck();           
        }

        /*private void FixedUpdate()
        {
            if (!isActive) return;
            //UpdatePosition();
        }*/

        #endregion

        public void SetActive(bool active)
        {
            isActive = active;
        }

        private void InputCheck()
        {
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
                    if (currentSpeed >= maxForwardSpeed)
                    {
                        currentSpeed = maxForwardSpeed;
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

        private void Brake(float brakePower)
        {
            currentSpeed -= Time.deltaTime * brakePower;

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
            }
        }

        private void EngineBreak()
        {
            currentSpeed -= Time.deltaTime * brakePower * 2;

            if (currentSpeed <= 0.1f)
            {
                currentSpeed = 0;
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
                //if (tile == null) return;
                //surfaceType = tile.GetSurfaceType();
            }
        }
    }
}

