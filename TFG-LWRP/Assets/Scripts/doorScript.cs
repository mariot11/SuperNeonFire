using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : TrapScript
{
    public AudioSource openSound;
    public AudioSource closeSound;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setOpen(bool isOpen) {
        anim.SetBool("isOpen", isOpen);
    }

    public void makeOpenSound() {
        openSound.Play();
    }

    public void makeCloseSound()
    {
        closeSound.Play();
    }
}
