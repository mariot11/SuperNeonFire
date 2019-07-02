using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoalTrigger : MonoBehaviour
{
    public GameObject winMenu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            winMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
