/// <author>Thoams Krahl</author>

using UnityEngine;

namespace ProjectGTA2_Unity.Characters.Data
{
    [CreateAssetMenu(fileName = "newCharacterData", menuName = "Data/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] protected float walkSpeed = 2f;
        [SerializeField] protected float runSpeed = 4f;
        [SerializeField] protected float jumpForce = 10f;
        [SerializeField] protected float rotationSensitivity = 285f;
        [SerializeField] protected float gravityFactor = 15f;
        [SerializeField] protected float stoppingDistance = 1f;
        [SerializeField] private float walkStepDistance = 0.5f;
        [SerializeField] private float sprintStepDistance = 0.25f;
        [SerializeField] private float crouchStepDistance = 1.0f;
        [SerializeField] private float minFallHeight = 2f;

        public float Walkspeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float JumpForce => jumpForce;
        public float RotationSensitivity => rotationSensitivity;
        public float GravityFactor => gravityFactor;
        public float StoppingDistance => stoppingDistance;
        public float WalkStepDistance => walkStepDistance;
        public float SprintStepDistance => sprintStepDistance;
        public float CrouchStepDistance => crouchStepDistance;
        public float MinFallHeight => minFallHeight;
    }
}

