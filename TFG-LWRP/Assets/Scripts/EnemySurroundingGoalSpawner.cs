using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySurroundingGoalSpawner : CreateObjectsAround
{
    private Vector3[] localPositions;

    private void Awake()
    {
        SCollider = objectToInstantiate.GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        updatePositions();
    }

    private void updatePositions()
    {
        if (objectsInstantiated) { 
            for (int i = 0; i < objectsAmount; i++)
            {
                objectsArray[i].transform.position = transform.position + localPositions[i];
            }
        }
    }

    public override void InstantiateObjects()
    {
        if (objectsAmount != 0)
        {
            objectsArray = new GameObject[objectsAmount];
            localPositions = new Vector3[objectsAmount];
            objectsInstantiated = true;
            float degree = 360 / objectsAmount;

            for (int i = 0; i < objectsAmount; i++)
            {
                objectsArray[i] = Instantiate(objectToInstantiate, new Vector3(transform.position.x, transform.position.y, transform.position.z + radius), Quaternion.identity, transform);
                objectsArray[i].transform.RotateAround(transform.position, transform.up, degree * i);
                localPositions[i] = objectsArray[i].transform.localPosition;
            }
        }
    }

    public override void setAtributes(int pointsAmount, float radius, SpawnEnemiesTrigger trigger)
    {
        objectsAmount = pointsAmount;
        this.radius = radius;

        if (SCollider)
            SCollider.radius = radius;
        else
            Debug.Log("El objeto necesita un Sphere Collider");
    }

    public void decreaseAmountByOne() {
        //Destruimos los objetivos creados alrededor del player
        for (int i = 0; i < objectsAmount; i++)
        {
            Destroy(objectsArray[i]);
        }
        objectsInstantiated = false; //volvemos a poner reiniciar el bool que controla si los objetos están instanciados
        objectsAmount--; //disminuimos la cantidad de objetos a instanciar en una unidad
        if (objectsAmount > 0)
        {
            InstantiateObjects(); //se vuelve a llamar al método que crea los objetos una vez hemos disminuido la cantidad de ellos
        }
        else objectsInstantiated = true;
    }
}
