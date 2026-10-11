using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonController : MonoBehaviour
{
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
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;
    private Vector2 smoothedControllerLook;
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);


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

            float controllerXRotation = smoothedControllerLook.x * controllerSensitivity * Time.deltaTime;
            float controllerYRotation = smoothedControllerLook.y * controllerSensitivity * Time.deltaTime;

            ApplyHorizontalRotation(controllerXRotation);
            ApplyVerticalRotation(controllerYRotation);
        }
        else
        {
            // Keep mouse behaviour unchanged
            smoothedControllerLook = Vector2.zero;

            float mouseXRotation = lookInput.x * mouseSensitivity;
            float mouseYRotation = lookInput.y * mouseSensitivity;

            ApplyHorizontalRotation(mouseXRotation);
            ApplyVerticalRotation(mouseYRotation);
        }
    }
}
    
