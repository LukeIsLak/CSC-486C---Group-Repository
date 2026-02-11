using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] public float speed = 5f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float xSensitivity = 100f;
    [SerializeField] private float ySensitivity = 100f;
    [SerializeField] private float rotationXlimit = 80f;
    [SerializeField] private CharacterData characterData;
    private CharacterController controller;
    private Camera camera;
    private Vector2 moveDirection;
    private Vector2 lookValue;

    private float rotationX; 
    private float verticalVelocity;
    private bool isJumping;
    private bool isAttacking;
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
        controller.Move(move * speed * Time.deltaTime);

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
