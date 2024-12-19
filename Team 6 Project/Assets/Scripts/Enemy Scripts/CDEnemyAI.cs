// CDEnemyAI.cs
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CDEnemyAI : MonoBehaviour, IDamage, IKnockback
{
    [Header("----- Powerup Spawn Chances -----")]
    [SerializeField] private List<PowerupChance> powerupChances = new List<PowerupChance>();

    [SerializeField] Renderer model;
    [SerializeField] public NavMeshAgent agent;

    [SerializeField] Transform headPosition;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] public int HP;
    [SerializeField] public int attackDamage = 10;
    [SerializeField] float attackRate;

    [SerializeField] GameObject iceParticles;

    [SerializeField] int scoreValue;

    private GameObject currentTargetCrop;
    private Color colourOriginal;
    private bool isAttacking;
    private bool isKnockedback;
    private bool inRange;

    private float originalSpeed;

    [SerializeField] Material flashRedMaterial;
    private Material[] originalMaterials;
    Color materialOrig;

    [SerializeField] Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        materialOrig = GetComponentInChildren<Renderer>().material.color;
        //originalMaterials = model.materials;
        GameManager.Instance.GameGoal(1);

        originalSpeed = agent.speed;

        // Set initial target crop
        UpdateTargetCrop();
    }

    void Update()
    {
        if (isKnockedback) { return; }

        if (!isAttacking && agent.remainingDistance > agent.stoppingDistance && agent.velocity.magnitude > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (currentTargetCrop == null)
        {
            UpdateTargetCrop();
        }

        if (GameManager.Instance.playerScript.ContainsPowerup(PlayerController.PowerupType.Freeze))
        {
            iceParticles.SetActive(true);
            agent.speed = 0;
        }
        else
        {
            iceParticles.SetActive(false);
            agent.speed = originalSpeed;
        }

        if (currentTargetCrop != null)
        {
            Vector3 cropDirection = currentTargetCrop.transform.position - headPosition.position;
            agent.SetDestination(currentTargetCrop.transform.position);


            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget(cropDirection);
            }

            if (!isAttacking && Vector3.Distance(transform.position, currentTargetCrop.transform.position) <= agent.stoppingDistance)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            agent.ResetPath(); // Stop moving if no target is available
            anim.SetBool("isWalking", false);
        }

    }

    void UpdateTargetCrop()
    {
        currentTargetCrop = GameManager.Instance.GetNearestCrop(transform.position);
        if (currentTargetCrop != null)
        {
            //Debug.Log("New target crop assigned: " + currentTargetCrop.name);
        }
        else
        {
            //Debug.Log("No crops left to target.");
            // no More crops, you lose
            GameManager.Instance.YouLose();
        }
    }

    void FaceTarget(Vector3 cropDirection)
    {
        Quaternion rot = Quaternion.LookRotation(cropDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            AudioManager.Instance.enemyDeathSound.PlayAtPoint(transform.position);
            GameManager.Instance.GameGoal(-1);
            GameManager.Instance.enemyScoreTotal += scoreValue;

            // Roll powerup chances
            for (int i = 0; i < powerupChances.Count; i++)
            {
                if (powerupChances[i].RollChance())
                {
                    powerupChances[i].SpawnPowerup(transform.position);
                    break;
                }
            }

            Destroy(gameObject);
        }
        else
        {
            AudioManager.Instance.enemyHitSound.PlayAtPoint(transform.position);
        }
    }

    IEnumerator FlashRed()
    {
        // Get the SkinnedMeshRenderer dynamically
        SkinnedMeshRenderer visibleModel = GetComponentInChildren<SkinnedMeshRenderer>();
        if (visibleModel != null)
        {
            // Save original color
            Material material = visibleModel.material;
            Color originalColor = material.color;

            // Flash red
            material.color = Color.red;

            // Wait for the flash duration
            yield return new WaitForSeconds(0.1f);

            // Restore the original color
            material.color = originalColor;
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", true);

        CropDamage cropDamage = currentTargetCrop.GetComponent<CropDamage>();
        if (cropDamage != null)
        {
            cropDamage.TakeDamage(attackDamage);
            Debug.Log("Crop took " + attackDamage + " damage.");
        }
        else
        {
            Debug.LogError("No CropDamage script found on target crop!");
        }

        yield return new WaitForSeconds(attackRate);
        anim.SetBool("isAttacking", false);
        isAttacking = false;
        Debug.Log("Attack completed.");
    }

    public void Knockback(Vector3 direction, float strength, float time)
    {
        StartCoroutine(KnockbackAnimation(direction, strength, time));
    }

    IEnumerator KnockbackAnimation(Vector3 direction, float strength, float time)
    {
        isKnockedback = true;

        float angleSpeedOriginal = agent.angularSpeed;
        agent.angularSpeed = 0;

        float accelerationOriginal = agent.acceleration;
        agent.acceleration = 999;

        Vector3 originalDestination = agent.destination;
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position - direction * strength);
        }

        yield return new WaitForSeconds(time);

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(originalDestination);
        }
        agent.acceleration = accelerationOriginal;
        agent.angularSpeed = angleSpeedOriginal;
        isKnockedback = false;
    }
}
