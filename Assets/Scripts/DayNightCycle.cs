using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Skybox Materials")]
    [Tooltip("Material del skybox para el día")]
    public Material daySkybox;
    
    [Tooltip("Material del skybox para la noche")]
    public Material nightSkybox;
    
    [Header("Lighting")]
    [Tooltip("Luz direccional (el sol)")]
    public Light directionalLight;
    
    [Header("Cycle Settings")]
    [Tooltip("Duración completa del ciclo día/noche en segundos")]
    [Range(10f, 600f)]
    public float cycleDuration = 120f; // 2 minutos por defecto
    
    [Tooltip("Hora de inicio (0 = medianoche, 0.5 = mediodía, 1 = medianoche)")]
    [Range(0f, 1f)]
    public float startTime = 0.25f; // Empieza al amanecer
    
    [Header("Day Settings")]
    [Tooltip("Intensidad de la luz durante el día")]
    public float dayLightIntensity = 1.5f;
    
    [Tooltip("Color de la luz durante el día")]
    public Color dayLightColor = new Color(1f, 0.95f, 0.9f);
    
    [Header("Night Settings")]
    [Tooltip("Intensidad de la luz durante la noche")]
    public float nightLightIntensity = 0.3f;
    
    [Tooltip("Color de la luz durante la noche")]
    public Color nightLightColor = new Color(0.5f, 0.6f, 0.8f);
    
    [Header("Transition")]
    [Tooltip("Velocidad de transición entre skyboxes")]
    [Range(0.1f, 5f)]
    public float transitionSpeed = 1f;
    
    private float currentTime;
    private float targetBlend = 0f;
    private float currentBlend = 0f;
    
    void Start()
    {
        currentTime = startTime;
        
        if (directionalLight == null)
        {
            directionalLight = FindFirstObjectByType<Light>();
            if (directionalLight == null)
            {
                Debug.LogWarning("⚠️ No se encontró una luz direccional. El ciclo día/noche no afectará la iluminación.");
            }
        }
        
        // Verificar que los materiales estén asignados
        if (daySkybox == null || nightSkybox == null)
        {
            Debug.LogError("❌ Asigna los materiales Day.mat y Night.mat en el inspector del DayNightCycle");
        }
        
        UpdateCycle();
    }
    
    void Update()
    {
        // Avanzar el tiempo (0 a 1 representa un ciclo completo)
        currentTime += Time.deltaTime / cycleDuration;
        if (currentTime >= 1f)
        {
            currentTime = 0f;
        }
        
        UpdateCycle();
    }
    
    void UpdateCycle()
    {
        // Calcular el blend entre día y noche
        // 0 = día completo, 1 = noche completa
        targetBlend = CalculateBlendValue(currentTime);
        
        // Suavizar la transición
        currentBlend = Mathf.Lerp(currentBlend, targetBlend, Time.deltaTime * transitionSpeed);
        
        // Aplicar el skybox
        if (daySkybox != null && nightSkybox != null)
        {
            RenderSettings.skybox.Lerp(daySkybox, nightSkybox, currentBlend);
        }
        
        // Actualizar la luz direccional
        if (directionalLight != null)
        {
            UpdateLight(currentTime, currentBlend);
        }
    }
    
    float CalculateBlendValue(float time)
    {
        // Crear una curva suave para la transición día/noche
        // 0-0.25 = Noche (00:00 - 06:00)
        // 0.25-0.35 = Amanecer (06:00 - 08:24)
        // 0.35-0.65 = Día (08:24 - 15:36)
        // 0.65-0.75 = Atardecer (15:36 - 18:00)
        // 0.75-1.0 = Noche (18:00 - 24:00)
        
        if (time < 0.25f) // Noche temprana
        {
            return 1f;
        }
        else if (time < 0.35f) // Amanecer
        {
            float t = (time - 0.25f) / 0.1f;
            return 1f - Mathf.SmoothStep(0f, 1f, t);
        }
        else if (time < 0.65f) // Día
        {
            return 0f;
        }
        else if (time < 0.75f) // Atardecer
        {
            float t = (time - 0.65f) / 0.1f;
            return Mathf.SmoothStep(0f, 1f, t);
        }
        else // Noche tardía
        {
            return 1f;
        }
    }
    
    void UpdateLight(float time, float blend)
    {
        // Rotar la luz (simular el movimiento del sol)
        float rotationAngle = time * 360f - 90f; // -90 para que empiece en el horizonte
        directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, 170f, 0f);
        
        // Interpolar intensidad y color
        directionalLight.intensity = Mathf.Lerp(dayLightIntensity, nightLightIntensity, blend);
        directionalLight.color = Color.Lerp(dayLightColor, nightLightColor, blend);
    }
    
    // Métodos públicos para control externo
    public void SetTimeOfDay(float time)
    {
        currentTime = Mathf.Clamp01(time);
    }
    
    public float GetTimeOfDay()
    {
        return currentTime;
    }
    
    public bool IsNight()
    {
        return currentBlend > 0.5f;
    }
    
    public bool IsDay()
    {
        return currentBlend < 0.5f;
    }
    
    // Para debugging en el editor
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateCycle();
        }
    }
}
