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
        // Update game goal
        GameManager.Instance.GameGoal(1);
    }
    void Update()
    {
        if (playerInRange)
        {
            // The enemy chases the player directly
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
            // sorry had to commment this one out, kept crashing the game T_T
            //if (hit.gameObject.layer == LayerMask.NameToLayer("Player") && GameManager.Instance != null)
           // {
               // GameManager.Instance.StartCoroutine(GameManager.Instance.ApplyBlindEffect());
           // }
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
    /*IEnumerator Death()
    {

        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.GameGoal(-1);

        Destroy(gameObject);
    }*/
}

