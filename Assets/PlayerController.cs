using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{

    public float speedF, speedB, speedS, sprintMulti, gravity, jumpForce, jumpDuration, bobAmount;
    public float health;
    public bool fly;
    public Camera cam;
    public CharacterController ch;
    public Image healthBar;
    public bool invincible;
    public float regenDelay;
    public float regenRate;
    public GameObject waterProcessing;
    public Volume damageProcessing;

    private float jumpTimer;
    private bool isGrounded;
    private bool isJumping;
    private Vector3 groundCheckPoint;
    private LayerMask groundLayer;
    private Vector3 jumpForceVector;
    private bool isCrouching;
    private Vector3 moveDirection;
    private Vector3 origWeaponPos;
    [HideInInspector]
    public bool isSprinting;
    public float maxHealth;
    public DeathControl deathControl;
    [HideInInspector]
    public bool isAiming;
    [HideInInspector]
    public bool isFire;

    private Vignette vignette;

    private float timeSinceDamage;
    
    void Start()
    {
        damageProcessing.profile.TryGet<Vignette>(out vignette);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ch = gameObject.GetComponent<CharacterController>();
        cam = gameObject.GetComponentInChildren<Camera>();
        jumpForceVector = new Vector3(0, jumpForce, 0);
        cam.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        maxHealth = health;
        timeSinceDamage = 0f;
    }
    private void OnCollisionEnter(Collision hit)
    {
        
        if (hit.gameObject.CompareTag("BulletAI"))
        {
            health--;
            float fill =health / maxHealth;
            healthBar.fillAmount = fill;
            timeSinceDamage = 0f;
            if (health <= 0f && !invincible)
            {
                Death();
                
            }
        }
    }

    public void ResetDeath()
    {
        float fill =health / maxHealth;
        healthBar.fillAmount = fill;
    }
    

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isGrounded = true;
    }

    void FixedUpdate()
    {
        float reset = Input.GetAxis("Debug Reset");
        if (reset > 0)
        {
            deathControl.Reset();
        }
        HandleCrouch(Input.GetKey(KeyCode.LeftControl));
        HandleMovement();
        HandleJump();
        isSprinting = Input.GetButton("Sprint");
        Regen();
        
        float swap = Input.GetAxis("Switch");
        if (swap == 0)
        {
            ch.Move(moveDirection * Time.deltaTime);
        }

    }
    private void Regen()
    {
        if (health < maxHealth && timeSinceDamage >= regenDelay)
        {
            health += regenRate * Time.deltaTime;
            health = Mathf.Clamp(health, 0f, maxHealth);
            float fill =health / maxHealth;
            healthBar.fillAmount = fill;
        }
        vignette.intensity.value = 0.5f * (1-(health / maxHealth));
        timeSinceDamage += Time.deltaTime;
    }

    private void Death()
    {
        isAiming = false;
        deathControl.Death();
    }



    private void HandleCrouch(bool up)
    {
        float targetPosY = up ? -0.5f : 0.5f;
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, targetPosY, 0), Time.deltaTime * 5);
        isCrouching = up;
        if (up)
        {
            isSprinting = false;
        }
    }


    private void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        
        float speedf = vertical > 0 ? (isSprinting && !isAiming ? speedF * sprintMulti : isAiming ? speedF * 0.5f : speedF) : speedB * (isAiming ? 0.5f : 1f);
        float speeds = horizontal != 0 ? (isAiming ? speedS * 0.5f : speedS) : 0f;
    
        float crouchMultiplier = isCrouching ? 0.7f : 1f;
        //if (transform.position.y >= -2.9)
        //{
        moveDirection = (transform.forward * vertical * speedf * crouchMultiplier + transform.right * horizontal * speeds * crouchMultiplier + Vector3.down * gravity * Time.deltaTime*Time.deltaTime);
        //}
        // else if(transform.position.y > -3.1)
        // { 
        //     moveDirection = (transform.forward * vertical * speedf * crouchMultiplier + transform.right * horizontal * speeds * crouchMultiplier + Vector3.down*bobAmount*Time.deltaTime);
        // }
         if(transform.position.y < -3.93)
         {
             RenderSettings.fogMode = FogMode.Exponential;
             RenderSettings.fogDensity = 0.1f;
             RenderSettings.fogColor = new Color(0f, 0.345f, 1f, 1f);
             waterProcessing.SetActive(true);
             //     moveDirection = (transform.forward * vertical * speedf * crouchMultiplier + transform.right * horizontal * speeds * crouchMultiplier + -Vector3.down*bobAmount*Time.deltaTime);
        }
         else
         {
             RenderSettings.fogMode = FogMode.ExponentialSquared;
             RenderSettings.fogDensity = 0.003f;
             RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 1f); 
             waterProcessing.SetActive(false);
         }
        //
        //

    }

    private void HandleJump()
    {
        if (Input.GetButton("Jump") && (isGrounded|| fly))
        {
            jumpForceVector = Vector3.up * jumpForce;
            jumpTimer = 0f;
            isJumping = true;
            isGrounded = false;
        }
        if(isJumping)
        {
            jumpTimer += Time.deltaTime;
            float forceDecreaseFactor = Mathf.Lerp(1, 0, jumpTimer / jumpDuration);
            jumpForceVector = Vector3.up * jumpForce * forceDecreaseFactor;
            moveDirection += jumpForceVector;
        }
    }
    
}
