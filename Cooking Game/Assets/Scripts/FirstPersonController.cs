using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonController : MonoBehaviour
{
    public const string SensitivityKey = "LookSensitivity";

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.9f;
    [SerializeField] private float controllerSensitivity = 140f;
    [SerializeField] private float controllerSmoothness = 20f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;
    private Vector2 smoothedControllerLook;
    private float sensitivityMultiplier = 1f;   // set by the pause-menu slider
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);

    void Awake()
    {
        // load the saved slider value (Awake runs even if this script starts disabled)
        sensitivityMultiplier = PlayerPrefs.GetFloat(SensitivityKey, 1f);

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    // Called by the pause-menu slider
    public void SetSensitivityMultiplier(float value)
    {
        sensitivityMultiplier = Mathf.Clamp(value, 0.1f, 5f);
        PlayerPrefs.SetFloat(SensitivityKey, sensitivityMultiplier);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (playerInputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleJumping();
        if (animator != null)
        {
            animator.SetFloat("Speed", playerInputHandler.MovementInput.magnitude);
        }

        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        Vector2 lookInput = playerInputHandler.RotationInput;

        // Check whether the current look input is coming from a controller
        if (UnityEngine.InputSystem.Gamepad.current != null &&
            UnityEngine.InputSystem.Gamepad.current.rightStick.ReadValue().magnitude > 0.01f)
        {
            // Smooth the controller input
            smoothedControllerLook = Vector2.Lerp(
                smoothedControllerLook,
                lookInput,
                controllerSmoothness * Time.deltaTime
            );

            float controllerXRotation = smoothedControllerLook.x * controllerSensitivity * sensitivityMultiplier * Time.deltaTime;
            float controllerYRotation = smoothedControllerLook.y * controllerSensitivity * sensitivityMultiplier * Time.deltaTime;

            ApplyHorizontalRotation(controllerXRotation);
            ApplyVerticalRotation(controllerYRotation);
        }
        else
        {
            smoothedControllerLook = Vector2.zero;

            float mouseXRotation = lookInput.x * mouseSensitivity * sensitivityMultiplier;
            float mouseYRotation = lookInput.y * mouseSensitivity * sensitivityMultiplier;

            ApplyHorizontalRotation(mouseXRotation);
            ApplyVerticalRotation(mouseYRotation);
        }
    }
}
