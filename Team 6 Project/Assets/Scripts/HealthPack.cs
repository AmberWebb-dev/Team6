using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IHealth health = other.GetComponent<IHealth>();

        if (health != null)
        {
            health.HealUp();
            Destroy(gameObject);
        }
    }
}
