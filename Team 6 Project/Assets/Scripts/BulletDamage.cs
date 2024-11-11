using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{

    enum DamageType { bullet, stationary };
    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int bulletSpeed;
    [SerializeField] int destroyTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (type == DamageType.bullet)
        {
            rb.velocity = transform.forward * bulletSpeed;
            Destroy(gameObject, destroyTimer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }

        if (type == DamageType.bullet)
        {
            Destroy(gameObject);
        }

    }
}