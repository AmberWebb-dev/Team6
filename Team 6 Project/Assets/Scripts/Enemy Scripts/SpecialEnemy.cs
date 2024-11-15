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

    Vector3 playerDir;
    Vector3 startPosition;

    float angleToPlayer;
    float stoppingDistanceOrig;

    Color ogColor;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ogColor = model.material.color;
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
        agent.SetDestination(hit.position);

        isRoaming = false;
    }
    bool CanSeePlayer()
    {
        playerDir = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
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

        StartCoroutine(FlashRed());

        if (HP <= 0 && !hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        if (audioSource != null && explosionSound != null)
        {
            audioSource.volume = 0.4f;
            audioSource.PlayOneShot(explosionSound);
        }

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

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ogColor;
    }
}