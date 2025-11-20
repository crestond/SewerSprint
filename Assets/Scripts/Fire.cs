using UnityEngine;
using UnityEngine.Rendering.Universal; // Needed for Light2D

public class FireLightFlicker : MonoBehaviour
{
    [SerializeField] private Light2D light2D;

    [Header("Intensity")]
    [SerializeField] private float baseIntensity = 1.5f; // average brightness
    [SerializeField] private float intensityVariance = 0.4f; // how much it flickers
    [SerializeField] private float flickerSpeed = 8f; // how fast it flickers

    [Header("Radius (optional)")]
    [SerializeField] private bool flickerRadius = true;
    [SerializeField] private float baseOuterRadius = 3f;
    [SerializeField] private float radiusVariance = 0.3f;

    private void Reset()
    {
        // auto-assign Light2D if on the same GameObject
        if (!light2D) light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (!light2D) return;

        // Use Perlin noise for a more organic flicker
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f); // 0..1

        float intensity = baseIntensity + (noise - 0.5f) * 2f * intensityVariance;
        light2D.intensity = Mathf.Max(0f, intensity);

        if (flickerRadius)
        {
            float radius = baseOuterRadius + (noise - 0.5f) * 2f * radiusVariance;
            light2D.pointLightOuterRadius = Mathf.Max(0f, radius);
        }
    }
}
