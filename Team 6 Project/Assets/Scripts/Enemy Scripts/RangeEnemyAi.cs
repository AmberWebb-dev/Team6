using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemyAi : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] int roamDistance;
    [SerializeField] int roamTimer;

    [SerializeField] int scoreValue;

    Color colorOrig;

    bool isShooting;
    bool playerInRange;
    bool isRoaming;
    bool isKnockedback;

    Vector3 playerDir;
    Vector3 startPosition;

    float stoppingDistanceOrig;
    float angleToPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        colorOrig = model.material.color;
        GameManager.Instance.GameGoal(1);
        startPosition = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {

        if(playerInRange && !CanSeePlayer())
        {
            if(!isRoaming && agent.remainingDistance < 0.05f)
            {
                StartCoroutine(Roam());
            }
        }
        else if(!playerInRange)
        {
            if(!isRoaming && agent.remainingDistance < 0.05f)
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
        playerDir = GameManager.Instance.player.transform .position - headPos.position; 
        angleToPlayer = Vector3.Angle(playerDir, transform.position);

        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir, out hit) && !isKnockedback)
        {
            // Player seen
            agent.SetDestination(GameManager.Instance.player.transform.position);

            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }
            if(!isShooting)
            {
                StartCoroutine(Shoot());
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
            agent.stoppingDistance = 0;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        // Chases player if shot out of range
        if (!isKnockedback)
        {
            agent.SetDestination(GameManager.Instance.player.transform.position);
        }

        if(HP <= 0)
        {
            GameManager.Instance.GameGoal(-1);
            GameManager.Instance.enemyScoreTotal += scoreValue;
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot.normalized, Time.deltaTime * faceTargetSpeed);
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
