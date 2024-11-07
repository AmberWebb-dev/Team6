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
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            agent.SetDestination(target.transform.position);
            cropDirection = (target.transform.position - transform.position).normalized;

            if(!isAttacking && cropInRange)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crops"))
        {
            cropInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Crops"))
        {
            cropInRange = false;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
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