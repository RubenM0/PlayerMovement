using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; private set; }

    [SerializeField] private InputManager input;
    [SerializeField] private Transform groundCheck;
 
    private Rigidbody rb;
    
    //Walking
    [SerializeField] private float moveForce = 90f;
    
    //Airborne
    [SerializeField] private float airControlMultiplier = 0.9f;
    [SerializeField] private float airDrag = 2f;
    private bool isWalking;
    public bool IsWalking => isWalking;
    
    //Jumping
    private float jumpForce = 20f;
    
    //Crouching
    private Vector3 playerScale;
    private Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);
    
    public bool isCrouching {get; private set;}
    private bool canUncrouch;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        rb = GetComponent<Rigidbody>();

        playerScale = transform.localScale;
    }

    private void Start()
    {
        input.OnPlayerJumpPerformed += Input_OnPlayerJumpPerformed;
        
        input.OnPlayerCrouchPerformed += Input_OnPlayerCrouchPerformed;
        input.OnPlayerCrouchCanceled += Input_OnPlayerCrouchCanceled;
        
        input.OnPlayerSprintPerformed += Input_OnPlayerSprintPerformed;
    }
    
    private void FixedUpdate()
    {
        bool grounded = CheckGroundDetection();

        Vector2 inputVector = input.GetMovementNormalized();
        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y);
        inputDirection = transform.right * inputDirection.x + transform.forward * inputDirection.z;
        
        float force = grounded ? moveForce : moveForce * airControlMultiplier;
        rb.AddForce(inputDirection * force, ForceMode.Acceleration);

        //Basically creating air drag so player isn't faster in air
        if (!grounded)
        {
            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
            horizontal = Vector3.Lerp(horizontal, Vector3.zero, airDrag * Time.deltaTime);
            rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
        }

        isWalking = inputDirection != Vector3.zero && grounded;
        
        AllowedToUncrouch();
        ForceUncrouch();
    }
    
    //Dashing
    private void Input_OnPlayerSprintPerformed(object sender, EventArgs e)
    {
        //Empty atm
    }

    private void Input_OnPlayerJumpPerformed(object sender, EventArgs e)
    {
        if (!CheckGroundDetection())
            return;
        
        rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }

    public bool CheckGroundDetection()
    {
        //Use this value for the Y axis to place the ground check game object: -0.95
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
    }

    private void Input_OnPlayerCrouchCanceled(object sender, EventArgs e)
    {
        if (!AllowedToUncrouch())
            return;
        
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            
        isCrouching = false;
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
