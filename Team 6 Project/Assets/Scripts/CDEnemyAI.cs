using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CDEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform headPosition;
    [SerializeField] Transform attackPosition;

    [SerializeField] int faceTargetSpeed;
    [SerializeField] int HP;

    [SerializeField] GameObject attackObj;
    [SerializeField] float attackRate;

    [SerializeField] GameObject target;

    Color colourOriginal;

    bool isAttacking;
    bool cropInRange;

    Vector3 cropDirection;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        colourOriginal = model.material.color;
        GameManager.Instance.GameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        cropDirection = GameManager.Instance.crops.transform.position - headPosition.position;
        agent.SetDestination(GameManager.Instance.crops.transform.position);

        Debug.DrawRay(headPosition.position, cropDirection);

        RaycastHit hit;
        //if(target != null)
        if (Physics.Raycast(headPosition.position, cropDirection, out hit))
        {
            if (hit.collider.CompareTag("Crop"))
            {

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (!isAttacking && cropInRange)
                {
                    StartCoroutine(Attack());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crop"))
        {
            cropInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Crop"))
        {
            cropInRange = false;
        }
    }

    void faceTarget()
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

        Instantiate(attackObj, attackPosition.position, transform.rotation);
        yield return new WaitForSeconds(attackRate);

        isAttacking = false;
    }
}