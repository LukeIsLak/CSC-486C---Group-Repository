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
    [SerializeField] private float xSensitivity = 0.1f;
    [SerializeField] private float ySensitivity = 0.1f;
    [SerializeField] private float rotationXlimit = 80f;
    [SerializeField] private CharacterData characterData;
    
    [HideInInspector] public CharacterController controller;
    private Camera camera;
    public Vector2 moveDirection;
    private Vector2 lookValue;

    private float rotationX;
    private float verticalVelocity;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();

    }
    // Update is called once per frame
    void Update()
    {
        controller.Move(Vector3.zero);

        Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.y;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // keep grounded
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
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }

    }
    public void OnLook(InputAction.CallbackContext context)
    {
        lookValue = context.ReadValue<Vector2>();
        //Debug.Log($"Look value: {moveDirection}");
    }

    private void Look()
    {
        float mouseX = lookValue.x * xSensitivity;
        float mouseY = lookValue.y * ySensitivity;

        // Vertical look (move cam)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -rotationXlimit, rotationXlimit);
        camera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Horizontal look ( move body)
        transform.Rotate(Vector3.up * mouseX);
    }

    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (UIManager.instance.isInventoryOpen)
        {
            UIManager.instance?.HideInventoryView();
        }
        else
        {
            UIManager.instance?.ShowInventoryView();
        }

    }

    public void OnTogglePause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (PauseManager.instance.isPause)
        {
            PauseManager.instance?.Resume();
        }
        else
        {
            PauseManager.instance?.Pause();
        }
    }
}
