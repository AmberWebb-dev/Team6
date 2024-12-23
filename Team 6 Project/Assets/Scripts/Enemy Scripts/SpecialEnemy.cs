using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpecialEnemy : MonoBehaviour, IDamage
{
    [Header("----- Powerup Spawn Chances -----")]
    [SerializeField] private List<PowerupChance> powerupChances = new List<PowerupChance>();

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int HP;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] int explosionDamage = 10;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] int roamDistance;
    [SerializeField] int roamTimer;

    [SerializeField] GameObject iceParticles;

    [SerializeField] int scoreValue;

    bool playerInRange;
    bool hasExploded = false;
    bool isRoaming;
    bool isKnockedback;

    float originalSpeed;

    Vector3 playerDir;
    Vector3 startPosition;

    float angleToPlayer;
    float stoppingDistanceOrig;

    Color ogColor;
    private AudioSource audioSource;

    [SerializeField] Material flashRedMaterial;
    private Material[] originalMaterials;
    Color materialOrig;
    [SerializeField] Animator anim;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator not found!");
        }
        materialOrig = GetComponentInChildren<Renderer>().material.color;
        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.GameGoal(1);

        originalSpeed = agent.speed;

        startPosition = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;

        anim.Rebind();
        anim.Update(0);
    }

    void Update()
    {
        bool isWalking = agent.velocity.magnitude > 0.1f;

        var clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            Debug.Log($"Current Clip: {clipInfo[0].clip.name}");
        }
        else
        {
            Debug.Log("No Clip Playing!");
        }

        // Debug Logs to check what's happening
        Debug.Log($"Animator Parameter isWalking: {anim.GetBool("isWalking")}");
        Debug.Log($"NavMesh Speed: {agent.velocity.magnitude}");

        // Update the parameter
        anim.SetBool("isWalking", isWalking);

        // Force walking animation for testing
        if (isWalking)
        {
            anim.Play("walking"); // Force walking animation
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

        if (playerInRange && !CanSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {
                StartCoroutine(Roam());
            }
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {
                StartCoroutine(Roam());
            }
        }
    }

    IEnumerator Roam()
    {
        isRoaming = true;
        anim.SetBool("isWalking", true);
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;
        Vector3 randDistance = Random.insideUnitSphere * roamDistance;
        randDistance += startPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randDistance, out hit, roamDistance, 1);
        if (!isKnockedback)
        {
            agent.SetDestination(hit.position);
        }

        isRoaming = false;
        anim.SetBool("isWalking", false);
    }
    bool CanSeePlayer()
    {
        playerDir = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit) && !isKnockedback)
        {
            // Player seen
            agent.SetDestination(GameManager.Instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }
            

            agent.stoppingDistance = stoppingDistanceOrig;
            return true;
        }

        agent.stoppingDistance = 0;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //agent.SetDestination(transform.position);
            agent.stoppingDistance = 0;
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashRed());

        if (HP <= 0 && !hasExploded)
        {
            Explode();
        }
        else
        {
            AudioManager.Instance.enemyHitSound.PlayAtPoint(transform.position);
        }
    }

    void Explode()
    {
        hasExploded = true;

        anim.SetBool("isAttacking", true);

        AudioManager.Instance.explosionSound.Play();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == gameObject) continue; 

            IDamage damageable = hit.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }

            if (hit.CompareTag("Player") && GameManager.Instance != null)
            {
                GameManager.Instance.ApplyBlindEffect(3.5f);
            }
        }

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

    IEnumerator flashRed()
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