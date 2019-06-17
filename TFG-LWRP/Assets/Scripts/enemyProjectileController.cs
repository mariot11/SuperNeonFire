using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectileController : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    public float speed = 10;
    public int playerLayer = 14;
    public int playerBulletLayer = 11;

    private Vector3 startPosition;
    private float damageAmount;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer) //si se trata del player
            other.GetComponentInParent<PlayerScript>().takeDamage(damageAmount);

        //Provisional
        if (other.gameObject.layer != playerBulletLayer)
            DestroyProjectile();

        else if (other.GetComponent<ProjectileController>().getDamage()/10 >= damageAmount/10)
            DestroyProjectile();

    }

    void DestroyProjectile()
    {
        speed = 0;

        if (hitParticlesPrefab != null)
        {
            var hitVFX = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void setDamage(float damage)
    {
        damageAmount = damage;
    }

    public float getDamage()
    {
        return damageAmount;
    }
}
