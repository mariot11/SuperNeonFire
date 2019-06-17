using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private bool ended = false;

    private IEnumerator Shake(float duration, float magnitude) {
        Vector3 originalPos = Vector3.zero;
        float elapsed = 0.0f;
        ended = false;

        while (elapsed < duration){
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, originalPos.y, z);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
        ended = true;
    }

    public void ShakeCamera(float duration, float magnitud) {
            StartCoroutine(Shake(duration, magnitud));
    }

    public bool hasEnded() {
        return ended;
    }
}
