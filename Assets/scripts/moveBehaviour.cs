using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveBehaviour : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float runSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 20f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -30f;

    private Vector3 currentVelocity;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector3 inputDirection, bool isSprinting)
    {
        float targetSpeed = isSprinting ? runSpeed : moveSpeed;

        Vector3 localDirection = transform.TransformDirection(inputDirection.normalized);
        Vector3 targetVelocity = localDirection * targetSpeed;

        float accel = inputDirection.magnitude > 0.01f
            ? acceleration * (isSprinting ? 1.2f : 1f)
            : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            accel * Time.deltaTime
        );

        Vector3 motion = currentVelocity;

        // Gravedad
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        motion.y = verticalVelocity;

        controller.Move(motion * Time.deltaTime);
    }


    public void Jump()
    {
        if (!controller.isGrounded) return;

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}