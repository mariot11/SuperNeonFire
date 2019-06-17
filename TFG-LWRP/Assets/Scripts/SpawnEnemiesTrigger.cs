using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesTrigger : MonoBehaviour
{
    public GameObject enemySpawner;
    public GameObject[] turrets;
    public Animator[] doorsAnims;
    public int enemyAmount = 4;
    public float radius = 3f;

    private bool firstTime = true;
    private const string isOpen = "isOpen";
    private int totalEnemies;

    private void Start()
    {
        totalEnemies = enemyAmount + turrets.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { //Si es el jugador quien entra en el trigger
            
            if (firstTime)//Solo debe poner a true la variable canShoot la primera vez que entre en el trigger
            {
                //pasamos los atributos correspondientes al spawner para que cree los enemigos y al player para que cree los objetivos
                CreateObjectsAround enemySpawnerScript = enemySpawner.GetComponent<CreateObjectsAround>();
                EnemySurroundingGoalSpawner GoalSpawnerScript = other.GetComponent<EnemySurroundingGoalSpawner>();
                enemySpawnerScript.setAtributes(enemyAmount, radius, this);
                GoalSpawnerScript.setAtributes(enemyAmount, radius, this);
                enemySpawnerScript.InstantiateObjects();
                GoalSpawnerScript.InstantiateObjects();

                foreach (GameObject turret in turrets)
                {
                    turret.GetComponent<TurretScript>().setCanShootTrue();
                }

                foreach (Animator anim in doorsAnims) {
                    anim.SetBool(isOpen, false);
                }
                firstTime = false;
            }
        }
    }

    public void decreaseTotalEnemies() {
        totalEnemies--;

        if (totalEnemies == 0) {
            foreach (Animator anim in doorsAnims)
            {
                anim.SetBool(isOpen, true);
            }
        }
    }
}
