using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeadLook : MonoBehaviour
{
    PlayerInput input;
    Vector2 mouseLook;
    float xRotation = 0f;
    float yRotation = 0f;

    bool walkingPressed;
    bool isGrounded;
    Vector2 move;

    public bool cursorLocked = true;
    public float maxVertical = 90f;
    public float maxHorizontal = 45f;
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform playerHead;

    private void Awake()
    {
        // Get player input controls
        input = new PlayerInput();

        // Lock cursor if on
        switch (cursorLocked)
        {
            case true:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case false:
                Cursor.lockState = CursorLockMode.None;
                break;
        }

        // Subscribe events for character
        input.CharacterControls.Movement.performed += Movement;
        input.CharacterControls.Movement.canceled += Movement;

    }

    private void Update()
    {
        LookRotation();
    }

    private void LookRotation()
    {
        mouseLook = input.CharacterControls.HeadLook.ReadValue<Vector2>();

        float mouseX = mouseLook.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseLook.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxVertical, maxVertical);
        yRotation -= mouseX;
        yRotation = Mathf.Clamp(yRotation, -maxHorizontal, maxHorizontal);

        //if !hanging
        playerHead.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
        //else
        //playerHead.localRotation = Quaternion.Euler(xRotation, -yRotation, 0);
    }

    void Movement(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        //Debug.Log(move);
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
