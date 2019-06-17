using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    public float fireRange;
    public float speed = 10;
    public float minProjectileSize = 10;
    public float maxProjectileSize = 30;
    public int enemyLayer = 13;
    public int enemyBulletLayer = 16;
    public int switchLayer = 20;

    private Vector3 startPosition;
    private float damageAmount;
    private float damageMax = 100;
    private ParticleSystem ps;
    private TrailRenderer ts;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        ps = GetComponentInChildren<ParticleSystem>();
        ts = GetComponentInChildren<TrailRenderer>();
        setProjectileData();
    }

    // Update is called once per frame
    void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);

            if ((transform.position - startPosition).magnitude > fireRange) {
                DestroyProjectile();
            }
        }
        else {
            Debug.Log("No Speed");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ContactPoint contact = other.contacts[0];
        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        //Vector3 pos = contact.point;

        if (other.gameObject.layer == enemyLayer)
        { //si se trata de un enemigo
            EnemyScript enemyScript = other.gameObject.GetComponentInParent<EnemyScript>();
            enemyScript.takeDamage(damageAmount);
            enemyScript.StartKnockback(transform.forward, speed);
            
        }
        //provisional
        if (other.gameObject.layer != enemyBulletLayer)
        {
            if (other.gameObject.layer == switchLayer)
                other.GetComponent<SwitchScript>().setActive();
            DestroyProjectile();
        }

        else if (other.gameObject.GetComponent<enemyProjectileController>().getDamage() / 10 >= damageAmount / 10)
            DestroyProjectile();
    }

    private void DestroyProjectile() {
        speed = 0;

        if(hitParticlesPrefab != null){
            var hitVFX = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        }
            
        Destroy(gameObject);
    }

    public void setDamage(float damage) {
        damageAmount = damage;
    }

    public float getDamage() {
        return damageAmount;
    }

    public Vector3 getForward() {
        return transform.TransformDirection(transform.forward);
    }

    private void setProjectileData()
    {
        ParticleSystem.MainModule main = ps.main;
        //hacer la regla de tres para calcular el tamaño del proyectil correspondiente según la cantidad de daño
        float size = (damageAmount * (maxProjectileSize - minProjectileSize) / damageMax) + minProjectileSize;
        main.startSize = size;
        //prueba
        speed = size;
        fireRange = size;
        //print(damageAmount);
        //sacar la proporción respecto al tamaño base para aplicarla al ampliar otros elementos
        float proportion = size / minProjectileSize;
        //aumentar el tamaño del trail respectivamente
        ts.startWidth = ts.startWidth * proportion;
        //aumentar el tamaño del collider
        transform.localScale = transform.localScale * proportion;
    }
}
