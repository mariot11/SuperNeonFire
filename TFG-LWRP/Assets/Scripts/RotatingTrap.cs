using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTrap : TrapScript
{
    [Range(0f, 5f)]
    public float rotationSpeed = 1f;
    public bool rotateToRight = true;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if(anim == null)    anim = GetComponent<Animator>();
        anim.speed = rotationSpeed;
        anim.SetBool("right", rotateToRight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
