using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float xSensitivity = 100f;
    [SerializeField] float ySensitivity = 100f;
    [SerializeField] float rotationXlimit = 80f;

    private CharacterController controller;
    private Camera camera;

    private Vector2 moveDirection;
    private Vector2 lookValue;

    private float rotationX; 
    private float verticalVelocity;
    private bool isJumping;
    private float currentSpeed;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isJumping && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
            isJumping = false;
        }
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        Look();

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        //Debug.Log($"Move value: {moveDirection}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isJumping = true;
        }

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookValue = context.ReadValue<Vector2>();
        //Debug.Log($"Look value: {moveDirection}");
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) currentSpeed = sprintSpeed;

        if (context.canceled) currentSpeed = walkSpeed;
    }

    private void Look()
    {
        float mouseX = lookValue.x * xSensitivity * Time.deltaTime;
        float mouseY = lookValue.y * ySensitivity * Time.deltaTime;

        // Vertical look (move cam)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -rotationXlimit, rotationXlimit);
        camera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Horizontal look ( move body)
        transform.Rotate(Vector3.up * mouseX);
    }
}
