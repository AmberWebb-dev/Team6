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

    Color colorOrig;

    bool isShooting;
    bool playerInRange;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.Instance.GameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            playerDir = GameManager.Instance.transform.position - headPos.position;

            agent.SetDestination(GameManager.Instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(shoot());
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
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        agent.SetDestination(GameManager.Instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.Instance.GameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot.normalized, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        
    }
}
