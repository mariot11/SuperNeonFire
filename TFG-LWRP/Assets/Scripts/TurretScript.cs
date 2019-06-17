using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretScript : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject firePoint;
    public SpawnEnemiesTrigger trigger;
    public float timeToShoot = 0.5f;
    public float cameraShakeTime;
    public float cameraShakeMag;
    [Range(0f, 0.5f)]
    public float damageFreezeTime = 0.05f;
    [Range(0f, 0.5f)]
    public float killFreezeTime = 0.1f;
    [Range(0f, 1f)]
    public float hitTimeScale = 0f;
    [Range(0f, 1f)]
    public float killTimeScale = 0.6f;
    public float turretHealth = 100f;
    public float knockbackTime = 0.2f;
    public Image healthBar;
    public Canvas turretCanvas;


    private Freezer _freezer;
    private const string damaged = "damaged";
    private const string shoot = "shoot";
    private GameObject currentMissile;
    private CameraShake cameraShake;
    private Transform player;
    private bool canShoot = false;
    private float timer = 0f;
    private float currentHealth;
    //private Quaternion canvasInitialRotation = Quaternion.Euler(90, 0, 0);
    //private Vector3 canvasLocalPosition;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _freezer = GameObject.FindWithTag("Freezer").GetComponent<Freezer>();
        anim = GetComponentInChildren<Animator>();
        cameraShake = GameObject.FindObjectOfType<Camera>().GetComponent<CameraShake>();
        currentHealth = turretHealth;
        //canvasLocalPosition = turretCanvas.transform.localPosition;//guardamos la posicion inicial del canvas para corregirla más adelante
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        //updateUIPosition();

        if (canShoot) {
            if (timer > timeToShoot)
            {
                shootMissile();
                //resetear el timer y  el bool
                timer = 0f;
                canShoot = false;
            }
            else {
                timer += Time.deltaTime;
            }
        }
    }

    public void setCanShootTrue() {
        canShoot = true;
    }

    //private void updateUIPosition()
    //{
    //    turretCanvas.transform.position = transform.position + canvasLocalPosition;
    //    turretCanvas.transform.rotation = canvasInitialRotation;
    //}

    private void shootMissile() {
        anim.SetTrigger(shoot);
        currentMissile = Instantiate(missilePrefab, firePoint.transform.position, firePoint.transform.rotation);
        currentMissile.GetComponent<MissileController>().setParentTurret(this.gameObject);
    }

    public void takeDamage(float damage)
    {
        currentHealth = currentHealth - damage;
        healthBar.fillAmount = currentHealth / turretHealth;
        //_freezer.Freeze(damageFreezeTime, hitTimeScale);
        anim.SetTrigger(damaged);
        if (currentHealth <= 0)
        {
            //No funciona bien el freeze junto con el cameraShake
            cameraShake.ShakeCamera(cameraShakeTime, cameraShakeMag);
            _freezer.Freeze(killFreezeTime, killTimeScale);
            destroyEnemy();
        }
        else {
            _freezer.Freeze(damageFreezeTime, hitTimeScale);
        }
    }

    private IEnumerator Knockback(Vector3 direction, float velocity)
    {
        float elapsed = 0.0f;
        float knockbackSpeed = velocity / 5;
        Vector3 initialPosition = transform.position;
        while (elapsed < knockbackTime)
        {
            if (elapsed < knockbackTime / 2)
                transform.position = transform.position + direction.normalized * knockbackSpeed * Time.deltaTime;
            else transform.position = transform.position - direction.normalized * knockbackSpeed * Time.deltaTime;


            elapsed += Time.deltaTime;

            yield return null;
        }

    }

    private void destroyEnemy()
    {
        //Fyield return new WaitUntil(cameraShake.hasEnded);
        trigger.decreaseTotalEnemies();
        cameraShake.ShakeCamera(cameraShakeTime, cameraShakeMag);
        Destroy(transform.parent.gameObject, 0.1f);
    }

    public void StartKnockback(Vector3 direction, float velocity)
    {
        StartCoroutine(Knockback(direction, velocity));
    }
}
