using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [Header("Configuración Inicial")]
    [Tooltip("Número de secciones que se generan al inicio del juego")]
    [Range(1, 10)]
    public int initialSections = 3;

    [Tooltip("Distancia en Z entre cada sección")]
    public float sectionDistance = 42f;

    [Tooltip("Posición inicial de la primera sección extra")]
    public Vector3 startPosition = new Vector3(0f, 0f, 42f);

    private void Start()
    {
        Debug.Log("🚀 LevelInitializer.Start() llamado");
        // Suscribirse al evento de carga de escena
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Generar secciones en el primer Start
        Invoke(nameof(GenerateInitialSections), 0.1f);
    }
    
    private void OnDestroy()
    {
        // Desuscribirse al destruir
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    /// <summary>
    /// Se llama cuando se carga una escena
    /// </summary>
	private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
	{
		if (scene.name == "Ciudad")
		{
			Debug.Log("🎮 Escena 'Ciudad' cargada - Regenerando secciones iniciales");
			Invoke(nameof(GenerateInitialSections), 0.1f);
		}
	}    private void GenerateInitialSections()
    {
        // Verifica que el manager existe
        if (RoadSectionManager.Instance == null)
        {
            Debug.LogError("⚠️ LevelInitializer: No se encontró RoadSectionManager!");
            return;
        }

        if (!RoadSectionManager.Instance.HasSections())
        {
            Debug.LogError("⚠️ LevelInitializer: RoadSectionManager no tiene prefabs!");
            return;
        }

        // Primero limpiar secciones anteriores (excepto permanentes)
        CleanupOldSections();

        Debug.Log($"🎮 Generando {initialSections} secciones iniciales...");
        
        // Resetear el tracking de spawn del manager
        RoadSectionManager.Instance.ResetSpawnTracking();

        // Genera las secciones iniciales
        for (int i = 0; i < initialSections; i++)
        {
            GameObject selectedSection = RoadSectionManager.Instance.GetRandomSection();
            
            if (selectedSection != null)
            {
                // Usa el tracking global para obtener la siguiente posición Z válida
                float nextZ = RoadSectionManager.Instance.GetNextSpawnZ();
                Vector3 spawnPosition = new Vector3(0f, 0f, nextZ);
                
                GameObject newSection = Instantiate(selectedSection, spawnPosition, Quaternion.identity);
                newSection.name = $"Section_Initial_{i + 1}";
                
                // Añade el script de destrucción si no lo tiene
                if (newSection.GetComponent<DestroyOldSection>() == null)
                {
                    DestroyOldSection destroyer = newSection.AddComponent<DestroyOldSection>();
                    destroyer.destroyDistance = 50f;
                }
                
                Debug.Log($"✅ Sección inicial {i + 1} spawneada en {spawnPosition}");
            }
        }
    }
    
    /// <summary>
    /// Limpia las secciones spawneadas anteriormente (excepto permanentes)
    /// </summary>
    private void CleanupOldSections()
    {
        DestroyOldSection[] allSections = FindObjectsByType<DestroyOldSection>(FindObjectsSortMode.None);
        int cleanedCount = 0;
        
        foreach (DestroyOldSection section in allSections)
        {
            // No destruir secciones permanentes
            if (!section.isPermanent)
            {
                Destroy(section.gameObject);
                cleanedCount++;
            }
        }
        
        if (cleanedCount > 0)
        {
            Debug.Log($"🧹 {cleanedCount} secciones antiguas limpiadas");
        }
    }

    // Método público para ajustar secciones iniciales en runtime si es necesario
    public void SetInitialSections(int count)
    {
        initialSections = Mathf.Clamp(count, 1, 10);
    }
    
    /// <summary>
    /// Regenera las secciones iniciales (para cuando se reinicia el juego)
    /// </summary>
    public void RegenerateInitialSections()
    {
        Debug.Log("🔄 RegenerateInitialSections() llamado manualmente");
        GenerateInitialSections();
    }
}
