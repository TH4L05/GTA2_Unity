using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class Player : Character
    {
        [SerializeField] private WeaponBelt weaponBelt;
        [SerializeField] private AudioEventList audioEvents;

        private Vector3 move = Vector3.zero;
        private Vector2 input = Vector2.zero;
        private Vector3 rotation = Vector3.zero;


        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();
            Pickup.PickupCollected += PickupColleted;

        }

        private void Update()
        {
            GroundCheck();
            ForwardBackwardMovement();
            Rotation();
            Jump();

            PlayRunAnim();



            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

        }

        private void ForwardBackwardMovement()
        {
            move = Vector3.zero;
            input.y = UnityEngine.Input.GetAxis("Vertical");

            //input.y *= -1;

            move = transform.forward * input.y * walkSpeed;
            //move *= Time.deltaTime;

            //transform.Translate(move,Space.World);
            //controller.Move(move);
            rb.velocity = move;
            //rb.MovePosition(move);

        }

        private void Rotation()
        {
            input.x = Input.GetAxis("Horizontal");

            var rotationY = input.x * rotationSensitivity;

            rotation = new Vector3(0f, rotationY, 0f);
            rotation *= Time.deltaTime;

            transform.Rotate(rotation);
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && onGround)
            {
                Debug.Log("JUMP");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);


                onGround = false;
            }
        }

        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        }

        #region Animation

        private void PlayRunAnim()
        {
            if (move != Vector3.zero && onGround)
            {
                animator.SetBool("GunEquiped", false);
                if (weaponBelt.GunEquipped)
                {
                    animator.SetBool("GunEquiped", true);
                }
                animator.SetBool("Run", true); 
            }
            else
            {
                animator.SetBool("Run", false);
            }
        }

        #endregion


        private void PickupColleted(PickupType pickupType, int amount)
        {
            Debug.Log("PickUpCollected");

            switch (pickupType)
            {
                case PickupType.Invalid:
                    break;


                case PickupType.Health:
                    break;


                case PickupType.Pistol:
                    weaponBelt.AddAmmo(pickupType.ToString(),amount);
                    break;

                case PickupType.Machinegun:
                    weaponBelt.AddAmmo(pickupType.ToString(), amount);
                    break;


                default:
                    break;
            }
        }

    }
}

