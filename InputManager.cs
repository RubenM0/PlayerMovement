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

    //Dashing
    public event EventHandler OnPlayerDashPerformed;
    
    //Interacting
    public event EventHandler OnPlayerInteractPerformed;

    //Pause game
    public event EventHandler OnPlayerPausedGame;
    
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    private void Start()
    {
        playerInputActions.Player.Jump.performed += JumpOnPerformed;
        
        playerInputActions.Player.Crouch.performed += CrouchOnPerformed;
        playerInputActions.Player.Crouch.canceled += CrouchOnCanceled;

        playerInputActions.Player.Dash.performed += DashOnPerformed;
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
    
    //Dash
    private void DashOnPerformed(InputAction.CallbackContext context)
    {
        OnPlayerDashPerformed?.Invoke(this, EventArgs.Empty);
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