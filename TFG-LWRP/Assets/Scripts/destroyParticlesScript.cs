using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyParticlesScript : MonoBehaviour
{
    private ParticleSystem ps;
    // Start is called before the first frame update
    void Awake()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.isStopped) {
            Destroy(gameObject);
        }
    }
}
