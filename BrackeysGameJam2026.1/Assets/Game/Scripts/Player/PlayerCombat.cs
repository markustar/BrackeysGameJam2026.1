using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    InputManager inputManager;
    
    void Start()
    {
        inputManager = FindFirstObjectByType<InputManager>();
    }

   
    void Update()
    {
        
    }
}
