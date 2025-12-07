using UnityEngine;

public class RoadSectionManager : MonoBehaviour
{
    public static RoadSectionManager Instance { get; private set; }

    [Header("Pool de Secciones")]
    [Tooltip("Array de diferentes prefabs de secciones. Se elegirá uno aleatoriamente.")]
    public GameObject[] roadSectionPrefabs;
    
    [Header("Spawning")]
    [Tooltip("Distancia entre secciones")]
    public float sectionLength = 42f;
    
    // Track de la última posición Z spawneada
    private float lastSpawnedZ = 0f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Validación
        if (roadSectionPrefabs == null || roadSectionPrefabs.Length == 0)
        {
            Debug.LogError("⚠️ RoadSectionManager: No hay prefabs asignados!");
        }
        else
        {
            Debug.Log($"✅ RoadSectionManager: {roadSectionPrefabs.Length} prefabs de secciones cargados");
        }
    }

    /// <summary>
    /// Obtiene un prefab de sección aleatorio del pool
    /// </summary>
    public GameObject GetRandomSection()
    {
        if (roadSectionPrefabs == null || roadSectionPrefabs.Length == 0)
        {
            Debug.LogError("⚠️ No hay secciones disponibles!");
            return null;
        }

        int randomIndex = Random.Range(0, roadSectionPrefabs.Length);
        return roadSectionPrefabs[randomIndex];
    }

    /// <summary>
    /// Verifica si hay secciones disponibles
    /// </summary>
    public bool HasSections()
    {
        return roadSectionPrefabs != null && roadSectionPrefabs.Length > 0;
    }
    
    /// <summary>
    /// Obtiene la siguiente posición Z válida para spawneo
    /// </summary>
    public float GetNextSpawnZ()
    {
        lastSpawnedZ += sectionLength;
        return lastSpawnedZ;
    }
    
    /// <summary>
    /// Obtiene la siguiente posición Z válida basada en una posición de referencia
    /// </summary>
    public float GetNextSpawnZFrom(float referenceZ)
    {
        // Calcula la siguiente posición basada en la referencia
        float nextZ = referenceZ + sectionLength;
        
        // Actualiza lastSpawnedZ solo si la nueva posición es mayor
        if (nextZ > lastSpawnedZ)
        {
            lastSpawnedZ = nextZ;
        }
        
        return nextZ;
    }
    
    /// <summary>
    /// Establece la última posición Z spawneada (útil para inicialización)
    /// </summary>
    public void SetLastSpawnZ(float z)
    {
        lastSpawnedZ = z;
        Debug.Log($"📍 RoadSectionManager: lastSpawnedZ actualizado a {lastSpawnedZ}");
    }
    
    /// <summary>
    /// Obtiene la última posición Z spawneada
    /// </summary>
    public float GetLastSpawnZ()
    {
        return lastSpawnedZ;
    }
    
    /// <summary>
    /// Resetea el tracking de posición Z (llamar al reiniciar juego)
    /// </summary>
    public void ResetSpawnTracking()
    {
        lastSpawnedZ = 0f;
        Debug.Log("🔄 RoadSectionManager: Tracking de spawn reseteado");
    }
}
