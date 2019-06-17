using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjectsAround : MonoBehaviour
{
    public GameObject objectToInstantiate;

    protected SphereCollider SCollider;
    protected bool objectsInstantiated = false;
    protected float radius = 0;
    protected int objectsAmount = 0;
    protected GameObject[] objectsArray;
    protected SpawnEnemiesTrigger triggerScript;

    private void Awake()
    {
        SCollider = objectToInstantiate.GetComponent<SphereCollider>();
    }

    public GameObject[] GetObjectsArray() {
        return objectsArray;
    }

    public virtual void InstantiateObjects() {
        if((objectsAmount > 0) && (!objectsInstantiated)) //Si la cantidad de objetos es mayor que 0 y los objetos aún no han sido instanciados
        {
            objectsArray = new GameObject[objectsAmount];
            objectsInstantiated = true; //actualizamos el estado de los objetos
            float degree = 360 / objectsAmount;

            for (int i = 0; i < objectsAmount; i++) {
                objectsArray[i] = Instantiate(objectToInstantiate, new Vector3 (transform.position.x, transform.position.y, transform.position.z + radius), Quaternion.identity);
                if (objectsArray[i].GetComponent<EnemyScript>()) {
                    objectsArray[i].GetComponent<EnemyScript>().setTriggerScript(triggerScript);
                }
                objectsArray[i].transform.RotateAround(transform.position, transform.up, degree * i);
            }
        } 
    }

    public virtual void setAtributes(int pointsAmount, float radius, SpawnEnemiesTrigger trigger) {
        if (!objectsInstantiated)
        {
            triggerScript = trigger;
            objectsAmount = pointsAmount;
            this.radius = radius;

            if (SCollider)
                SCollider.radius = 2 * radius;
            else
                Debug.Log("El objeto necesita un Sphere Collider");
        }
    }

    public float getRadius() {
        return radius;
    }

}
