using UnityEngine;

public class RoadSectionManager : MonoBehaviour
{
    public static RoadSectionManager Instance { get; private set; }

    [Header("Pool de Secciones")]
    [Tooltip("Array de diferentes prefabs de secciones. Se elegirá uno aleatoriamente.")]
    public GameObject[] roadSectionPrefabs;

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
}
