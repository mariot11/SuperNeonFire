using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerScript : MonoBehaviour
{
    public float health_ammo_max = 100f;
    public float accPower; //poder de aceleración
    public Image healthBar;
    public Image chargeCircle;
    //public Color colorMin = Color.yellow;
    //public Color colorMax = Color.red;

    private float health_ammo_min;
    private float health_ammo;
    private float health_ammo_beforeClick;
    private Rigidbody rigidBody;
    private float accInputX;
    private float accInputY;
    private float camRayLength = 100f;
    private int floorMask;
    const string floor = "Floor";

    [Header("Shooting Variables")]
    public CameraShake cameraShake;
    public GameObject firePoint;
    public GameObject chargeParticles;
    public GameObject projectilePrefab;
    public float totalWaitTime = 0.8f;//tiempo de espera hasta que empiede a recargarse la vida
    public int dropRate = 30; //velocidad a la que baja la barra cuando cargamos un disparo
    public int initialRiseRate = 15; //velocidad a la que sube inicialmente la barra cuando cargamos un disparo
    public int maxRiseRate = 30; //velocidad máxima a la que sube la barra cuando cargamos un disparo
    public float impulseRatio = 3f;
    public float chargeAnimationStartTime = 0.5f;//tiempo de espera para empeza
    [Range(0f, 0.5f)]
    public float deathFreezeTime = 0.8f;
    [Range(0f, 1f)]
    public float deathTimeScale = 0.5f;
    public Animator cameraAnim;

    private ParticleSystem muzzleFlash; //efecto flash que se ve al disparar un arma
    private ParticleSystem charge;
    private PostProcessingAnimation ppAnimationScript;
    private Animator anim;
    private Freezer _freezer;
    private const string damaged = "damaged";
    private const string playerDead = "playerDead";
    private bool canShoot = true;
    private bool mouseUpBool = false;
    private float health_ammo_used = 0f;
    private float chargeTime = 0f;
    private float cameraShakeTimeMin = 0.2f;
    private float cameraShakeTimeMax = 0.55f;
    private float cameraShakeMagMin = 0.1f;
    private float cameraShakeMagMax = 0.65f;
    private float oneShot_health_ammo = 1f;
    private float impulseForce = 0f;
    private float currentRiseRate = 15; //velocidad a la que sube inicialmente la barra cuando cargamos un disparo
    private float currentWaitTime = 0f; //tiempo durante el cual la barra baja o se regenera más lentamente

    //audio
    public AudioSource chargeSound;
    public AudioSource shootSound;
    public AudioLowPassFilter filter;
    public int bassMin = 5000;
    public int bassmax = 8000;
    public float shootVolumeMin = 0.2f;
    public float shootVolumeMax = 0.6f;

    private void Awake()
    {
        _freezer = GameObject.FindWithTag("Freezer").GetComponent<Freezer>();
        floorMask = LayerMask.GetMask(floor);
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        health_ammo = health_ammo_max;
        currentRiseRate = initialRiseRate;
        health_ammo_min = oneShot_health_ammo;
        muzzleFlash = firePoint.GetComponent<ParticleSystem>();
        charge = chargeParticles.GetComponent<ParticleSystem>();
        ppAnimationScript = FindObjectOfType<Camera>().GetComponent<PostProcessingAnimation>();
        Time.timeScale = 1f;
    }

    void Update()
    {
        //obtener el Input de cada eje (1 o -1) para aplicar la fuerza en una determinada dirección y sentido
        accInputY = Input.GetAxisRaw("Vertical"); //Aceleración vertical
        accInputX = Input.GetAxisRaw("Horizontal"); //Aceleración horizontal

        if (Time.timeScale > 0) {

            Turning();

            if (Input.GetMouseButtonDown(0)) {
                //activar efecto de sonido de cargar disparo
                chargeSound.Play();

                health_ammo_beforeClick = health_ammo; //guardamos el valor que tenía la barra antes de pulsar
                //restamos X valor constante de vida/municion por cada click que se sumará al tiempo que esté cargado el disparo, siempre y cuando quede suficiente vida/munición
                if (health_ammo > oneShot_health_ammo + health_ammo_min)
                {
                    health_ammo -= oneShot_health_ammo;
                    health_ammo_used = oneShot_health_ammo;
                }
                
                else canShoot = false;

                currentWaitTime = 0f; //reiniciar el tiempo de espera para regenerar la barra
                currentRiseRate = initialRiseRate; //reiniciar el ratio de subida de la barra
            }

            if (Input.GetMouseButton(0))
            {
                if ((health_ammo > health_ammo_min) && (canShoot == true))
                {
                    health_ammo = Mathf.Max(health_ammo - dropRate * Time.deltaTime, health_ammo_min);
                    //hacer la resta health_ammo_beforeClick - health_ammo para saber cuanto ha quitado
                    health_ammo_used = Mathf.Min(health_ammo_used + dropRate * Time.deltaTime, health_ammo_max - health_ammo_min);
                    //health_ammo_used = health_ammo_beforeClick - health_ammo;    
                    chargeTime += Time.deltaTime;
                    //print(health_ammo);
                    if (!charge.isPlaying && chargeTime > chargeAnimationStartTime)
                    {
                        charge.Play(false); //pasamos false para que no reproduzca automáticamente los sistemas de partículas hijos
                    }
                }
                else if(health_ammo <= health_ammo_min){ //si se mantiene pulsado pero ya no se puede cargar más, para la animación de partículas
                    charge.Stop();
                    //parar efecto de sonido de cargar 
                    chargeSound.Stop();
                }

            }
            //Disparar proyectil al levantar el click
            else if (Input.GetMouseButtonUp(0))
            {
                //parar efecto de sonido de cargar 
                chargeSound.Stop();

                if (canShoot == true) {
                    charge.Stop();
                    chargeTime = 0f;                
                    impulseForce = Mathf.Pow(health_ammo_used / 10, impulseRatio) * 2f;
                    SpawnProjectile(health_ammo_used);
                    //empezar la corrutina que controla el screenshake, el tiempo y la magnitud lo calculamos con un Lerp según la potencia del disparo
                    float cameraShakeTimeReal = Mathf.Lerp(cameraShakeTimeMin, cameraShakeTimeMax, health_ammo_used / health_ammo_max);
                    float cameraShakeMagReal = Mathf.Lerp(cameraShakeMagMin, cameraShakeMagMax, health_ammo_used / health_ammo_max);
                    cameraShake.ShakeCamera(cameraShakeTimeReal, cameraShakeMagReal);
                    //print(cameraShakeTimeReal + ": " + cameraShakeMagReal);
                    muzzleFlash.Play();
                    health_ammo_used = 0f;//reiniciar el valor de la variable una vez usado
                }
                mouseUpBool = true;

            }
            else {
                //regenerar vida
                if (health_ammo < health_ammo_max){
                    currentWaitTime += Time.deltaTime;
                    if (currentWaitTime > totalWaitTime) {
                        health_ammo = Mathf.Min(health_ammo + currentRiseRate * Time.deltaTime, health_ammo_max);
                        currentRiseRate = Mathf.Min(currentRiseRate + 2 * Time.deltaTime, health_ammo_max);
                        if (health_ammo > oneShot_health_ammo + health_ammo_min)
                        {
                            canShoot = true;
                        }
                        //print("currentRiseRate: " + currentRiseRate);
                        //print(health_ammo);
                    }               
                }
            }

            updateUI();

        }
        

    }

    private void FixedUpdate()
    {
        //Guardamos los valores del input en un vector para normalizarlo
        Vector2 accInputVector = new Vector2(accInputX, accInputY);
        //Normalizamos el vector para que la magnitud del resultado de la suma de las fuerzas en X e Y sea la misma
        accInputVector = accInputVector.normalized;
        
        //aplicamos la fuerza según la dirección y el sentido para mover el personaje 
        rigidBody.AddForce(Vector3.forward * accInputVector.y * accPower, ForceMode.Force);
        rigidBody.AddForce(Vector3.right * accInputVector.x * accPower, ForceMode.Force);

        //Aplicar la fuerza cuando se levanta el ratón
        if (mouseUpBool == true) {
            mouseUpBool = false;
            rigidBody.AddForce(firePoint.transform.forward * -1 * impulseForce, ForceMode.Impulse);
            impulseForce = 0f;
        }
    }

    private void Turning() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            transform.rotation = newRotation;
            //rigidBody.MoveRotation(newRotation);
        }
    }

    private void updateUI() {
        healthBar.fillAmount = health_ammo / health_ammo_max; //actualizar la barra de vidad del UI
        chargeCircle.fillAmount = Mathf.Lerp(0, 1, health_ammo_used / health_ammo_max);
        //chargeCircle.color = Color.Lerp(colorMin, colorMax, health_ammo_used / health_ammo_max); //actualizar el color del círculo de carga
    }

    private void SpawnProjectile(float damageAmount)
    {
        GameObject newProjectile;

        if (firePoint != null)
        {
            //Se reproduce el sonido
            shootSound.volume = Mathf.Lerp(shootVolumeMin, shootVolumeMax, damageAmount / health_ammo_max);
            filter.cutoffFrequency = Mathf.Lerp(bassMin, bassmax, damageAmount/health_ammo_max);
            shootSound.Play();
            //Se crea una instancia del proyectil
            newProjectile = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            newProjectile.GetComponent<ProjectileController>().setDamage(damageAmount); //le pasamos el daño que hace al proyectil
        }
        else
        {
            Debug.Log("No Fire Point");
        }
    }

    public void takeDamage(float damage)
    {
        StartCoroutine(ppAnimationScript.postProcessAnimation());
        health_ammo = health_ammo - damage;
        anim.SetTrigger(damaged);
        healthBar.fillAmount = health_ammo / health_ammo_max;
        if (health_ammo <= 0)
        {
            die();
        }
    }

    private void die() {
        cameraAnim.SetTrigger(playerDead);
        _freezer.Freeze(deathFreezeTime, deathTimeScale);
        //Destroy(gameObject);
    }

}
