using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public doorScript[] doors;
    public float timeLimit = 3f;

    //sounds
    public AudioSource timerSound;
    public AudioSource switchActiveSound;

    private bool isActive = false;
    private bool LastSeconds = false;
    private float timer = 0f;
    private const float xSeconds = 2f;
    private Animator anim;
    private const string active = "isActive";

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            if (timer < timeLimit)
            {
                timer += Time.deltaTime;
                
                if (timer > timeLimit - xSeconds) {
                    if (LastSeconds == false) {
                        LastSeconds = true;
                        timerSound.pitch = 1.3f;
                    }
                }
            }
            else {
                //parar el sonido del temporizador 
                timerSound.Stop();

                isActive = false;
                foreach (doorScript ds in doors)
                    ds.setOpen(isActive);
                anim.SetBool(active, isActive);
                timer = 0f;
            }
        }
    }

    public void setActive() {
        //activar el sonido del temporizador 
        LastSeconds = false;
        timerSound.pitch = 1;
        switchActiveSound.Play();
        timerSound.Play();

        isActive = true;
        foreach (doorScript ds in doors)
            ds.setOpen(isActive);
        anim.SetBool(active, isActive);
    }


    
}
