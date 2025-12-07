using UnityEngine;

/// <summary>
/// Editor helper para configurar faros personalizados en vehículos
/// Añade esto al prefab del vehículo si quieres configurar faros manualmente
/// </summary>
public class HeadlightSetup : MonoBehaviour
{
    [Header("Configuración Automática")]
    [Tooltip("Crear faros automáticamente al iniciar")]
    public bool createHeadlightsOnStart = true;
    
    [Header("Posiciones de Faros")]
    [Tooltip("Posición del faro izquierdo relativa al vehículo")]
    public Vector3 leftHeadlightPosition = new Vector3(-0.5f, 0.5f, 2f);
    
    [Tooltip("Posición del faro derecho relativa al vehículo")]
    public Vector3 rightHeadlightPosition = new Vector3(0.5f, 0.5f, 2f);
    
    [Header("Configuración de Luces")]
    [Tooltip("Intensidad de los faros")]
    [Range(0f, 8f)]
    public float intensity = 3f;
    
    [Tooltip("Alcance de los faros")]
    [Range(5f, 50f)]
    public float range = 30f;
    
    [Tooltip("Ángulo del cono de luz")]
    [Range(30f, 120f)]
    public float spotAngle = 70f;
    
    [Tooltip("Color de los faros")]
    public Color color = new Color(1f, 0.95f, 0.85f);
    
    [Tooltip("Ángulo de inclinación hacia abajo")]
    [Range(0f, 45f)]
    public float downwardAngle = 5f;
    
    void Start()
    {
        if (createHeadlightsOnStart)
        {
            CreateHeadlights();
        }
    }
    
    [ContextMenu("Crear Faros")]
    public void CreateHeadlights()
    {
        // Limpiar faros existentes
        Light[] existingLights = GetComponentsInChildren<Light>();
        foreach (Light light in existingLights)
        {
            if (light.gameObject.name.Contains("Headlight"))
            {
                if (Application.isPlaying)
                    Destroy(light.gameObject);
                else
                    DestroyImmediate(light.gameObject);
            }
        }
        
        // Crear nuevos faros
        CreateHeadlight("Headlight_Left", leftHeadlightPosition);
        CreateHeadlight("Headlight_Right", rightHeadlightPosition);
        
        Debug.Log($"💡 Faros creados para {gameObject.name}");
    }
    
    GameObject CreateHeadlight(string name, Vector3 localPosition)
    {
        GameObject headlight = new GameObject(name);
        headlight.transform.SetParent(transform);
        headlight.transform.localPosition = localPosition;
        headlight.transform.localRotation = Quaternion.Euler(downwardAngle, 0f, 0f);
        
        Light light = headlight.AddComponent<Light>();
        light.type = LightType.Spot;
        light.color = color;
        light.intensity = 0f; // Iniciar apagado, VehicleHeadlights lo controlará
        light.range = range;
        light.spotAngle = spotAngle;
        light.innerSpotAngle = spotAngle * 0.5f;
        light.shadows = LightShadows.None;
        light.renderMode = LightRenderMode.ForcePixel; // Mejor calidad
        light.cullingMask = ~0; // Iluminar todo
        
        return headlight;
    }
    
    [ContextMenu("Aplicar Configuración a Luces Existentes")]
    public void ApplyConfigToExistingLights()
    {
        Light[] lights = GetComponentsInChildren<Light>();
        
        foreach (Light light in lights)
        {
            if (light.gameObject.name.Contains("Headlight"))
            {
                light.type = LightType.Spot;
                light.color = color;
                light.range = range;
                light.spotAngle = spotAngle;
                light.innerSpotAngle = spotAngle * 0.5f;
                light.shadows = LightShadows.None;
                
                Debug.Log($"✅ Configuración aplicada a {light.gameObject.name}");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualizar posiciones de faros en el editor
        Gizmos.color = Color.yellow;
        
        Vector3 leftWorldPos = transform.TransformPoint(leftHeadlightPosition);
        Vector3 rightWorldPos = transform.TransformPoint(rightHeadlightPosition);
        
        Gizmos.DrawWireSphere(leftWorldPos, 0.2f);
        Gizmos.DrawWireSphere(rightWorldPos, 0.2f);
        
        // Dibujar dirección de los faros
        Gizmos.color = Color.cyan;
        Vector3 direction = Quaternion.Euler(downwardAngle, 0f, 0f) * Vector3.forward;
        direction = transform.TransformDirection(direction);
        
        Gizmos.DrawRay(leftWorldPos, direction * range * 0.3f);
        Gizmos.DrawRay(rightWorldPos, direction * range * 0.3f);
    }
}
