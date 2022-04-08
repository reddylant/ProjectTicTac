using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /*  This enum will tell us when the player is on the ground jumping or falling in the air,
        or hanging on a ledge they grabbed. This is how we will determine what animations are
        played, what rays are casted, and any other behaviour   */
    enum MoveState
    {
        Ground,
        Air,
        Ledge
    }

    //Serialized Fields
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float groundedDist = 0.01f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbSpeed;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private MoveState moveState;


    // Storage of the animator
    Animator animator;

    // Storage of the RigidBody
    Rigidbody rigidbody;

    // IDs for the action bools
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int isBracedHash;
    int isHangingHash;
    int isGroundedHash;

    // Reading player input
    PlayerInput input;

    // Storing player input
    bool walkingPressed;
    bool sprintPressed;
    bool jumpingPressed;

    // iteration values
    float curJumpForce;

    // Location of a hit point
    Vector3 nextPoint;
    Vector3 nextNormal;

    // Private variables
    bool isGrounded;
    ParkourDetection PD;
    Transform hands;

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

        // Initialize Parkour Detection
        PD = GetComponent<ParkourDetection>();

        // Get IDs for triggers
        isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");
        isRunningHash = Animator.StringToHash("isRunning");
        isBracedHash = Animator.StringToHash("isBraced");
        isHangingHash = Animator.StringToHash("isHanging");
        isGroundedHash = Animator.StringToHash("isGrounded");

        // set iter values
        curJumpForce = jumpForce;

        hands = gameObject.transform.Find("Hands").transform;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        isGrounded = GroundedCheck();

    }

    private void FixedUpdate()
    {
        switch (moveState)
        {
            case MoveState.Ground:
                animator.applyRootMotion = true;
                rigidbody.isKinematic = false;
                break;
            case MoveState.Air:
                rigidbody.isKinematic = false;
                break;
            case MoveState.Ledge:
                rigidbody.isKinematic = true;
                animator.applyRootMotion = false;
                break;
            default:
                break;
        }
    }

    void handleMovement()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        
        //animator.SetBool(isHangingHash, true);

        animator.SetBool(isGroundedHash, isGrounded);
        
        if (isGrounded && moveState != MoveState.Ledge)
        {
            moveState = MoveState.Ground;
        }
        else if (!isGrounded && moveState != MoveState.Ledge)
        {
            moveState = MoveState.Air;
            animator.applyRootMotion = false;
        }


        if (moveState == MoveState.Ground)
        {
            // Start root motion of walking
            if (walkingPressed && !isWalking && isGrounded)
            {
                animator.SetBool(isWalkingHash, true);
            }

            // Start root motion of running
            if ((walkingPressed && sprintPressed) && !isRunning && isGrounded)
            {
                animator.SetBool(isRunningHash, true);
            }
        }
        // Stop root motion of walking
        if ((!walkingPressed && isWalking) || !isGrounded)
        {
            animator.SetBool(isWalkingHash, false);
        }
        // Stop root motion of running
        if (((!walkingPressed || !sprintPressed) && isRunning) || !isGrounded)
        {
            animator.SetBool(isRunningHash, false);
        }

        // Start and stop jumping
        if (jumpingPressed && !isJumping && isGrounded && moveState == MoveState.Ground)
        {
            if (PD.FindLedge())
            {
                moveState = MoveState.Ledge;
                nextPoint = new Vector3(PD.hitHor.point.x, PD.hitVert.point.y, PD.hitHor.point.z);
                nextNormal = -PD.hitHor.normal;
                animator.SetBool(isBracedHash, true);

                // Gets direction player needs to face
                Quaternion lookRotaion = Quaternion.LookRotation(nextNormal);

                // Move player to new position
                transform.position = Vector3.Lerp(transform.position, (nextPoint - transform.rotation * hands.localPosition), Time.deltaTime * climbSpeed);

                // Rotate player
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotaion, Time.deltaTime * climbSpeed);

            }
            else
            {
                animator.SetBool(isJumpingHash, true);
                rigidbody.drag = 0f;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
                rigidbody.AddForce(new Vector3(0f, curJumpForce, 0f), ForceMode.Impulse);
                curJumpForce = curJumpForce / 4;
                if (curJumpForce < 0)
                {
                    curJumpForce = 0;
                }
                moveState = MoveState.Air;
            }
            
        }
        else if (jumpingPressed && !isGrounded)
        {
            if (PD.FindLedge())
            {
                moveState = MoveState.Ledge;
                nextPoint = new Vector3(PD.hitHor.point.x, PD.hitVert.point.y, PD.hitHor.point.z);
                nextNormal = -PD.hitHor.normal;
                animator.SetBool(isBracedHash, true);

                // Gets direction player needs to face
                Quaternion lookRotaion = Quaternion.LookRotation(nextNormal);

                // Move player to new position
                transform.position = Vector3.Lerp(transform.position, (nextPoint - transform.rotation * hands.localPosition), Time.deltaTime * climbSpeed);

                // Rotate player
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotaion, Time.deltaTime * climbSpeed);

            }
            else
            {
                rigidbody.AddForce(new Vector3(0f, curJumpForce, 0f));
                curJumpForce = curJumpForce / 2;
                if (curJumpForce < 0)
                {
                    curJumpForce = 0;
                }
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
