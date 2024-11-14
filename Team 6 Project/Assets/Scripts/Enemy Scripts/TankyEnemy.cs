using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankyEnemy : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform headPosition;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int HP;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackRate;

    [SerializeField] int scoreValue;

    private GameObject currentTargetCrop;
    private Color colourOriginal;
    private bool isAttacking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        colourOriginal = model.material.color;
        GameManager.Instance.GameGoal(1);

        UpdateTargetCrop();
    }

    void Update()
    {
        if (currentTargetCrop == null)
        {
            UpdateTargetCrop();
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
            agent.ResetPath();
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
            GameManager.Instance.GameGoal(-1);
            GameManager.Instance.enemyScoreTotal += scoreValue;
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colourOriginal;
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        CropDamage cropDamage = currentTargetCrop.GetComponent<CropDamage>();
        if (cropDamage != null)
        {
            cropDamage.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }
}
