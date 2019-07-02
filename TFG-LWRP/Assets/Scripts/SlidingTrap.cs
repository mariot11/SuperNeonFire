using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingTrap : TrapScript
{
    public float waitingTime = 0f;
    [Range(0f, 2f)]
    public float animatorVelocity = 1f;
    public AudioSource trapSound;

    private Animator anim;
    private int WaitState = Animator.StringToHash("Wait");

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = animatorVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("wait")) {
            StartCoroutine(WaitXSeconds());
        }
    }

    private IEnumerator WaitXSeconds() {
        yield return new WaitForSeconds(waitingTime);
        anim.SetTrigger("timeToSlide");
    }

    public void makeSound() {
        trapSound.Play();
    }
}
