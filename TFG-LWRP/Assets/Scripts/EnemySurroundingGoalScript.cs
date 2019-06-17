using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySurroundingGoalScript : MonoBehaviour
{
    private bool occcupied = false;

    public void setOccupied(bool newState) {
        occcupied = newState;
    }

    public bool getOccupied()
    {
        return occcupied;
    }
}
