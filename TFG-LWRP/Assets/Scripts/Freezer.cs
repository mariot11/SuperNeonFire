using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{
    private float _pendingFreezeDuration = 0f;
    private float timeScale = 0f;
    bool _isFrozen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_pendingFreezeDuration > 0 && !_isFrozen) {
            StartCoroutine(DoFreeze());
        }
    }

    public void Freeze(float duration, float timeScale) {
        _pendingFreezeDuration = duration;
        this.timeScale = timeScale;
    }

    IEnumerator DoFreeze()
    {
        _isFrozen = true;
        var original = Time.timeScale;
        var originalFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = timeScale;
        yield return new WaitForSecondsRealtime(_pendingFreezeDuration);

        Time.timeScale = original;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        _pendingFreezeDuration = 0;
        _isFrozen = false;

    }
}
