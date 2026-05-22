using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }


    [Header("Turn Settings")]
    public float weatherChangeChance = 0.2f; // probability (0-1) that the weather changes each turn


    [Header("Scene References")]
    public Light directionalLight;
    public ParticleSystem rainParticleSystem;
    public ParticleSystem windParticleSystem;


    // Lighting profiles
    [Header("Lighting: Sunny")]
    public Color sunnyLightColor     = new Color(1.00f, 0.96f, 0.84f);
    public float sunnyLightIntensity = 1.20f;
    public Color sunnyAmbientColor   = new Color(0.45f, 0.52f, 0.60f);

    [Header("Lighting: Light Rain")]
    public Color rainLightLightColor     = new Color(0.78f, 0.82f, 0.88f);
    public float rainLightLightIntensity = 0.65f;
    public Color rainLightAmbientColor   = new Color(0.34f, 0.36f, 0.42f);

    [Header("Lighting: Heavy Rain")]
    public Color rainHeavyLightColor     = new Color(0.45f, 0.50f, 0.60f);
    public float rainHeavyLightIntensity = 0.35f;
    public Color rainHeavyAmbientColor   = new Color(0.22f, 0.24f, 0.30f);

    [Header("Lighting: Windy")]
    public Color windyLightColor     = new Color(0.82f, 0.86f, 0.90f);
    public float windyLightIntensity = 0.80f;
    public Color windyAmbientColor   = new Color(0.37f, 0.40f, 0.46f);


    [Header("Heavy Rain Fog")]
    public Color heavyRainFogColor   = new Color(0.38f, 0.41f, 0.48f);
    public float heavyRainFogDensity = 0.04f;


    [Header("Particle Emission Rate Config")]
    public float lightRainEmission = 80f;
    public float heavyRainEmission = 400f;
    public float windEmission      = 30f; 


    [Header("Wind Config")]
    public float defaultTurbulence = 0.3f;
    public float defaultPulseMagnitude = 0.5f;
    public float defaultPulseFrequency = 0.1f;
    public float windyWind = 2.5f;
    public float windyTurbulence = 0.8f;

    public float heavyRainWind = 0.8f;

    
    [Header("Transition")]
    public float transitionDuration = 2.5f; // in seconds, for lighting crossfade


    // State
    public WeatherState CurrentWeather { get; private set; } = WeatherState.Sunny;

    public event Action<WeatherState> OnWeatherChanged;

    private Coroutine _transitionRoutine;
    private WindZone _windZone;


    // Unity cycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (directionalLight == null) Debug.LogWarning("[WeatherManager] Directional Light is not assigned");
        if (rainParticleSystem == null) Debug.LogWarning("[WeatherManager] Rain ParticleSystem is not assigned");
        if (windParticleSystem == null) Debug.LogWarning("[WeatherManager] Wind ParticleSystem is not assigned");

        SetupWindZone();
        ConfigRainParticles();
        ConfigWindParticles();

        ApplyWeatherImmediate(WeatherState.Sunny);

        if (TurnManager.Instance != null) TurnManager.Instance.OnTurnPassed += OnTurnPassed;
        else Debug.LogWarning("[WeatherManager] failed to subscribe to TurnManager - call WeatherManager.Instance.SetWeather() manually");
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null) TurnManager.Instance.OnTurnPassed -= OnTurnPassed;
    }

    
    // Public API
    public void SetWeather(WeatherState weather)
    {
        if (CurrentWeather == weather) return;
        
        CurrentWeather = weather;
        OnWeatherChanged?.Invoke(weather);

        if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
        _transitionRoutine = StartCoroutine(TransitionRoutine(weather));
    }

    public void SetWeatherImmediate(WeatherState weather)
    {
        if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
        
        CurrentWeather = weather;
        OnWeatherChanged?.Invoke(weather);
        ApplyWeatherImmediate(weather);
    }

    
    // Turn callback
    private void OnTurnPassed(int turnCount)
    {
        float roll      = UnityEngine.Random.value;
        bool success    = roll <= weatherChangeChance;

        if (success)
        {
            WeatherState newWeather = GetRandomWeather();
            bool sameAsCurrent      = newWeather == CurrentWeather;

            Debug.Log($"[WeatherManager] Turn {turnCount} - Random value {roll:F2} <= {weatherChangeChance:F2} - Weather change triggered " +
            $"- {CurrentWeather} -> {newWeather}" + (sameAsCurrent ? " (same state, no change)" : ""));
            SetWeather(newWeather);
        }
        else Debug.Log($"[WeatherManager] Turn {turnCount} - Random value {roll:F2} <= {weatherChangeChance:F2} - Weather unchanged");
    }

    private static WeatherState GetRandomWeather()
    {
        var states = (WeatherState[]) Enum.GetValues(typeof(WeatherState));
        return states[UnityEngine.Random.Range(0, states.Length)];
    }


    // Transition coroutine
    private IEnumerator TransitionRoutine(WeatherState target)
    {
        Color   startLightColor     = directionalLight.color;
        float   startLightIntensity = directionalLight.intensity;
        Color   startAmbientColor   = RenderSettings.ambientLight;
        float   startFogDensity     = RenderSettings.fogDensity;
        Color   startFogColor       = RenderSettings.fogColor;
        bool    fogEnabled          = RenderSettings.fog;

        // calculating target values
        GetLightingForWeatherState(target, out Color targetLightColor, out float targetLightIntensity, out Color targetAmbientColor);
        bool    targetFog           = target == WeatherState.RainHeavy;
        Color   targetFogColor      = targetFog ? heavyRainFogColor : startFogColor;
        float   targetFogDensity    = targetFog ? heavyRainFogDensity : 0f;

        if (targetFog) RenderSettings.fog = true;

        ApplyParticles(target);
        ApplyWindZone(target);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            directionalLight.color      = Color.Lerp(startLightColor, targetLightColor, smooth);
            directionalLight.intensity  = Mathf.Lerp(startLightIntensity, targetLightIntensity, smooth);
            RenderSettings.ambientLight = Color.Lerp(startAmbientColor, targetAmbientColor, smooth);
            RenderSettings.fogDensity   = Mathf.Lerp(startFogDensity, targetFogDensity, smooth);
            RenderSettings.fogColor     = Color.Lerp(startFogColor, targetFogColor, smooth);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap to target values
        directionalLight.color      = targetLightColor;
        directionalLight.intensity  = targetLightIntensity;
        RenderSettings.ambientLight = targetAmbientColor;
        RenderSettings.fogDensity   = targetFogDensity;
        RenderSettings.fogColor     = targetFogColor;

        if (!targetFog) RenderSettings.fog = false;
    }

    // No lerp apply
    private void ApplyWeatherImmediate(WeatherState weather)
    {
        GetLightingForWeatherState(weather, out Color lightColor, out float lightIntensity, out Color ambientColor);

        directionalLight.color      = lightColor;
        directionalLight.intensity  = lightIntensity;
        RenderSettings.ambientLight = ambientColor;

        bool isFoggy = weather == WeatherState.RainHeavy;
        RenderSettings.fog          = isFoggy;
        RenderSettings.fogColor     = isFoggy ? heavyRainFogColor : Color.white;
        RenderSettings.fogDensity   = isFoggy ? heavyRainFogDensity : 0f;
        
        ApplyParticles(weather);
        ApplyWindZone(weather);
    }


    private void GetLightingForWeatherState(WeatherState weather, out Color lightColor, out float lightIntensity, out Color ambientColor)
    {
        switch (weather)
        {
            case WeatherState.Sunny:
                lightColor      = sunnyLightColor;
                lightIntensity  = sunnyLightIntensity;
                ambientColor    = sunnyAmbientColor;
                break;
            case WeatherState.RainLight:
                lightColor      = rainLightLightColor;
                lightIntensity  = rainLightLightIntensity;
                ambientColor    = rainLightAmbientColor;
                break;
            case WeatherState.RainHeavy:
                lightColor      = rainHeavyLightColor;
                lightIntensity  = rainHeavyLightIntensity;
                ambientColor    = rainHeavyAmbientColor;
                break;
            case WeatherState.Windy:
                lightColor      = windyLightColor;
                lightIntensity  = windyLightIntensity;
                ambientColor    = windyAmbientColor;
                break;
            default:
                lightColor      = sunnyLightColor;
                lightIntensity  = sunnyLightIntensity;
                ambientColor    = sunnyAmbientColor;
                break;
        }
    }


    private void ApplyParticles(WeatherState weather)
    {
        // Rain
        if (rainParticleSystem != null)
        {
            var emission = rainParticleSystem.emission;
            switch (weather)
            {
                case WeatherState.RainLight:
                    emission.rateOverTime = lightRainEmission;
                    if (!rainParticleSystem.isPlaying) rainParticleSystem.Play();
                    break;
                case WeatherState.RainHeavy:
                    emission.rateOverTime = heavyRainEmission;
                    if (!rainParticleSystem.isPlaying) rainParticleSystem.Play();
                    break;
                default:
                    rainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    break;
            }
        }

        // wind
        if (windParticleSystem != null)
        {
            var emission = windParticleSystem.emission;
            switch (weather)
            {
                case WeatherState.Windy:
                    emission.rateOverTime = windEmission;
                    if (!windParticleSystem.isPlaying) windParticleSystem.Play();
                    break;
                default:
                    windParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    break;
            }
        }
    }

    // Wind zone
    private void SetupWindZone()
    {
        _windZone = GetComponentInChildren<WindZone>();
        if (_windZone == null) { Debug.LogWarning("[WeatherManager] WindZone is null - stopping SetupWindZOne()"); return; }

        _windZone.windMain = 0f;
        _windZone.windTurbulence = defaultTurbulence;
        _windZone.windPulseMagnitude = defaultPulseMagnitude;
        _windZone.windPulseFrequency = defaultPulseFrequency;
    }

    private void ApplyWindZone(WeatherState weather)
    {
        if (_windZone == null) return;
        _windZone.windMain = weather == WeatherState.Windy ? windyWind : weather == WeatherState.RainHeavy ? heavyRainWind : 0f;
        _windZone.windTurbulence = weather == WeatherState.Windy ? windyTurbulence : defaultTurbulence;
    }


    // Particle system setup (no config)
    private void ConfigRainParticles()
    {
        if (rainParticleSystem == null) return;

        var main = rainParticleSystem.main;
        main.loop               = true;
        main.startLifetime      = 1.8f;
        main.startSpeed         = new ParticleSystem.MinMaxCurve(12f, 18f);
        main.startSize          = new ParticleSystem.MinMaxCurve(0.04f, 0.07f);
        main.startColor         = new Color(0.7f, 0.8f, 0.95f, 0.6f);
        main.maxParticles       = 3000;
        main.simulationSpace    = ParticleSystemSimulationSpace.World;

        var shape = rainParticleSystem.shape;
        shape.enabled   = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale     = new Vector3(50f, 1f, 50f);

        var forces = rainParticleSystem.forceOverLifetime;
        forces.enabled = true;
        forces.y = new ParticleSystem.MinMaxCurve(25f);

        var renderer = rainParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode     = ParticleSystemRenderMode.Stretch;
        renderer.velocityScale  = 0.06f;
        renderer.lengthScale    = 1.5f;

        rainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void ConfigWindParticles()
    {
        if (windParticleSystem == null) return;

        var main = windParticleSystem.main;
        main.loop               = true;
        main.startLifetime      = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSpeed         = new ParticleSystem.MinMaxCurve(4f, 8f);
        main.startSize          = new ParticleSystem.MinMaxCurve(2.3f, 3.0f);
        main.startRotation      = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.startColor         = new Color(0.75f, 0.72f, 0.60f, 0.75f);   // dusty tan
        main.maxParticles       = 500;
        main.simulationSpace    = ParticleSystemSimulationSpace.World;
        main.gravityModifier    = 0.05f;
 
        var shape = windParticleSystem.shape;
        shape.enabled       = true;
        shape.shapeType     = ParticleSystemShapeType.Box;
        shape.scale         = new Vector3(5f, 10f, 40f);
 
        var vel = windParticleSystem.velocityOverLifetime;
        vel.enabled = true;
        vel.x       = new ParticleSystem.MinMaxCurve(5f, 9f); // wind dir is +x
        vel.y       = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        vel.z       = new ParticleSystem.MinMaxCurve(0f, 0f);
 
        // Fade out at end of life
        var col = windParticleSystem.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.55f, 0.15f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = grad;
 
        windParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
