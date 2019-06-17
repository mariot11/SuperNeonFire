using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Transform enemyDestination;
    public Transform surroundingGoal;
    public float enemyHealth = 10f;
    public float enemyDamage = 20f;
    public float timeBetweenBullets = 0.4f;
    public float shootingRange = 6f;
    public float knockbackTime = 0.2f;
    [Range(0f, 0.5f)]
    public float damageFreezeTime = 0.05f;
    [Range(0f, 0.5f)]
    public float killFreezeTime = 0.1f;
    [Range(0f, 1f)]
    public float hitTimeScale = 0f;
    [Range(0f, 1f)]
    public float killTimeScale = 0.6f;
    public Image healthBar;
    public Canvas enemyCanvas;
    public float cameraShakeTime;
    public float cameraShakeMag;
    public GameObject firePoint;
    public GameObject projectilePrefab;
    public int playerBulletLayer = 11;

    private Freezer _freezer;
    private const string damaged = "damaged";
    private float currentHealth;
    private float timeSinceLastBullet = 0f;
    private CameraShake cameraShake;
    private NavMeshAgent _navMeshAgent;
    private ParticleSystem muzzleFlash;
    private GameObject player;
    private Vector3 canvasLocalPosition;
    private Quaternion initialRotation = Quaternion.Euler(90, 0, 0);
    private Animator anim;
    private SpawnEnemiesTrigger triggerScript;

    // Start is called before the first frame update
    void Start()
    {
        _freezer = GameObject.FindWithTag("Freezer").GetComponent<Freezer>();
        anim = GetComponentInChildren<Animator>();
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        cameraShake = GameObject.FindObjectOfType<Camera>().GetComponent<CameraShake>();
        enemyDestination = player.transform;//Establecemos el jugador como el objetivo inicialmente
        currentHealth = enemyHealth;
        muzzleFlash = firePoint.GetComponent<ParticleSystem>();
        canvasLocalPosition = enemyCanvas.transform.localPosition;//guardamos la posicion inicial del canvas para corregirla más adelante
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);
        checkObstacles();
        updateUIPosition();//corregimos la posición y rotación del canvas para que no las herede del padre y se mantengan fijas a vista del jugador
        if (enemyDestination)
            _navMeshAgent.SetDestination(enemyDestination.position);
        else { enemyDestination = player.transform; }
        tryToShoot();
        timeSinceLastBullet += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "objective" && surroundingGoal == null) { //Si uno de los objetivos entra dentro del area de visión del enemigo y este aún no tiene objetivo
            print("yes");
            EnemySurroundingGoalScript objectiveOcupation = other.GetComponent<EnemySurroundingGoalScript>();
            if (!objectiveOcupation.getOccupied()) //si el objetivo no está ocupado por otro enemigo
            {
                objectiveOcupation.setOccupied(true);
                surroundingGoal = other.transform;
                enemyDestination = surroundingGoal;
                checkObstacles();//Comprueba si hay obstáculos y asigna el objetivo correspondiente
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "objective") && (enemyDestination == other.transform)) {
            surroundingGoal = null;
            other.GetComponent<EnemySurroundingGoalScript>().setOccupied(false);
            enemyDestination = player.transform;
            //_navMeshAgent.SetDestination(player.transform.position);
        }
    }

    private void checkObstacles() {
        if (surroundingGoal != null) {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position) * 4.5f, Color.yellow);
            if (Physics.Raycast(transform.position, player.transform.position - transform.position, player.GetComponent<EnemySurroundingGoalSpawner>().getRadius(), obstacleMask))
            {
                //Si hay un obstáculo entre el enemigo y el player establecemos la posición del player como nuevo destino
                enemyDestination = player.transform;
            }
            else if (_navMeshAgent.destination != surroundingGoal.transform.position) { //Si no hay obstáculo por medio y el destino no es el objetivo, lo establecemos
                enemyDestination = surroundingGoal.transform;
            }
        }

    }

    public void takeDamage(float damage) {
        currentHealth = currentHealth - damage;
        healthBar.fillAmount = currentHealth / enemyHealth;
        //_freezer.Freeze(damageFreezeTime, hitTimeScale);
        anim.SetTrigger(damaged);
        if (currentHealth <= 0)
        {
            //No funciona bien el freeze junto con el cameraShake
            cameraShake.ShakeCamera(0.3f, 0.6f);
            _freezer.Freeze(killFreezeTime, killTimeScale);
            destroyEnemy();
        }
        else {
            _freezer.Freeze(damageFreezeTime, hitTimeScale);
        }
    }

    public void setTriggerScript(SpawnEnemiesTrigger trigger) {
        triggerScript = trigger;
    }

    private void updateUIPosition() {
        enemyCanvas.transform.position = transform.position + canvasLocalPosition;
        enemyCanvas.transform.rotation = initialRotation;
    }

    private void SpawnProjectile()
    {
        GameObject newProjectile;

        if (firePoint != null)
        {
            //Se crea una instancia del proyectil
            newProjectile = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            newProjectile.GetComponent<enemyProjectileController>().setDamage(enemyDamage); //le pasamos el daño que hace al proyectil
            muzzleFlash.Play();
            timeSinceLastBullet = 0f;
        }
        else
        {
            Debug.Log("No Fire Point");
        }
    }

    private void tryToShoot() {
        if (Vector3.Distance(player.transform.position, transform.position) <= shootingRange) { // si la distancia entre el enemigo y el jugador es menor al rango de disparo
            if ((timeSinceLastBullet > timeBetweenBullets) && (!Physics.Raycast(transform.position, player.transform.position - transform.position, shootingRange, obstacleMask))) { //si el tiempo desde el último disparo ha superado al tiempo entre balas y no hay ningún obstáculo entre el enemigo y el player
                int rand = Mathf.RoundToInt(Random.Range(0, 2));
                if (rand == 1)
                {
                    SpawnProjectile();
                    StartKnockback(-transform.forward, enemyDamage/2);
                }    
            }
        }
    }

    private IEnumerator Knockback(Vector3 direction, float velocity) {
        float elapsed = 0.0f;
        float knockbackSpeed = velocity / 5;
        Vector3 initialPosition = transform.position;
        while (elapsed < knockbackTime) {
            transform.position = transform.position + direction.normalized * knockbackSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;

            yield return null;
        }

    }

    private void destroyEnemy() {
        cameraShake.ShakeCamera(cameraShakeTime, cameraShakeMag);
        triggerScript.decreaseTotalEnemies();
        //yield return new WaitUntil(cameraShake.hasEnded);
        //gameObject.SetActive(false);
        player.GetComponent<EnemySurroundingGoalSpawner>().decreaseAmountByOne();
        //reseteamos los objetivos
        enemyDestination = player.transform;
        surroundingGoal = null;
        Destroy(gameObject, 0.1f);
    }

    public void StartKnockback(Vector3 direction, float velocity) {
        StartCoroutine(Knockback(direction, velocity));
    }
}
