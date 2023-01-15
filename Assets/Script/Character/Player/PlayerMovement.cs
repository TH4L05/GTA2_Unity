using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private WeaponBelt weaponBelt;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform groundRaycast;

        /*[SerializeField] private AudioEventList audioEvents;
        [SerializeField] private float walk_step_Distance = 1f;
        [SerializeField] private float sprint_step_Distance = 0.5f;
        [SerializeField] private float crouch_step_Distance = 1.5f;
        [SerializeField] private float maxSlopeAngle = 45f;

        [SerializeField] protected float walkSpeed;
        [SerializeField] protected float runSpeed;
        [SerializeField] protected float jumpForce;
        [SerializeField] protected float gravityFactor = 5f;
        [SerializeField] protected float rotationSensitivity = 5f;*/

        private Rigidbody rb;
        private Vector3 move = Vector3.zero;
        private Vector2 input = Vector2.zero;
        private Vector3 rotation = Vector3.zero;

        private float accumulated_Distance = 1f;
        private float step_Distance = 0f;

        private TileType tileType;
        private SurfaceType surfaceType;
        private bool onGround;
        private bool onSlope;
        private RaycastHit hit;
        private CharacterData characterData;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            step_Distance = characterData.SprintStepDistance;
        }

        private void Update()
        {
            GroundCheck();
            Jump();
            PlayRunAnim();
            PlayFootStepSound(input);


            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * characterData.GravityFactor, ForceMode.Acceleration);
            }
        }

        private void FixedUpdate()
        {
            if (!onGround) return;
            ForwardBackwardMovement();
            Rotation();
        }

        #region Movement

        private void ForwardBackwardMovement()
        {
            move = Vector3.zero;
            input.y = Input.GetAxis("Vertical");

            //input.y *= -1;

            move = transform.forward * input.y * characterData.RunSpeed;
            //move *= Time.deltaTime;

            rb.velocity = move;

            //rb.useGravity = !onSlope;

        }

        private void Rotation()
        {
            if (!onGround) return;

            input.x = Input.GetAxis("Horizontal");

            var rotationY = input.x * characterData.RotationSensitivity;

            rotation = new Vector3(0f, rotationY, 0f);
            rotation *= Time.deltaTime;

            transform.Rotate(rotation);
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && onGround)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * characterData.JumpForce, ForceMode.Impulse);
                onGround = false;
            }
        }

        private void GroundCheck()
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
        }

        private void SlopeCheck(RaycastHit hit)
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            //Debug.Log(angle);
            onSlope = angle < characterData.MaxSlopeAngle && angle != 0;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(move, hit.normal).normalized;
        }

        private void PlayFootStepSound(Vector2 input)
        {
            if (!onGround) return;

            if (input.y == 0)
            {
                accumulated_Distance = 0f;
                return;
            }

            accumulated_Distance += Time.deltaTime;

            if (accumulated_Distance > step_Distance)
            {
                accumulated_Distance = 0f;

                switch (surfaceType)
                {
                    case SurfaceType.Invalid:
                        break;

                    case SurfaceType.Normal:
                        characterData.AudioEvents.PlayAudioEventOneShot("FootstepsConcrete");
                        break;

                    case SurfaceType.Grass:
                        characterData.AudioEvents.PlayAudioEventOneShot("FootstepsGrass");
                        break;

                    case SurfaceType.Metal:
                        characterData.AudioEvents.PlayAudioEventOneShot("FootstepsSolid");
                        break;

                    case SurfaceType.Wood:
                        characterData.AudioEvents.PlayAudioEventOneShot("FootstepsWood");
                        break;

                    case SurfaceType.Electrified:
                        break;

                    case SurfaceType.ElectrifiedPlatform:
                        break;

                    default:
                        break;
                }


            }
        }

        #endregion

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

        public void SetCharacterData(CharacterData data)
        {
            characterData = data;
        }
    }
}

