using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] float packRespawnTimer;

    Vector3 origPos;
    
    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IHealth health = other.GetComponent<IHealth>();

        if (health != null)
        {
            health.HealUp();
            AudioManager.Instance.healSound.Play();
            // Move health pack off the map
            Vector3 gonePos = new Vector3(0, -10, 0);
            gameObject.transform.position = gonePos;
            StartCoroutine(RespawnHealthPack());
        }
    }

    IEnumerator RespawnHealthPack()
    {
        yield return new WaitForSeconds(packRespawnTimer);
        // Return pack back to original position
        gameObject.transform.position = origPos;
    }
}
