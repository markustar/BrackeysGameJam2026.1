
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float slowWalkSpeed = 3f;
    bool canSprint = true;
    bool canSlowWalk = true;
    bool canAttack = true;
    bool isMoving;
    
    InputManager inputManager;  

    Rigidbody2D _playerRb;

    [Header("Stamina")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float playerStamina = 100f;
    [SerializeField] float runCost = 15;
    [SerializeField] float slowWalkCost = 10;
    [SerializeField] float attackCost = 25;
    Coroutine regenStamina;
    

    [SerializeField] float chargeRate = 25f;
    bool IsDraining = false;

    [SerializeField] Image stamina;

    

    [Header("Combat")]

    [SerializeField] Transform aimPos;
    [SerializeField] GameObject attackHitBox;
    [SerializeField] float attackDuration = 0.3f;
    float attackTimer = 5f;
    [SerializeField] bool isAttacking = true ;

    [Header("Animation")]

    Animator anim;

    
    [Header("Sound")]
    [SerializeField] AudioClip footSteps; 
    [Range(0,1)]   
    [SerializeField] float volume;
    float footStepTimer;
    [SerializeField] float currentFootStepTimer;

    [SerializeField] float footStepTimerWalk = 0.6f;
    [SerializeField] float footStepTimerRun = 0.2f;
    [SerializeField] float footStepTimerSlowWalk = 1f;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        inputManager = FindFirstObjectByType<InputManager>();
        anim = GetComponentInChildren<Animator>();

        attackHitBox.SetActive(false);

        footStepTimer = 5f;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDraining && playerStamina < maxStamina)
        {
            if (regenStamina == null)
                regenStamina = StartCoroutine(StaminaRegenHandler());

        }
        IsDraining = false;

        if (runCost > playerStamina)
        {
            canSprint = false;
        }
        else { canSprint = true; }

        if (slowWalkCost > playerStamina)
        {
            canSlowWalk = false;
        }
        else { canSlowWalk = true; }

       Animate();

        // Combat

        if(attackCost > playerStamina)
            canAttack = false;
        else canAttack = true;
        MousePosToWorlView();
        CheckIfCanAttack();
    }

    private void MousePosToWorlView()
    {
        Vector3 mousePos;
        if (inputManager.attackAction.WasPressedThisFrame() && !isAttacking && canAttack)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector2 direction = mousePos - aimPos.position;
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // aimPos.rotation = Quaternion.Euler(0f, 0f, angle);

            if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if(direction.x > 0)
                aimPos.rotation = Quaternion.Euler(0,0,0);
                else 
                 aimPos.rotation = Quaternion.Euler(0,0,180);
            }
            else
            {
                if(direction.y > 0)
                 aimPos.rotation = Quaternion.Euler(0,0,90);
                else
                 aimPos.rotation = Quaternion.Euler(0,0,-90);
            }
            HandleAttack();
        }
    }

    void FixedUpdate()
    {   
        HandleMovement();
    }

    void HandleMovement()
    {   
        Vector2 input = inputManager.GetInput();
        float currentSpeed = 0f;
        if(input == Vector2.zero)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isSlowWalking", false);
            isMoving = false;
        }
        // run Logic
            
        if(inputManager.sprintAction.IsPressed() && canSprint)
        {
            currentSpeed = runSpeed;
            currentFootStepTimer = footStepTimerRun;
            isMoving = true;
            if(_playerRb.linearVelocity.magnitude > 0.1f)
            {
            StaminaSub(runCost);
            anim.SetBool("isRunning", true);
            PlaySound();
            }
            anim.SetBool("isSlowWalking", false);
             anim.SetBool("isWalking", false);
        }
        // stealth logic

        else if (inputManager.slowWalkAction.IsPressed() && canSlowWalk)
        {   
            currentSpeed = slowWalkSpeed;
            currentFootStepTimer = footStepTimerSlowWalk;
            isMoving = true;
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
            if(input != Vector2.zero)
            {
            anim.SetBool("isSlowWalking", true);
            PlaySound();
            }
            StaminaSub(slowWalkCost);
            
        }
        else // Walk Logic
        {
            currentSpeed = walkSpeed;
            currentFootStepTimer = footStepTimerWalk;
            isMoving = true;
            anim.SetBool("isRunning", false);
            anim.SetBool("isSlowWalking", false);
            if(input != Vector2.zero)
            {
            anim.SetBool("isWalking", true);
            PlaySound();
            }
            
        }

       Addvelocity(currentSpeed);
    
    }

    public void StaminaSub(float cost)
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

    Vector2 snappedinput;
    void Addvelocity(float speed)
    {   
        Vector2 rawInput = inputManager.GetInput();

        if(rawInput.x > 0) snappedinput = Vector2.right;
        if(rawInput.x < 0) snappedinput = Vector2.left;
        if(rawInput.y > 0) snappedinput = Vector2.up;
        if(rawInput.y < 0) snappedinput = Vector2.down;
        

        targetVelocity = snappedinput * speed;
        // _playerRb.linearVelocity = Vector2.Lerp(_playerRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        if(inputManager.GetInput() != Vector2.zero && !isAttacking)
        {
            _playerRb.linearVelocity = Vector2.Lerp(_playerRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        }
        else
        {
            _playerRb.linearVelocity = Vector2.Lerp(_playerRb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

    }

    public IEnumerator StaminaRegenHandler()
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

    void HandleAttack()
    {
        if(!isAttacking)
        {
            isAttacking = true;
            attackHitBox.SetActive(true);
            playerStamina -= attackCost;
            stamina.fillAmount = playerStamina / maxStamina;
        }
        
    }

    void CheckIfCanAttack()
    {
        if(isAttacking)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackDuration)
            {
                attackTimer = 0;
                isAttacking = false;
                attackHitBox.SetActive(false);
            }

        }
    }

    void Animate()
    {
        anim.SetFloat("MoveX", snappedinput.x);
        anim.SetFloat("MoveY", snappedinput.y);
    }

    
    void PlaySound()
    {
       if(!isMoving)
        {
            footStepTimer = 0f;
            return;
        }

        footStepTimer += Time.deltaTime;

        if(footStepTimer >= currentFootStepTimer)
        {
            footStepTimer = 0f;
            SoundFXManager.instance.PlaySoundFXClip(footSteps, this.transform, volume);
        }
    }

}
