using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;
    public float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    void Start()
    {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        if (capsuleCollider != null)
        {
            Destroy(capsuleCollider);
        }

        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        controller.height = 2f;
        controller.radius = 0.5f;
        controller.center = new Vector3(0f, 0f, 0f);

        controller.skinWidth = 0.02f;
        controller.stepOffset = 0.3f;
        controller.slopeLimit = 60f;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);

        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        Vector3 horizontalMove = input * moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = horizontalMove;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}