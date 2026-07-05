using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MobileCharacterController3D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    
    [Header("Mobile Touch Control")]
    [SerializeField] private float touchSensitivity = 0.05f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 touchStartPos;
    private Vector2 touchInputVector;
    private bool isMoving = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleTouchInput();
        ApplyMovement();
    }

    private void HandleTouchInput()
    {
        // Detect touches on screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isMoving = true;
                    break;

                case TouchPhase.Moved:
                    // Calculate the drag vector relative to start position
                    Vector2 dragDelta = touch.position - touchStartPos;
                    touchInputVector = Vector2.ClampMagnitude(dragDelta * touchSensitivity, 1.0f);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    touchInputVector = Vector2.zero;
                    isMoving = false;
                    break;
            }
        }
        else
        {
            // Fallback for testing inside the Unity Editor using WASD/Arrow keys
            touchInputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    private void ApplyMovement()
    {
        if (characterController.isGrounded)
        {
            // Translate the 2D touch input into 3D world direction coordinates
            moveDirection = new Vector3(touchInputVector.x, 0.0f, touchInputVector.y);
            moveDirection *= moveSpeed;

            // Rotate character smoothly to face the moving direction
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // Apply gravity over time
        moveDirection.y -= gravity * Time.deltaTime;

        // Execute movement framework
        characterController.Move(moveDirection * Time.deltaTime);
    }
}