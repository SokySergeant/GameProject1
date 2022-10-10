using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovementScript : MonoBehaviour
{
    public float playerSpeed = 5f;
    private float horizontalInput;

    private CharacterController controller;



    void Start()
    {
        controller = GetComponent<CharacterController>();
    }



    void Update()
    {
        controller.Move(new Vector3(0f, 0f, horizontalInput * playerSpeed * Time.deltaTime));
    }



    //get horizontal input
    public void OnHorizontal(InputValue input){
        horizontalInput = input.Get<float>();
    }
}
