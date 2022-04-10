using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Serialized Fields
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float groundedDist = 0.01f;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;


    // Storage of the animator
    Animator animator;

    // Storage of the RigidBody
    Rigidbody rigidbody;

    // IDs for the action bools
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int isGroundedHash;
    int YVelocityHash;
    int XVelocityHash;
    int ZVelocityHash;

    // Reading player input
    PlayerInput input;

    // Storing player input
    bool walkingPressed;
    bool sprintPressed;
    bool jumpingPressed;

    // iteration values
    float curJumpForce;

    bool grounded;
    Vector2 move;
    float sprint;

    // Dylan's Jump Variables
    bool isJumpPressed;
    bool jumpHeld;
    bool isJumping;
    float initialJumpVelocity;
    [SerializeField]
    float firstJump;
    [SerializeField]
    float maxJumpHeight = 3;
    [SerializeField]
    float maxJumpTime = .5f;
    [SerializeField]
    float jumpTime = .35f;
    float jumpTimeCounter;

    private void Awake()
    {
        input = new PlayerInput();

        input.CharacterControls.Enable();

        // Subscribe events for character
        //input.CharacterControls.Movement.performed += ctx => walkingPressed = ctx.ReadValueAsButton();
        //input.CharacterControls.Jump.performed += ctx => jumpingPressed = ctx.ReadValueAsButton();
        //input.CharacterControls.Sprint.performed += ctx => sprintPressed = ctx.ReadValueAsButton();

        input.CharacterControls.Jump.started += OnJump;
        input.CharacterControls.Jump.canceled += OnJump;

        setJumpVariables();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize animator
        animator = GetComponent<Animator>();

        // Initialize RigidBody
        rigidbody = GetComponent<Rigidbody>();

        // Get IDs for triggers
        isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");
        isRunningHash = Animator.StringToHash("isRunning");
        isGroundedHash = Animator.StringToHash("isGrounded");
        YVelocityHash = Animator.StringToHash("YVelocity");
        XVelocityHash = Animator.StringToHash("XVelocity");
        ZVelocityHash = Animator.StringToHash("ZVelocity");

        // set iter values
        curJumpForce = jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void FixedUpdate()
    {
        animator.SetFloat(YVelocityHash, rigidbody.velocity.y);
    }

    void UpdateMovement()
    {
        move = input.CharacterControls.Movement.ReadValue<Vector2>();
        sprint = input.CharacterControls.Sprint.ReadValue<float>();
        //jump = input.CharacterControls.Jump.ReadValue<float>();

        grounded = GroundedCheck();

        if (grounded)
        {
            animator.SetFloat(ZVelocityHash, move.y);
            animator.SetFloat(XVelocityHash, move.x);
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isGroundedHash, true);
            animator.applyRootMotion = true;
        }
        else
        {
            animator.SetBool(isGroundedHash, false);
            animator.applyRootMotion = false;
            animator.SetFloat(ZVelocityHash, rigidbody.velocity.z);
            animator.SetFloat(XVelocityHash, rigidbody.velocity.x);
        }

        if(move.y >= 1 && sprint == 1)
        {
            animator.SetFloat(ZVelocityHash, 2);
            Debug.Log("Sprinting");
        }
        JumpCheck();
    }

    void JumpCheck()
    {
        if(!isJumping && grounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            animator.applyRootMotion = false;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rigidbody.AddForce(new Vector3(0, firstJump, 0), ForceMode.VelocityChange);
            jumpHeld = true;
        }
        else if(!isJumpPressed && isJumping && grounded)
        {
            animator.applyRootMotion = true;
            isJumping = false;
        }
        else if (!isJumpPressed && isJumping && !grounded)
        {
            jumpHeld = false;
        }

        if(isJumpPressed && isJumping && jumpHeld)
        {
            if(jumpTimeCounter > 0)
            {
                rigidbody.AddForce(new Vector3(0, initialJumpVelocity, 0), ForceMode.VelocityChange);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    void setJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    public bool GroundedCheck()
    {
        Debug.DrawRay(animator.rootPosition + transform.up * (groundedDist / 2) + (transform.forward * .1f), Vector3.down * groundedDist, Color.blue);
        if(Physics.Raycast(animator.rootPosition + transform.up * (groundedDist / 2) + (transform.forward * .1f), Vector3.down, maxDistance: groundedDist, playerLayer))
        {
            return true;
        }
        else if(Physics.Raycast(animator.rootPosition + transform.up * (groundedDist / 2) + (transform.right * .1f) - (transform.forward * .07f), Vector3.down, maxDistance: groundedDist, playerLayer))
        {
            Debug.DrawRay(animator.rootPosition + transform.up * (groundedDist / 2) + (transform.right * .15f), Vector3.down * groundedDist, Color.blue);
            return true;
        }
        else if (Physics.Raycast(animator.rootPosition + transform.up * (groundedDist / 2) - (transform.right * .15f) + (transform.forward * .07f), Vector3.down, maxDistance: groundedDist, playerLayer))
        {
            Debug.DrawRay(animator.rootPosition + transform.up * (groundedDist / 2) + (transform.right * .1f) - (transform.forward * .07f), Vector3.down * groundedDist, Color.blue);
            Debug.DrawRay(animator.rootPosition + transform.up * (groundedDist / 2) - (transform.right * .15f) + (transform.forward * .07f), Vector3.down * groundedDist, Color.blue);
            return true;
        }
        else
        {
            Debug.DrawRay(animator.rootPosition + transform.up * (groundedDist / 2) + (transform.right * .1f) - (transform.forward * .07f), Vector3.down * groundedDist, Color.blue);
            Debug.DrawRay(animator.rootPosition + transform.up * (groundedDist / 2) - (transform.right * .15f) + (transform.forward * .07f), Vector3.down * groundedDist, Color.blue);
            return false;
        }
    }

    private void OnEnable()
    {
        input.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        input.CharacterControls.Disable();
    }
}
