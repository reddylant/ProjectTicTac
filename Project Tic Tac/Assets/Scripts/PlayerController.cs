using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum MoveState
    {
        Ground,
        Air,
        Ledge
    }

    //Serialized Fields
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float groundedDist = 0.01f;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;
    [SerializeField] float firstJump;
    [SerializeField] float maxJumpHeight = 3;
    [SerializeField] float maxJumpTime = .5f;
    [SerializeField] float jumpTime = .35f;

    // Storage of the animator
    Animator animator;

    // Storage of the RigidBody
    Rigidbody rigidbody;

    // IDs for the action bools
    int isJumpingHash;
    int isCrouchingHash;
    int isGroundedHash;
    int isHangingHash;
    int isBracedHash;
    int isMantlingHash;
    int YVelocityHash;
    int XVelocityHash;
    int ZVelocityHash;

    // Reading player input
    PlayerInput input;

    bool grounded;
    Vector2 move;
    float sprint;
    float crouch;

    // Jump Variables
    bool isJumpPressed;
    bool jumpHeld;
    bool isJumping;
    float initialJumpVelocity;
    float jumpTimeCounter;

    // Parkour variables
    ParkourDetection PD;
    Vector3 targetLocation;
    Quaternion targetRotation;
    [SerializeField]
    MoveState moveState;
    [SerializeField]
    Transform hands;
    
    // Speed the player will move from starting position to next position
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float rotateSpeed;

    // Checks if player is moving while on ledge
    bool isMoving;

    private void Awake()
    {
        input = new PlayerInput();

        input.CharacterControls.Enable();

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
        isJumpingHash = Animator.StringToHash("isJumping");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isGroundedHash = Animator.StringToHash("isGrounded");
        YVelocityHash = Animator.StringToHash("YVelocity");
        XVelocityHash = Animator.StringToHash("XVelocity");
        ZVelocityHash = Animator.StringToHash("ZVelocity");
        isBracedHash = Animator.StringToHash("isBraced");
        isHangingHash = Animator.StringToHash("isHanging");
        isMantlingHash = Animator.StringToHash("isMantling");

        // Set parkour variables
        PD = GetComponent<ParkourDetection>();
        hands = GameObject.FindGameObjectWithTag("Player").transform.Find("Hands");
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
        crouch = input.CharacterControls.Crouch.ReadValue<float>();

        // Moves player to targetLocation
        if (isMoving && transform.position != targetLocation )
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.deltaTime);
        }
        else if (isMoving && transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }

        // If player is attached to a ledge
        if (moveState == MoveState.Ledge)
        {
            rigidbody.isKinematic = true;
            animator.applyRootMotion = false;
            animator.SetBool(isBracedHash, true);

            if (!isMoving)
            {
                animator.SetFloat(ZVelocityHash, move.y);
                animator.SetFloat(XVelocityHash, move.x);
            }
            
            if(!isMoving)
            {
                if (move.y >= .8f)
                {
                    if (PD.LedgeUp())
                    {
                        isMoving = true;
                        UpdateTarget();
                    }
                    else if (PD.Mantle())
                    {
                        Debug.Log("Mantling");
                        moveState = MoveState.Ground;
                        animator.applyRootMotion = true;
                        animator.SetBool(isMantlingHash, true);
                        animator.SetBool(isBracedHash, false);

                        targetLocation = PD.hitVert.point;
                        isMoving = true;
                        Debug.Log(targetLocation);
                    }
                }
                if (move.x >= .8f)
                {
                    if (PD.WallCheckRight())
                    {
                        if (PD.LedgeCornerRight())
                        {
                            isMoving = true;
                            UpdateTarget();
                        }
                    }
                    else
                    {
                        if (PD.LedgeRight())
                        {
                            isMoving = true;
                            UpdateTarget();
                        }
                        else
                        {
                            if (PD.LedgeCornerRight())
                            {
                                isMoving = true;
                                UpdateTarget();
                            }
                        }
                    }
                }
                if (move.x <= -.8f)
                {
                    if (PD.WallCheckLeft())
                    {
                        if (PD.LedgeCornerLeft())
                        {
                            isMoving = true;
                            UpdateTarget();
                        }
                    }
                    else
                    {
                        if (PD.LedgeLeft())
                        {
                            isMoving = true;
                            UpdateTarget();
                        }
                        else
                        {
                            if (PD.LedgeCornerLeft())
                            {
                                isMoving = true;
                                UpdateTarget();
                            }
                        }
                    }
                }
            }
        }
        // Else if the player is not attached to a ledge
        else
        {
            animator.applyRootMotion = true;
            animator.SetBool(isBracedHash, false);
            grounded = GroundedCheck();
            
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Braced To Crouch"))
            {
                rigidbody.isKinematic = false;
                animator.SetBool(isMantlingHash, false);
                isMoving = false;
                
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
            }

            if (move.y >= 0.8 && sprint == 1)
            {
                animator.SetFloat(ZVelocityHash, 2);
            }

            if (crouch == 1)
                animator.SetBool(isCrouchingHash, true);
            else
                animator.SetBool(isCrouchingHash, false);
        }

        JumpCheck();
    }

    void JumpCheck()
    {
        if (moveState == MoveState.Ledge)
        {
            // This will handle jumping off ledge
        }
        else
        {
            if (!isJumping && grounded && isJumpPressed)
            {
                animator.SetBool(isJumpingHash, true);
                animator.applyRootMotion = false;
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rigidbody.AddForce(new Vector3(0, firstJump, 0), ForceMode.VelocityChange);
                jumpHeld = true;
            }
            else if (!isJumpPressed && isJumping && grounded)
            {
                animator.applyRootMotion = true;
                isJumping = false;
            }
            else if (!isJumpPressed && isJumping && !grounded)
            {
                jumpHeld = false;
            }

            if (isJumpPressed && isJumping && jumpHeld)
            {
                if (PD.FindLedge())
                {
                    moveState = MoveState.Ledge;
                    rigidbody.isKinematic = false;
                    UpdateTarget();
                    isMoving = true;
                }
                else if (jumpTimeCounter > 0)
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

    void UpdateTarget()
    {
        targetLocation = PD.hitHor.point;
        targetLocation.y = PD.hitVert.point.y;
        targetLocation.y -= hands.localPosition.y;
        targetLocation += -(hands.forward * hands.localPosition.z);

        targetRotation = Quaternion.LookRotation(-PD.hitHor.normal);
    }
}
