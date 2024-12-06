using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpecialEnemy : MonoBehaviour, IDamage
{
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

    [SerializeField] int scoreValue;

    bool playerInRange;
    bool hasExploded = false;
    bool isRoaming;
    bool isKnockedback;

    Vector3 playerDir;
    Vector3 startPosition;

    float angleToPlayer;
    float stoppingDistanceOrig;

    Color ogColor;
    private AudioSource audioSource;

    [SerializeField] Material flashRedMaterial;
    private Material[] originalMaterials;
    Color materialOrig;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        materialOrig = model.material.color;
        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.GameGoal(1);
        startPosition = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
    }

    void Update()
    {
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
                GameManager.Instance.StartCoroutine(GameManager.Instance.ApplyBlindEffect());
            }
        }

        GameManager.Instance.GameGoal(-1);
        GameManager.Instance.enemyScoreTotal += scoreValue;
        Destroy(gameObject); 
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = materialOrig;
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