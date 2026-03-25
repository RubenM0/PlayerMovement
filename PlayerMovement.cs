using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; private set; }

    //Update visuals
    public event EventHandler OnPlayerDashed;
    
    [SerializeField] private InputManager input;
    [SerializeField] private Transform groundCheck;
 
    private Rigidbody rb;
    
    //Walking
    private float moveForce = 2f;
    private bool isWalking;
    public bool IsWalking => isWalking;
    
    //Jumping
    private float jumpForce = 40f;
    
    //Crouching
    private Vector3 playerScale;
    private Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);
    
    public bool isCrouching {get; private set;}
    private bool canUncrouch;
    
    //Sliding (atp)
    private float slideForce = 4f;
    private bool isSliding;
    
    //Dashing
    private float dashForce = 70f;
    private float dashCooldown = 1.8f;
    public float DashCooldown => dashCooldown;
    
    private bool readyToDash;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        rb = GetComponent<Rigidbody>();

        readyToDash = true;
        playerScale = transform.localScale;
    }

    private void Start()
    {
        input.OnPlayerJumpPerformed += Input_OnPlayerJumpPerformed;
        
        input.OnPlayerCrouchPerformed += Input_OnPlayerCrouchPerformed;
        input.OnPlayerCrouchCanceled += Input_OnPlayerCrouchCanceled;
        
        input.OnPlayerDashPerformed += Input_OnPlayerDashPerformed;
    }
    
    private void FixedUpdate()
    {
        Vector2 inputVector = input.GetMovementNormalized();
        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y);
        inputDirection = transform.right * inputDirection.x + transform.forward * inputDirection.z;

        if (isSliding)
            return;
        
        rb.AddForce(inputDirection * moveForce, ForceMode.Impulse);

        isWalking = inputDirection != Vector3.zero && CheckGroundDetection();
        
        AllowedToUncrouch();
        ForceUncrouch();
    }
    
    //Dashing
    private void Input_OnPlayerDashPerformed(object sender, EventArgs e)
    {
        if (readyToDash)
        {
            Vector3 dashDirection = GetPlayerDirection();
        
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        
            OnPlayerDashed?.Invoke(this, EventArgs.Empty);
            readyToDash = false;
        
            Invoke(nameof(ResetDashCooldown), dashCooldown);
        }
    }

    //Dash + sliding based on key holding
    private Vector3 GetPlayerDirection()
    {
        Vector2 inputVector = input.GetMovementNormalized();
    
        //Check for holding a movement key
        if (inputVector.sqrMagnitude > 0.01f)
        {
            Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y);
            direction = transform.right * direction.x + transform.forward * direction.z;
            return direction.normalized;
        }
    
        return transform.forward;
    }
    
    private void ResetDashCooldown()
    {
        readyToDash = true;
    }
    
    private void Input_OnPlayerJumpPerformed(object sender, EventArgs e)
    {
        if (!CheckGroundDetection())
            return;
        
        rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }

    public bool CheckGroundDetection()
    {
        Vector3 playerDownVector = Vector3.down; 
        
        float radius = 0.025f;
        float dist = 0.1f;
        
        RaycastHit hit;
        if (Physics.SphereCast(groundCheck.transform.position, radius, playerDownVector, out hit, dist))
        {
            if (hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }
    
    //Crouching
    private void Input_OnPlayerCrouchPerformed(object sender, EventArgs e)
    {
        if (!CheckGroundDetection())
            return;
        
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            
        isCrouching = true;
        
        Vector2 inputVector = input.GetMovementNormalized();
        if (inputVector.sqrMagnitude > 0.1f)
        {
            isSliding = true;
        }
    }

    private void Input_OnPlayerCrouchCanceled(object sender, EventArgs e)
    {
        if (!AllowedToUncrouch())
            return;
        
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            
        isCrouching = false;
        isSliding = false;
    }
    
    private bool AllowedToUncrouch()
    {
        if (!isCrouching)
            return true;
        
        RaycastHit hit;

        float dist = 2f;
        float offset = 1.75f;
        
        if (Physics.Raycast(transform.position, Vector3.up * offset, out hit, dist))
        {
            if (hit.collider != null)
            {
                return false;
            }
        }
        return true;
    }
    
    //Force the player to uncrouch if not crouching and just left an area they were forced to stay crouched in
    private void ForceUncrouch()
    {
        
    }
}