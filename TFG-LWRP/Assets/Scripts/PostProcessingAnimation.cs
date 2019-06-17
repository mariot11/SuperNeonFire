using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingAnimation : MonoBehaviour
{
    public float animationTime = 0.5f;
    public float CAIntensityMax = 1f;
    public float LDIntensityMax = 31;
    public float GrainIntensityMax = 1f;
    public float GrainSizeMax = 1f;
    public float VIntensityMax = 0.45f;

    private PostProcessVolume volume;
    private float CAIntensityInitial;
    private float LDIntensityInitial;
    private float GrainIntensityInitial;
    private float GrainSizeInitial;
    private float VIntensityInitial;

    ChromaticAberration chromaticAberrationLayer;
    LensDistortion lensDistortionLayer;
    Grain grainLayer;
    Vignette vignetteLayer;

    // Start is called before the first frame update
    void Start()
    {
        volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out chromaticAberrationLayer);
        volume.profile.TryGetSettings(out lensDistortionLayer);
        volume.profile.TryGetSettings(out grainLayer);
        volume.profile.TryGetSettings(out vignetteLayer);

        CAIntensityInitial = chromaticAberrationLayer.intensity.value;
        LDIntensityInitial = lensDistortionLayer.intensity.value;
        GrainIntensityInitial = grainLayer.intensity.value;
        GrainSizeInitial = grainLayer.intensity.value;
        VIntensityInitial = vignetteLayer.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator postProcessAnimation() {
        float timer = 0f;
        float interval = ((animationTime / 2) / Time.deltaTime);

        float CAIntensityPart = (CAIntensityMax - CAIntensityInitial) / interval;
        float LDIntensityPart = (LDIntensityMax - LDIntensityInitial) / interval;
        float GrainIntensityPart = (GrainIntensityMax - GrainIntensityInitial) / interval;
        float GrainSizePart = (GrainSizeMax - GrainSizeInitial) / interval;
        float VIntensityPart = (VIntensityMax - VIntensityInitial) / interval;

        while (timer < animationTime) {
            if (timer < animationTime / 2)
            {
                chromaticAberrationLayer.intensity.value = Mathf.Min(chromaticAberrationLayer.intensity.value + CAIntensityPart, CAIntensityMax);
                lensDistortionLayer.intensity.value = Mathf.Min(lensDistortionLayer.intensity.value - LDIntensityPart, LDIntensityMax);
                grainLayer.intensity.value = Mathf.Min(grainLayer.intensity.value + GrainIntensityPart, GrainIntensityMax);
                grainLayer.size.value = Mathf.Min(grainLayer.size.value + GrainSizePart, GrainSizeMax);
                vignetteLayer.intensity.value = Mathf.Min(vignetteLayer.intensity.value + VIntensityPart, VIntensityMax);
                //chromaticAberrationLayer.intensity.value += CAIntensityPart;
                //lensDistortionLayer.intensity.value += LDIntensityPart;
                //grainLayer.intensity.value += GrainIntensityPart;
                //vignetteLayer.intensity.value += VIntensityPart;
            }
            else {
                chromaticAberrationLayer.intensity.value = Mathf.Max(chromaticAberrationLayer.intensity.value - CAIntensityPart, CAIntensityInitial);
                lensDistortionLayer.intensity.value = Mathf.Max(lensDistortionLayer.intensity.value + LDIntensityPart, LDIntensityInitial);
                grainLayer.intensity.value = Mathf.Max(grainLayer.intensity.value - GrainIntensityPart, GrainIntensityInitial);
                grainLayer.size.value = Mathf.Max(grainLayer.size.value - GrainSizePart, GrainSizeInitial);
                vignetteLayer.intensity.value = Mathf.Max(vignetteLayer.intensity.value - VIntensityPart, VIntensityInitial);
                //chromaticAberrationLayer.intensity.value -= CAIntensityPart;
                //lensDistortionLayer.intensity.value -= LDIntensityPart;
                //grainLayer.intensity.value -= GrainIntensityPart;
                //vignetteLayer.intensity.value -= VIntensityPart;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
