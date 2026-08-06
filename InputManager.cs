using UnityEngine;
using System;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    //Jumping
    public event EventHandler OnPlayerJumpPerformed;
    
    //Crouching
    public event EventHandler OnPlayerCrouchPerformed;
    public event EventHandler OnPlayerCrouchCanceled;

    //Sprinting
    public event EventHandler OnPlayerSprintPerformed;
    
    
    //Pause game
    public event EventHandler OnPlayerPausedGame;

    //Interact
    public event EventHandler OnPlayerInteract;
    //Alternative
    public event EventHandler OnPlayerAlternativeInteractPerformed;
    
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        
        playerInputActions.Player.Jump.performed += JumpOnPerformed;
        
        playerInputActions.Player.Crouch.performed += CrouchOnPerformed;
        playerInputActions.Player.Crouch.canceled += CrouchOnCanceled;

        playerInputActions.Player.Sprint.performed += SprintOnPerformed;
        
        playerInputActions.Player.Interact.performed += InteractOnPerformed;
        playerInputActions.Player.AlternativeInteract.performed += AlternativeInteractOnPerformed;
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }
    
    //Jumping
    private void JumpOnPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPlayerJumpPerformed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    //Crouching
    private void CrouchOnPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPlayerCrouchPerformed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    //Stopped crouching
    private void CrouchOnCanceled(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            OnPlayerCrouchCanceled?.Invoke(this, EventArgs.Empty);
        }
    }
    
    //Sprint
    private void SprintOnPerformed(InputAction.CallbackContext context)
    {
        OnPlayerSprintPerformed?.Invoke(this, EventArgs.Empty);
    }

    //Interactions
    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        OnPlayerInteract?.Invoke(this, EventArgs.Empty);
    }
    private void AlternativeInteractOnPerformed(InputAction.CallbackContext context)
    {
        OnPlayerAlternativeInteractPerformed?.Invoke(this, EventArgs.Empty);
    }
    
    public Vector2 GetMovementNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        
        inputVector = inputVector.normalized;
        return inputVector;
    }

    public Vector2 GetCameraRotation()
    {
        Vector2 lookVector = playerInputActions.Player.Look.ReadValue<Vector2>();

        return lookVector;
    }
}
