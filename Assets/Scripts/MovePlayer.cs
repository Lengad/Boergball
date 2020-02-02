using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class MovePlayer : NetworkBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 tmpMoveDirection;

    [SerializeField]
    private float rotY;
    private CameraFollow cameraFollow;

    AudioSource audioData;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioData = GetComponent<AudioSource>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        cameraFollow = Camera.main.GetComponentInParent<CameraFollow>();
        cameraFollow.target = transform;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            tmpMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            tmpMoveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                tmpMoveDirection.y = jumpSpeed;
                audioData.Play(0);
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        tmpMoveDirection.y -= gravity * Time.deltaTime;
        moveDirection = transform.TransformDirection(tmpMoveDirection);
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
        // Player rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, cameraFollow.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
           
    }
}