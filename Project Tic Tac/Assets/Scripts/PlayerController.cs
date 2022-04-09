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

    private void Awake()
    {
        input = new PlayerInput();

        // Subscribe events for character
        input.CharacterControls.Movement.performed += ctx => walkingPressed = ctx.ReadValueAsButton();
        input.CharacterControls.Jump.performed += ctx => jumpingPressed = ctx.ReadValueAsButton();
        input.CharacterControls.Sprint.performed += ctx => sprintPressed = ctx.ReadValueAsButton();
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
        YVelocityHash = Animator.StringToHash("YVelocity");
        XVelocityHash = Animator.StringToHash("XVelocity");
        ZVelocityHash = Animator.StringToHash("ZVelocity");

        // set iter values
        curJumpForce = jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }

    private void FixedUpdate()
    {
        animator.SetFloat(YVelocityHash, rigidbody.velocity.y);
    }

    void handleMovement()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        bool isGrounded = GroundedCheck();

        // Start and stop root motion of walking and running
        if (walkingPressed && !sprintPressed && isGrounded)
        {
            animator.SetFloat(ZVelocityHash, 1);
        }
        else if ((walkingPressed && sprintPressed) && isGrounded)
        {
            animator.SetFloat(ZVelocityHash, 2);
        }
        else if ((!walkingPressed || !sprintPressed) || (!isGrounded && !isJumping))
        {
            animator.SetFloat(ZVelocityHash, 0);
        }

        if (jumpingPressed && !isJumping && isGrounded)
        {
            if (walkingPressed && !sprintPressed && isGrounded)
            {
                animator.SetFloat(ZVelocityHash, 1);
            }
            else if ((walkingPressed && sprintPressed) && isGrounded)
            {
                animator.SetFloat(ZVelocityHash, 2);
            }

            animator.SetBool(isJumpingHash, true);
            rigidbody.drag = 0f;
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            rigidbody.AddForce(new Vector3(0f, curJumpForce, 0f), ForceMode.Impulse);
            curJumpForce = curJumpForce / 4;
            if (curJumpForce < 0)
            {
                curJumpForce = 0;
            }
        }
        else if (jumpingPressed && !isGrounded)
        {
            if (walkingPressed && !sprintPressed)
            {
                animator.SetFloat(ZVelocityHash, 1);
            }
            else if (walkingPressed && sprintPressed)
            {
                animator.SetFloat(ZVelocityHash, 2);
            }

            rigidbody.AddForce(new Vector3(0f, curJumpForce, 0f));
            curJumpForce = curJumpForce / 2;
            if (curJumpForce < 0)
            {
                curJumpForce = 0;
            }
        }

        if ((!jumpingPressed || isGrounded) && isJumping)
        {
            animator.SetBool(isJumpingHash, false);
            curJumpForce = jumpForce;
        }
    }

    public bool GroundedCheck()
    {
        
        bool rayHit = Physics.Raycast(animator.rootPosition + transform.up * (groundedDist / 2), Vector3.down, maxDistance: groundedDist, playerLayer);

        Color rayColor;

        if (rayHit)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(animator.rootPosition + transform.up * groundedDist, Vector3.down * groundedDist, rayColor);

        return rayHit;
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
