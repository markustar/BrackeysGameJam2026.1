using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float slowWalkSpeed = 3f;
    bool canSprint = true;
    bool canSlowWalk = true;
    bool canAttack = true ;
    InputManager inputManager;  

    Rigidbody2D _playerRb;

    [Header("Stamina")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float playerStamina = 100f;
    [SerializeField] float runCost = 15;
    [SerializeField] float slowWalkCost = 10;
    [SerializeField] float attackCost = 20;
    Coroutine regenStamina;
    

    [SerializeField] float chargeRate = 25f;
    bool IsDraining = false;

    [SerializeField] Image stamina;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        inputManager = FindFirstObjectByType<InputManager>();

       
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsDraining && playerStamina < maxStamina) 
        {   
            if(regenStamina == null)
            regenStamina = StartCoroutine(StaminaRegenHandler());
            
        }
        IsDraining = false;

        if(runCost > playerStamina)
        {
            canSprint = false;
        }
        else {canSprint = true;}

        if(slowWalkCost > playerStamina)
        {
            canSlowWalk = false;
        }
        else {canSlowWalk = true;}
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

        if(inputManager.sprintAction.IsPressed() && canSprint)
        {
            Addvelocity(runSpeed);
            
            if(_playerRb.linearVelocity.magnitude > 0.1f)
            {
            StaminaSub(runCost);
            }
        }
        // stealth

        if (inputManager.slowWalkAction.IsPressed() && canSlowWalk)
        {   
            
            Addvelocity(slowWalkSpeed);
            StaminaSub(slowWalkCost);
            
        }

       
    
    }

    private void StaminaSub(float cost)
    {   
        playerStamina -= cost * Time.deltaTime;
        stamina.fillAmount = playerStamina / maxStamina;
        IsDraining = true;
        if(playerStamina < 0)
        {
            playerStamina = 0;
        }
        if(regenStamina != null)
        {
            StopCoroutine(regenStamina);
            regenStamina = null;
        }
    }

    Vector2 targetVelocity;
    float acceleration = 20f;
    float deceleration = 30f;
    void Addvelocity(float speed)
    {    
        targetVelocity = inputManager.GetInput() * speed;
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

    IEnumerator StaminaRegenHandler()
    {   
        yield return new WaitForSeconds(1f);
        while (playerStamina < maxStamina)
        {
        playerStamina += chargeRate / 10f;
        if(playerStamina > maxStamina)
            {
                playerStamina = maxStamina;
            }
        stamina.fillAmount = playerStamina / maxStamina;    
        yield return new WaitForSeconds(0.1f);
        }
        regenStamina = null;
    }

    
}
