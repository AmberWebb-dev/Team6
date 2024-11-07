using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CDEnemy : MonoBehaviour
{
    //setting up basic things here, will make the methods soon.
    bool isShooting;
    bool targetInRange;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] GameObject bullet;
    
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] float shootRate;
    [SerializeField] float attackRange;

    [SerializeField] float stoppingDis = 2f;

    Color colorOrig;
    public GameObject targetCrop;

    Vector3 targetDir;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDis;
        GameManager.Instance.GameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(targetCrop != null)
        {
            agent.SetDestination(targetCrop.transform.position);

            if (agent.remainingDistance <= stoppingDis)
            {
                targetDir = (targetCrop.transform.position - transform.position).normalized;

<<<<<<< HEAD:Team 6 Project/Assets/Scripts/RangeEnemyAi.cs
                // Call faceTarget only if within stopping distance
                faceTarget();

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
=======
        if (targetInRange && !isShooting)
        {
            StartCoroutine(shoot() );
>>>>>>> parent of 7fd9c49 (deleted unused scripts (made by me)):Team 6 Project/Assets/Scripts/CDEnemy.cs
        }
    }


    //we are doing the range

    //private void onTriggerEnter(Collider other)
    //{
    //    if(other.gameObject == targetCrop)
    //    {
    //        targetInRange = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.CompareTag("Player"))
    //    {
    //        targetInRange = false;
    //    }
    //}

    //dmg method
    public void takeDamage(int amount)
    {
        HP -= amount;
        //color change here

        if(HP <=0)
        {
            GameManager.Instance.GameGoal(-1);
            Destroy(gameObject);
        }
    }

    //shooting
    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, Quaternion.LookRotation(targetDir));

        yield return new WaitForSeconds(shootRate);
        isShooting=false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
