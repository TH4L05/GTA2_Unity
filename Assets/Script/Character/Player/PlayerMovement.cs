/// <author>Thoams Krahl</author>

using UnityEngine;
using Unity.Netcode;
using ProjectGTA2_Unity.Tiles;
using ProjectGTA2_Unity.Characters.Data;
using ProjectGTA2_Unity.Cars;
using ProjectGTA2_Unity.Audio;
using ProjectGTA2_Unity.Weapons;
using ProjectGTA2_Unity.Input;

namespace ProjectGTA2_Unity.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour
    {
        #region SerializedFields

        [SerializeField] private ArmouryPlayer armoury;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform groundRaycast;
        [SerializeField] private AudioEventList audioEvents;

        #endregion

        #region PrivateFields

        private Rigidbody rb;
        private CharacterData characterData;
        private SurfaceType surfaceType;
        private RaycastHit hit;

        private Vector3 move = Vector3.zero;
        private Vector3 moveSlope = Vector3.zero;
        private Vector2 input = Vector2.zero;
        private float accumulated_Distance = 1f;
        private float step_Distance = 0f;

        private bool jump;
        private bool onGround;     
        private bool onSlope;
        private bool onJump;
        private bool onCar;

        private float startFallHeight;
        private bool isFalling => !onGround && rb.velocity.y < 0;
        private bool wasFalling;

        #endregion

        #region UnityFunctions

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            step_Distance = characterData.SprintStepDistance;
        }

        private void Update()
        {
            //if (!IsOwner) return;
            bool wasGrounded = onGround;

            ReadInputValues();
            GroundCheck();
            if(!wasFalling && isFalling) startFallHeight = transform.position.y;
            
            FallDamageCheck(wasGrounded);
            wasFalling = isFalling;

            Jump();
            PlayRunAnim();
            PlayFootStepSound(input);          
            moveSlope = Vector3.ProjectOnPlane(move, hit.normal);
        }

        private void FixedUpdate()
        {
            //if (!IsOwner) return;
            //rb.useGravity = onGround;
            if (!onGround)
            {
                //if (onCar)
                //{
                //    Vector3 v = rb.velocity;
                //    rb.AddForce(rb.velocity, ForceMode.Impulse);
                //    rb.velocity = v;
                //}
                //else
                //{
                    rb.AddForce(new Vector3(0, -1, 0) * characterData.GravityFactor, ForceMode.Force);
                //}

                return;
            }


            ForwardBackwardMovement();
            Rotation();
        }

        #endregion

        #region Movement

        private void ReadInputValues()
        {
            input = InputHandler.Instance.MovementAxisInputValue;
            jump = InputHandler.Instance.JumpInputPressed;
        }

        private void ForwardBackwardMovement()
        {
            move = transform.forward * input.y * characterData.RunSpeed;
            //move *= Time.deltaTime;

            if (!onSlope)
            {
                rb.velocity = move;
            }
            else
            {
                rb.velocity = moveSlope;
            }                 
        }

        private void Rotation()
        {
            if (!onGround) return;

            var rotationY = input.x * characterData.RotationSensitivity;
            Vector3 rotation = new Vector3(0f, rotationY, 0f);
            rotation *= Time.deltaTime;

            transform.Rotate(rotation);
        }

        private void Jump()
        {
            if (jump && onGround)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce((transform.forward + Vector3.up) * characterData.JumpForce, ForceMode.Impulse);
                animator.SetBool("Jump", true);
                onGround = false;
                onJump = true;
            }
        }

        private void GroundCheck()
        {
            onGround = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
            animator.SetBool("OnGround", onGround);
            onCar = false;
         
            Vector3 rayOrigin = groundRaycast.position;
            Vector3 rayDirection = Vector3.down;
            Ray ray = new Ray(rayOrigin, rayDirection);

            if (Physics.Raycast(ray, out hit, 0.2f))
            {
                SlopeCheck(hit);

                var tile = hit.collider.gameObject.GetComponent<Tile>();
                if (tile != null)
                {
                    surfaceType = tile.GetSurfaceType();
                    //return;
                }

                /*var car = hit.collider.gameObject.GetComponent<Car>();
                if (car)
                {
                    onCar = true;
                }*/           
            }

            if (onJump && onGround)
            {
                onJump = false;
                animator.SetBool("Jump", false);
            }
        }

        private void SlopeCheck(RaycastHit hit)
        {
            onSlope = false;
            if (hit.normal != Vector3.up)
            {
                onSlope = true;
            }          
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
                        audioEvents.PlayAudioEventOneShotAttached("FootstepsConcrete", gameObject);
                        break;

                    case SurfaceType.Grass:
                        audioEvents.PlayAudioEventOneShotAttached("FootstepsGrass", gameObject);
                        break;

                    case SurfaceType.Metal:
                        audioEvents.PlayAudioEventOneShotAttached("FootstepsSolid", gameObject);
                        break;

                    case SurfaceType.Wood:
                        audioEvents.PlayAudioEventOneShotAttached("FootstepsWood", gameObject);
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
                if (armoury.GunEquipped)
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

        private void FallDamageCheck(bool wasGrounded)
        {
            float fallHeight = startFallHeight + transform.position.y;
            //Debug.Log(fallHeight);

            if (!wasGrounded && onGround)
            {           
                if ( fallHeight > characterData.MinFallHeight)
                {
                    Game.Instance.player.TakeDamage(fallHeight * 10f, DamageType.Normal, gameObject.tag);
                }               
            }
        }
    }
}

