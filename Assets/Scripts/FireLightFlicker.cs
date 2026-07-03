using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(HDAdditionalLightData))]
public class FireLightFlicker : MonoBehaviour
{
    [Header("Flicker Amount (as a % of your current Inspector values)")]
    [Range(0f, 1f)][SerializeField] private float intensityVariance = 0.3f; // 30% swing
    [Range(0f, 1f)][SerializeField] private float rangeVariance = 0.15f;    // 15% swing

    [Header("Flicker Speed")]
    [SerializeField] private float flickerSpeed = 8f;

    private Light _light;
    private HDAdditionalLightData _hdData;

    private float _baseIntensity;
    private float _baseRange;
    private float _noiseOffset;

    void Awake()
    {
        _light = GetComponent<Light>();
        _hdData = GetComponent<HDAdditionalLightData>();

        // Capture whatever you've already set in the Inspector as the baseline
        _baseIntensity = _hdData.intensity;
        _baseRange = _light.range;

        // Randomize per-instance so multiple fires don't flicker in sync
        _noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Perlin noise gives a smooth -0.5 to +0.5 wobble
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + _noiseOffset, 0f) - 0.5f;

        _hdData.intensity = _baseIntensity + (_baseIntensity * intensityVariance * noise * 2f);
        _light.range = _baseRange + (_baseRange * rangeVariance * noise * 2f);
    }
}