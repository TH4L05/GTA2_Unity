using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Cars
{
    public class CarMovementPlayer : CarMovement
    {
        protected Vector2 input = Vector2.zero;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            Rotation();
            InputCheck();
        }

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

        protected override void Accerlate()
        {
            currentSpeed += Time.deltaTime * accerlation;

            switch (movementDirection)
            {
                case MovementDirection.Forward:

                    float currentMax = maxForwardSpeed * gears[currentGear].speedOffset;

                    if (currentGear < gears.Length - 1 && currentSpeed >= currentMax)
                    {
                        IncreaseGear();
                    }
                    else if (currentSpeed >= currentMax)
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
    }


}

