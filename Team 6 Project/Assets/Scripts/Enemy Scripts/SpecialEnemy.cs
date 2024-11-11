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
    bool hasExploded = false; // New flag to prevent multiple explosions
    Vector3 playerDir;
    float angleToPlayer;

    Color ogColor;
    private AudioSource audioSource;

    void Start()
    {
        ogColor = model.material.color;
        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.GameGoal(1);
    }

    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(GameManager.Instance.player.transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
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

    void FaceTarget()
    {
        playerDir = GameManager.Instance.player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0 && !hasExploded) // Ensure explosion happens only once
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
        Destroy(gameObject); 
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ogColor;
    }
}
