using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankyEnemy : MonoBehaviour, IDamage
{
    [Header("----- Powerup Spawn Chances -----")]
    [SerializeField] private List<PowerupChance> powerupChances = new List<PowerupChance>();

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    private Color materialOrig;
    [SerializeField] Transform headPosition;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int HP;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackRate;

    [SerializeField] int scoreValue;

    [SerializeField] Animator animator;

    private GameObject currentTargetCrop;
    private Color colourOriginal;
    private bool isAttacking;
    bool isKnockedback;

    float originalSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        materialOrig = GetComponentInChildren<Renderer>().material.color;
        GameManager.Instance.GameGoal(1);
        animator = GetComponent<Animator>();

        originalSpeed = agent.speed;

        UpdateTargetCrop();
    }

    void Update()
    {
        if (isKnockedback) { return; }

        if (currentTargetCrop == null)
        {
            UpdateTargetCrop();
        }

        if (GameManager.Instance.playerScript.ContainsPowerup(PlayerController.PowerupType.Freeze))
        {
            agent.speed = originalSpeed / 4;
        }
        else
        {
            agent.speed = originalSpeed;
        }

        if (currentTargetCrop != null)
        {
            Vector3 cropDirection = currentTargetCrop.transform.position - headPosition.position;
            agent.SetDestination(currentTargetCrop.transform.position);

            animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

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
            agent.ResetPath();
            animator.SetBool("isWalking", false);
        }
    }

    void UpdateTargetCrop()
    {
        currentTargetCrop = GameManager.Instance.GetNearestCrop(transform.position);
        if (currentTargetCrop != null)
        {
        }
        else
        {
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

        animator.SetBool("isAttacking", true);

        CropDamage cropDamage = currentTargetCrop.GetComponent<CropDamage>();
        if (cropDamage != null)
        {
            cropDamage.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackRate);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
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
