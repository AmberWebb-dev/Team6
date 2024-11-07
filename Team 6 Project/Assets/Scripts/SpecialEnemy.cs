// SpecialEnemy.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemy : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] int explosionDamage = 10;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] AudioClip explosionSound;

    Color ogColor;
    private AudioSource audioSource;

    void Start()
    {
        ogColor = model.material.color;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

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
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player") && GameManager.Instance != null)
            {
                GameManager.Instance.StartCoroutine(GameManager.Instance.ApplyBlindEffect());
            }
        }
        StartCoroutine(Death());
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ogColor;
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
