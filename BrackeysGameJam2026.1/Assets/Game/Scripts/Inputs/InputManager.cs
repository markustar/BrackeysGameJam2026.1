using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    InputAction moveAction; 

    public InputAction sprintAction;
    public InputAction slowWalkAction;
    Vector2 moveValue;
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        slowWalkAction = InputSystem.actions.FindAction("Crouch");
    }

 
    void Update()
    {
        
       
    }

    public Vector2 GetInput()
    {
        moveValue = moveAction.ReadValue<Vector2>();
        moveValue = moveValue.normalized;
        return moveValue;
    }
}
