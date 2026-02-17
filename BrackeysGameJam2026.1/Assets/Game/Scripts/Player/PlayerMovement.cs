using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
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
        
    }
    void FixedUpdate()
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

    Vector2 targetVelocity;
    float acceleration = 20f;
    float deceleration = 30f;
    void Addvelocity(float speed)
    {    
        targetVelocity = inputManager.GetInput() * speed * Time.fixedDeltaTime;
        // _playerRb.linearVelocity = Vector2.Lerp(_playerRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        if(inputManager.GetInput() != Vector2.zero)
        {
            _playerRb.linearVelocity = Vector2.Lerp(_playerRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        }
        else
        {
            _playerRb.linearVelocity = Vector2.Lerp(_playerRb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

    }
}
