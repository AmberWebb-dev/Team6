// SpecialEnemy.cs
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

    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;

    Color ogColor;
    private AudioSource audioSource;

    void Start()
    {
        ogColor = model.material.color;
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (playerInRange)
        {
            // The enemy chases the player directly
            agent.SetDestination(GameManager.Instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
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
            agent.SetDestination(transform.position);
        }
    }

    void faceTarget()
    {
        playerDir = GameManager.Instance.player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        audioSource.volume = 0.4f;
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            IDamage damageable = hit.GetComponent<IDamage>();

            if (damageable != null && hit.gameObject != gameObject)
            {
                damageable.TakeDamage(explosionDamage);
            }
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player") && GameManager.Instance != null)
            {
                GameManager.Instance.StartCoroutine(GameManager.Instance.ApplyBlindEffect());
            }
        }
        StartCoroutine(Death());
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ogColor;
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}

/*// SpecialEnemy.cs
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

    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;

    Color ogColor;
    private AudioSource audioSource;

    void Start()
    {
        ogColor = model.material.color;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerInRange && canSeePlayer())
        {
            // The enemy chases the player
            agent.SetDestination(GameManager.Instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir, Color.yellow);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir.normalized, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                return true;
            }
        }

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
            agent.SetDestination(transform.position);
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        audioSource.volume = 0.4f;
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            IDamage damageable = hit.GetComponent<IDamage>();

            if (damageable != null && hit.gameObject != gameObject)
            {
                damageable.TakeDamage(explosionDamage);
            }
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player") && GameManager.Instance != null)
            {
                GameManager.Instance.StartCoroutine(GameManager.Instance.ApplyBlindEffect());
            }
        }
        StartCoroutine(Death());
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ogColor;
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}*/