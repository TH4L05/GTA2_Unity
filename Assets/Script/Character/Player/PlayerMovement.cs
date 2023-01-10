using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 5f;
        [SerializeField] private float rotationSensitivity  = 5f;
        [SerializeField] private float jumpforce  = 5f;
        [SerializeField] private float gravityFactor = 5f;
        [SerializeField] private Animator anim;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        private Vector3 move = Vector3.zero;
        private Vector2 input = Vector2.zero;
        private Vector3 rotation = Vector3.zero;
        private bool onGround = true; 

        void Start()
        {

        }

        private void Update()
        {
            GroundCheck();
            ForwardBackwardMovement();
            Rotation();
            Jump();

            if (move != Vector3.zero)
            {
                anim.SetBool("Run", true);
            }
            else
            {
                anim.SetBool("Run", false);
            }

            if(!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

        }

        public void StartSetup(float walkSpeed, float runSpeed)
        {
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
        }

        private void ForwardBackwardMovement()
        {
            move = Vector3.zero;
            input.y = Input.GetAxis("Vertical");

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
                rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
                

                onGround = false;
            }
        }

        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
            //return onGround;

            /*if (Physics.Raycast(transform.position, Vector3.down, 0.1f + 0.01f))
            {
                Debug.Log("OnGround");
                onGround = true;   
            }
            else
            {
                onGround = false;
                //rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }*/

            
        }


    }
}

