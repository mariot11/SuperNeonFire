using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MissileController : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    public float checkPositionTime = 10f;
    public float totalTurningTime = 0.5f;
    public float speed = 1;
    public float missileDamage = 50f;
    public float cameraShakeTime;
    public float cameraShakeMag;

    private GameObject parentTurret;
    private CameraShake cameraShake;
    private const int playerLayer = 14;
    private const int turretLayer = 17;
    private const int wallsLayer = 10;
    private float timer = 0f;
    private Transform player;
    private bool leftTurret = false;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cameraShake = GameObject.FindObjectOfType<Camera>().GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position += transform.forward * (speed * Time.deltaTime);

        if (timer >= checkPositionTime)
        {
            StartCoroutine(TurnToPlayer());
            timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case turretLayer:
                if (leftTurret) {
                    other.GetComponent<TurretScript>().takeDamage(missileDamage);
                    other.GetComponent<TurretScript>().StartKnockback(transform.forward, speed);
                    destroyMissile();
                    break;
                }
                break;
            case playerLayer:
                other.GetComponent<PlayerScript>().takeDamage(missileDamage);
                destroyMissile();
                break;
            case wallsLayer:
                destroyMissile();
                break;
            default:
                break;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == turretLayer) {
            if (!leftTurret)
                leftTurret = true;
        }
    }

    private void destroyMissile() {
        parentTurret.GetComponent<TurretScript>().setCanShootTrue();

        if (hitParticlesPrefab != null)
        {
            var hitVFX = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        }
        cameraShake.ShakeCamera(cameraShakeTime, cameraShakeMag);
        Destroy(gameObject);
    }

    public void setParentTurret(GameObject turret) {
        parentTurret = turret;
    }

    IEnumerator TurnToPlayer() {
        float turningTimer = 0;
        Quaternion missileRotation = transform.rotation;
        Quaternion missileToPlayerRotation = Quaternion.LookRotation(player.position - transform.position, transform.up);
        while (turningTimer < totalTurningTime)
        {
            transform.rotation = Quaternion.Lerp(missileRotation, missileToPlayerRotation, turningTimer/totalTurningTime);
            turningTimer += Time.deltaTime;
            yield return null;
        }
            
    }
}
