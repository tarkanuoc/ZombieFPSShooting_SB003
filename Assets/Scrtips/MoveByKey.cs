using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByKey : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    public float movingSpeed;
    public float speed = 6.0f;
    public float jumpSpeed = 20.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;


    private void Update()
    {
        // characterController.SimpleMove(direction * movingSpeed);

        if (characterController.isGrounded)
        {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");

            moveDirection = transform.right * hInput + transform.forward * vInput;
            // We are grounded, so recalculate move direction based on axes
            // moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

}
