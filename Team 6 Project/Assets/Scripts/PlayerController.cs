using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] CharacterController pController;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprint;

    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    private int jumpCount;
    int HPOriginal;

    bool isSprinting;
    bool isShooting;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        HPOriginal = HP;
        UpdatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Show the shooting distance on the scene screen
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        Movement();
        Sprint();
    }

    // Setter for jumpCount private variable
    void SetJumpCount(int jump)
    {
        jumpCount = jump;
    }

    // Getter for jumpCount private variable
    int GetJumpCount()
    { 
        return jumpCount; 
    }

    void Movement()
    {
        if(pController.isGrounded)
        {
            SetJumpCount(0);
            playerVelocity = Vector3.zero;
        }

        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));

        pController.Move(moveDirection * speed * Time.deltaTime);


        Jump();

        pController.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if(Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(Shoot());
        }
    }
    
    void Jump()
    {
        if(Input.GetButtonDown("Jump") && GetJumpCount() < jumpMax)
        {
            SetJumpCount(GetJumpCount() + 1);
            playerVelocity.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprint;
            isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprint;
            isSprinting = false;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            // Show what the player is shooting on the Debug Log
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            // If damage is not null set damage amount to shoot damage
            if(dmg != null )
            {
                dmg.TakeDamage(shootDamage);
            }
        }

        // Wait for duration of the shooting rate and set isShooting back to false
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();
        StartCoroutine(FlashDamage());

        // Player is killed
        if(HP <= 0)
        {
            GameManager.Instance.YouLose();
        }
    }

    public void UpdatePlayerUI()
    {
        GameManager.Instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    }

    IEnumerator FlashDamage()
    {
        GameManager.Instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.playerDamageScreen.SetActive(false);
    }
}
