/// <author>Thoams Krahl</author>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectGTA2_Unity.Input
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private bool enableMovementInputs;
        [SerializeField] private bool enableInteractionInputs;

        private GameControls gameControls;
        private List<InputAction> inputActionsMovement;
        private List<InputAction> inputActionsInteractive;

        public static InputHandler Instance;
        public Vector2 MovementAxisInputValue { get; private set; }
        public bool JumpInputPressed { get; private set; }
        public bool SprintInputPressed { get; private set; }
        public bool ShootInputPressed { get; private set; }
        public bool NextWeaponInputPressed { get; set; }
        public bool PreviousWeaponPressed { get; set; }
        public bool InteractInputPressed { get; private set; }


        #region UnityFunctions

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            Intitialize();
        }

        private void Start()
        {
            SetInputActions();
            if (enableMovementInputs) EnableDisableInputActions(true, inputActionsMovement);
            if (enableInteractionInputs) EnableDisableInputActions(true, inputActionsInteractive);
        }

        private void OnDestroy()
        {
            EnableDisableInputActions(false, inputActionsMovement);
            EnableDisableInputActions(false, inputActionsInteractive);
        }

        #endregion

        #region Setup

        private void Intitialize()
        {
            gameControls = new GameControls();
            inputActionsMovement = new List<InputAction>();
            inputActionsInteractive = new List<InputAction>();
        }

        private void SetInputActions()
        {
            var inputMovement = gameControls.Movement;
            var inputInteraction = gameControls.InterAction;

            inputMovement.MovementBase.performed += ReadBaseMovementValue;
            inputActionsMovement.Add(inputMovement.MovementBase);
   
            inputMovement.Jump.performed += JumpInputPerformed;
            inputMovement.Jump.canceled += JumpInputCanceled;
            inputActionsMovement.Add(inputMovement.Jump);


            //------------------------------------------------------------

            inputInteraction.Interact.performed += InteractInputPerformed;
            inputInteraction.Interact.canceled += InteractInputCanceled;
            inputActionsMovement.Add(inputInteraction.Interact);

            inputInteraction.Shoot.performed += ShootInputPerformed;
            inputInteraction.Shoot.canceled += ShootInputCanceled;
            inputActionsMovement.Add(inputInteraction.Shoot);

            inputInteraction.WeaponNext.performed += NextWeaponInputPerformed;
            inputInteraction.WeaponNext.canceled += NextWeaponInputCanceled;
            inputActionsMovement.Add(inputInteraction.WeaponNext);

            inputInteraction.WeaponPrevious.performed += PreviousWeaponInputPerformed;
            inputInteraction.WeaponPrevious.canceled += PreviousWeaponInputCanceled;
            inputActionsMovement.Add(inputInteraction.WeaponPrevious);

        }

        public void EnableDisableInputActions(bool enable, List<InputAction> inputActionList)
        {
            if (enable)
            {
                foreach (InputAction inputAction in inputActionList)
                {
                    inputAction.Enable();
                }
            }
            else
            {
                foreach (InputAction inputAction in inputActionList)
                {
                    inputAction.Disable();
                }
            }
        }

        #endregion

        #region Input

        private void ReadBaseMovementValue(InputAction.CallbackContext context)
        {
            MovementAxisInputValue = context.ReadValue<Vector2>();
        }

        private void JumpInputPerformed(InputAction.CallbackContext context)
        {
            JumpInputPressed = context.ReadValueAsButton();
        }

        private void JumpInputCanceled(InputAction.CallbackContext context)
        {
            JumpInputPressed = context.ReadValueAsButton();
        }

        private void InteractInputPerformed(InputAction.CallbackContext context)
        {
            InteractInputPressed = context.ReadValueAsButton();
        }

        private void InteractInputCanceled(InputAction.CallbackContext context)
        {
            InteractInputPressed = context.ReadValueAsButton();
        }

        private void ShootInputPerformed(InputAction.CallbackContext context)
        {
            ShootInputPressed = context.ReadValueAsButton();
        }

        private void ShootInputCanceled(InputAction.CallbackContext context)
        {
            ShootInputPressed = context.ReadValueAsButton();
        }

        private void NextWeaponInputPerformed(InputAction.CallbackContext context)
        {
            NextWeaponInputPressed = context.ReadValueAsButton();
        }

        private void NextWeaponInputCanceled(InputAction.CallbackContext context)
        {
            NextWeaponInputPressed = context.ReadValueAsButton();
        }

        private void PreviousWeaponInputPerformed(InputAction.CallbackContext context)
        {
            PreviousWeaponPressed = context.ReadValueAsButton();
        }

        private void PreviousWeaponInputCanceled(InputAction.CallbackContext context)
        {
            PreviousWeaponPressed = context.ReadValueAsButton();
        }

        #endregion
    }
}

