using UnityEngine;

public class DayNightCycler : MonoBehaviour
{
    public float timeSpeed = 1f; 
   
    public float currentWorldTime = 0f; 
    public float maxSunIntensity = 1.5f;
    public Color dayAmbientColor = new Color(0.7f, 0.7f, 0.7f);
    public Color nightAmbientColor = new Color(0.0f, 0.0f, 0.0f); 
    public Color daySkyColor = new Color(0.5f, 0.7f, 1.0f);
    public Light sunLight; 
    //number of seconds in a day totalling to 5 minutes
    private const float SECONDS_IN_DAY = 300f;

    void Update()
    {
        UpdateWorldTime();
        UpdateSunRotation();
    }

    void UpdateWorldTime()
    {
        currentWorldTime += Time.deltaTime * timeSpeed;

        if (currentWorldTime >= SECONDS_IN_DAY)
        {
            currentWorldTime %= SECONDS_IN_DAY; 
        }
    }

    void UpdateSunRotation()
    {
        if (sunLight == null) return;

        float dayPercentage = currentWorldTime / SECONDS_IN_DAY;
        float sunAngle = dayPercentage * 360f;

        sunLight.transform.localRotation = Quaternion.Euler(sunAngle - 90f, 170f, 0f);

        float sunXRotation = sunLight.transform.localEulerAngles.x;
        if (sunXRotation > 180) sunXRotation -= 360f;

        float intensityFactor;
        if (sunXRotation < -15f) intensityFactor = 0f;      
        else if (sunXRotation > 15f) intensityFactor = 1f; 
        else intensityFactor = Mathf.InverseLerp(-15f, 15f, sunXRotation); 

        RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, intensityFactor);

        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1.0f);
            
            Color targetSkyColor = Color.Lerp(Color.black, daySkyColor, intensityFactor);
            RenderSettings.skybox.SetColor("_SkyTint", targetSkyColor);
            
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0.0f, 1.0f, intensityFactor));
        }

        sunLight.intensity = Mathf.Lerp(0.0f, maxSunIntensity, intensityFactor); 
    }

    void OnDestroy()
    {
        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetColor("_SkyTint", new Color(0.5f, 0.5f, 0.5f));
            RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1.0f);
            RenderSettings.skybox.SetFloat("_Exposure", 1.0f);
        }
    }
}