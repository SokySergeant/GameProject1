using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float jumpPower = 10f;
    private Vector2 horizontalInput;

    private CharacterController controller;

    Vector3 velocity;



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 gravityDir = gravity.normalized;

        velocity += gravity * Time.deltaTime;

        Vector3 moveVector = new Vector3(horizontalInput.x * playerSpeed * Time.deltaTime, 0f, 0f);
        controller.Move(velocity * Time.deltaTime + moveVector);
        
        if (controller.isGrounded){
            velocity = Vector3.ProjectOnPlane(velocity, gravityDir); // The sign of the normal doesn't matter.
        }
    }



    //get horizontal input
    public void OnMove(InputValue input){
        horizontalInput = input.Get<Vector2>();
    }



    public void OnJump(InputValue input){
        if(controller.isGrounded){
            Vector3 gravityDir = Physics.gravity.normalized;
            velocity += -gravityDir * jumpPower;
        }
    }
}
