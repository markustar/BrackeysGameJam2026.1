using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float slowWalkSpeed = 3f;
    InputManager inputManager;  

    Rigidbody2D _playerRb;
    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        inputManager = FindFirstObjectByType<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // walk
        Addvelocity(walkSpeed);

        // run

        if(inputManager.sprintAction.IsPressed())
        {
             Addvelocity(runSpeed);
        }
        // stealth

        if(inputManager.slowWalkAction.IsPressed())
        {
            Addvelocity(slowWalkSpeed);
        }
    }

    void Addvelocity(float speed)
    {
        _playerRb.linearVelocity = inputManager.GetInput() * speed;
    }
}
