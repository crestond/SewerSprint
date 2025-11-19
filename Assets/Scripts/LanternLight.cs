using UnityEngine;
using UnityEngine.Rendering.Universal; // Needed for Light2D

public class LanternFlicker : MonoBehaviour
{
    [Header("Light Reference")]
    [SerializeField] private Light2D lanternLight;

    [Header("Intensity Settings")]
    [SerializeField] private float baseIntensity = 1f;      // Normal brightness
    [SerializeField] private float intensityVariance = 0.2f; // How much it can flicker up/down

    [Header("Flicker Speed")]
    [SerializeField] private float flickerSpeed = 2f;       // How fast it flickers

    private float noiseOffset;

    private void Awake()
    {
        // Auto-grab the Light2D if not assigned in Inspector
        if (lanternLight == null)
            lanternLight = GetComponent<Light2D>();

        // Random offset so multiple lanterns don't sync perfectly
        noiseOffset = Random.value * 100f;
    }

    private void Update()
    {
        if (lanternLight == null) return;

        // Smooth pseudo-random number between 0 and 1
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset);

        // Convert it to range -1..1
        float centered = (noise - 0.5f) * 2f;

        // Apply as a small change around the base intensity
        float targetIntensity = baseIntensity + centered * intensityVariance;

        // Clamp so it never goes negative
        lanternLight.intensity = Mathf.Max(0f, targetIntensity);
    }
}
