/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;

namespace ProjectGTA2_Unity.Characters
{
    public class Player : Character
    {
        [SerializeField] private WeaponBelt weaponBelt;
        [SerializeField] private AudioEventList audioEvents;

        [SerializeField] private float walk_step_Distance = 1f;
        [SerializeField] private float sprint_step_Distance = 0.5f;
        [SerializeField] private float crouch_step_Distance = 1.5f;
        [SerializeField] private Transform groundRaycast;
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float CarEnterDistance = 2f;

        private Vector3 move = Vector3.zero;
        private Vector2 input = Vector2.zero;
        private Vector3 rotation = Vector3.zero;

        private float accumulated_Distance = 1f;
        private float step_Distance = 0f;

        private TileType tileType;
        private SurfaceType surfaceType;

        private bool onSlope;
        RaycastHit hit;



        #region UnityFunctions

        private void Update()
        {
            GroundCheck();            
            Jump();
            PlayRunAnim();
            PlayFootStepSound(input);


            if (!onGround)
            {
                rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                weaponBelt.WeaponAttack();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckNearbyCarsToEnter();
            }

        }

        private void FixedUpdate()
        {
            if (!onGround) return;
            ForwardBackwardMovement();
            Rotation();
        }

        #endregion

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();
            Pickup.PickupCollected += PickupColleted;
            step_Distance = sprint_step_Distance;

        }

        #region Movement

        private void ForwardBackwardMovement()
        {
            move = Vector3.zero;
            input.y = Input.GetAxis("Vertical");

            //input.y *= -1;

            move = transform.forward * input.y * walkSpeed;
            //move *= Time.deltaTime;

            rb.velocity = move;

            //rb.useGravity = !onSlope;

        }

        private void Rotation()
        {
            if (!onGround) return;

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
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
                if(tile == null) return;
                surfaceType = tile.GetSurfaceType();
            }
        }

        private void SlopeCheck(RaycastHit hit)
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            //Debug.Log(angle);
            onSlope = angle < maxSlopeAngle && angle != 0;
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
                        audioEvents.PlayAudioEventOneShot("FootstepsConcrete");
                        break;

                    case SurfaceType.Grass:
                        audioEvents.PlayAudioEventOneShot("FootstepsGrass");
                        break;

                    case SurfaceType.Metal:
                        audioEvents.PlayAudioEventOneShot("FootstepsSolid");
                        break;

                    case SurfaceType.Wood:
                        audioEvents.PlayAudioEventOneShot("FootstepsWood");
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

        private void CheckNearbyCarsToEnter()
        {
            Collider[] carColliders = Physics.OverlapSphere(transform.position, CarEnterDistance, 1 << 8 );
           
            if (carColliders.Length < 1)
            {
                Debug.Log("NO Cars in Range");
                return;
            }

            Debug.Log(carColliders.Length);

            int index = 0;
            float distance = 999f;

            for (int i = 0; i < carColliders.Length; i++)
            {
                float lastDistance = distance;
                distance = Vector3.Distance(transform.position, carColliders[i].transform.position);

                if (distance < lastDistance)
                {
                    index = i;
                }
            }

            Debug.Log(index);
            Debug.Log(carColliders[index].gameObject.name);

            List<Component> results = new List<Component>();

            foreach (var item in results)
            {
                Debug.Log(item.name);
            }

            var car = carColliders[index].gameObject.GetComponent<Car>();
             
            if (car != null)
            {
                car.PlayerEnters(this);
            }
            else
            {
                Debug.Log("NO CAR");
            }
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
            weaponBelt.AddAmmo(pickupType.ToString(), amount);                
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CarEnterDistance);
        }

    }
}

