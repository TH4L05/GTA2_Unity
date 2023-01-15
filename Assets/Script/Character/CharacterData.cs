using ProjectGTA2_Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="newCharacterData", menuName ="Data/CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField] protected float walkSpeed = 2f;
    [SerializeField] protected float runSpeed = 4f;
    [SerializeField] protected float jumpForce = 10f;
    [SerializeField] protected float rotationSensitivity = 285f;
    [SerializeField] protected float gravityFactor = 15f;
    [SerializeField] private float walkStepDistance = 0.5f;
    [SerializeField] private float sprintStepDistance = 0.25f;
    [SerializeField] private float crouchStepDistance = 1.0f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private AudioEventList audioEvents;

    public float Walkspeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float RotationSensitivity => rotationSensitivity;
    public float GravityFactor => gravityFactor;
    public float WalkStepDistance => walkStepDistance;
    public float SprintStepDistance => sprintStepDistance;
    public float CrouchStepDistance => crouchStepDistance;
    public float MaxSlopeAngle => maxSlopeAngle;
    public AudioEventList AudioEvents => audioEvents;
}
