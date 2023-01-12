using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectGTA2_Unity.Car;

namespace ProjectGTA2_Unity
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarMovement : MonoBehaviour
    {
        private enum MovementDirection
        {
            Forward,
            Backward
        }

        [SerializeField] private Rigidbody rb;

        [SerializeField] private float mass;
        [SerializeField] private float accerlation;
        [SerializeField] private float brakePower;
        [SerializeField] private float turnRatio;
        [SerializeField] private float maxForwardSpeed;
        [SerializeField] private float maxBackwardSpeed;

        private bool isActive = false;
        private float currentSpeed;
        private Vector3 move = Vector3.zero;
        private Vector2 input = Vector2.zero;
        private Vector3 rotation = Vector3.zero;

        private MovementDirection movementDirection;
        private MovementDirection lastMovementDirection;


        #region UnityFunctions

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!isActive) return;
            InputCheck();            
        }

        private void FixedUpdate()
        {
            if (!isActive) return;
            UpdatePosition();
            Rotation();
        }

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
            currentSpeed -= Time.deltaTime * brakePower / 2;

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
            move = currentSpeed * transform.forward;
            rb.velocity = move;
        }

        private void BackwardMove()
        {
            move = currentSpeed * -transform.forward;
            rb.velocity = move;
        }

        private void Rotation()
        {
            //if (!onGround) return;
            if (currentSpeed < 0.65f) return;

            var rotationY = input.x * turnRatio - currentSpeed;

            rotation = new Vector3(0f, rotationY, 0f);
            rotation *= Time.deltaTime;

            transform.Rotate(rotation);
        }

        /*private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
            animator.SetBool("OnGround", onGround);

            Vector3 rayOrigin = groundRaycast.position;
            Vector3 rayDirection = Vector3.down;
            Ray ray = new Ray(rayOrigin, rayDirection);

            if (Physics.Raycast(ray, out hit, 0.2f, groundLayer))
            {
                SlopeCheck(hit);

                var tile = hit.collider.gameObject.GetComponent<Tile>();
                if (tile == null) return;
                surfaceType = tile.GetSurfaceType();
            }
        }*/
    }
}

