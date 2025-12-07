using UnityEngine;

/// <summary>
/// Controla los faros de un vehículo, encendiéndolos automáticamente durante la noche
/// </summary>
public class VehicleHeadlights : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Array de luces que funcionan como faros (pueden ser múltiples)")]
    public Light[] headlights;
    
    [Header("Configuración")]
    [Tooltip("Intensidad de los faros cuando están encendidos")]
    [Range(0f, 8f)]
    public float headlightIntensity = 2f;
    
    [Tooltip("Alcance de los faros en metros")]
    [Range(5f, 50f)]
    public float headlightRange = 25f;
    
    [Tooltip("Color de los faros")]
    public Color headlightColor = new Color(1f, 0.95f, 0.85f); // Blanco cálido
    
    [Tooltip("Velocidad de transición al encender/apagar")]
    [Range(0.1f, 5f)]
    public float transitionSpeed = 2f;
    
    private DayNightCycle dayNightCycle;
    private bool shouldBeOn = false;
    private float currentIntensity = 0f;
    
    void Start()
    {
        // Buscar el sistema de día/noche en la escena
        dayNightCycle = FindFirstObjectByType<DayNightCycle>();
        
        if (dayNightCycle == null)
        {
            Debug.LogWarning($"⚠️ No se encontró DayNightCycle en la escena. Los faros de {gameObject.name} estarán siempre encendidos.");
            shouldBeOn = true;
        }
        
        // Si no se asignaron luces manualmente, buscar en los hijos
        if (headlights == null || headlights.Length == 0)
        {
            headlights = GetComponentsInChildren<Light>();
            
            if (headlights.Length == 0)
            {
                Debug.LogWarning($"⚠️ No se encontraron luces en {gameObject.name}. Creando faros automáticamente.");
                CreateDefaultHeadlights();
            }
        }
        
        // Configurar las luces
        ConfigureHeadlights();
        
        // Iniciar con los faros apagados si es de día
        if (dayNightCycle != null)
        {
            shouldBeOn = dayNightCycle.IsNight();
            currentIntensity = shouldBeOn ? headlightIntensity : 0f;
        }
        
        UpdateHeadlights(currentIntensity);
    }
    
    void Update()
    {
        if (dayNightCycle != null)
        {
            // Determinar si los faros deberían estar encendidos
            shouldBeOn = dayNightCycle.IsNight();
        }
        
        // Suavizar la transición
        float targetIntensity = shouldBeOn ? headlightIntensity : 0f;
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * transitionSpeed);
        
        // Actualizar las luces
        UpdateHeadlights(currentIntensity);
    }
    
    void ConfigureHeadlights()
    {
        foreach (Light light in headlights)
        {
            if (light == null) continue;
            
            // Configurar como faro (spotlight)
            light.type = LightType.Spot;
            light.color = headlightColor;
            light.range = headlightRange;
            light.spotAngle = 60f; // Ángulo típico de faros
            light.innerSpotAngle = 30f;
            light.shadows = LightShadows.None; // Desactivar sombras para rendimiento
        }
    }
    
    void UpdateHeadlights(float intensity)
    {
        foreach (Light light in headlights)
        {
            if (light == null) continue;
            
            light.intensity = intensity;
            light.enabled = intensity > 0.01f; // Apagar completamente si casi 0
        }
    }
    
    void CreateDefaultHeadlights()
    {
        // Crear dos faros por defecto (izquierdo y derecho)
        GameObject leftHeadlight = new GameObject("Headlight_Left");
        GameObject rightHeadlight = new GameObject("Headlight_Right");
        
        leftHeadlight.transform.SetParent(transform);
        rightHeadlight.transform.SetParent(transform);
        
        // Posicionar en la parte frontal del vehículo
        leftHeadlight.transform.localPosition = new Vector3(-0.5f, 0.5f, 2f);
        rightHeadlight.transform.localPosition = new Vector3(0.5f, 0.5f, 2f);
        
        // Apuntar hacia adelante y ligeramente hacia abajo
        leftHeadlight.transform.localRotation = Quaternion.Euler(5f, 0f, 0f);
        rightHeadlight.transform.localRotation = Quaternion.Euler(5f, 0f, 0f);
        
        // Añadir componentes de luz
        Light leftLight = leftHeadlight.AddComponent<Light>();
        Light rightLight = rightHeadlight.AddComponent<Light>();
        
        headlights = new Light[] { leftLight, rightLight };
        
        Debug.Log($"💡 Faros creados automáticamente para {gameObject.name}");
    }
    
    // Método público para forzar encendido/apagado manual
    public void SetHeadlightsOn(bool on)
    {
        shouldBeOn = on;
    }
    
    // Método para obtener el estado actual
    public bool AreHeadlightsOn()
    {
        return currentIntensity > 0.5f;
    }
}
