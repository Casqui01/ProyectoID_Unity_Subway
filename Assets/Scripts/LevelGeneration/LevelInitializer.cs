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
        // Espera un frame para asegurar que RoadSectionManager está inicializado
        Invoke(nameof(GenerateInitialSections), 0.1f);
    }

    private void GenerateInitialSections()
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

        Debug.Log($"🎮 Generando {initialSections} secciones iniciales...");

        // Genera las secciones iniciales
        for (int i = 0; i < initialSections; i++)
        {
            GameObject selectedSection = RoadSectionManager.Instance.GetRandomSection();
            
            if (selectedSection != null)
            {
                Vector3 spawnPosition = startPosition + new Vector3(0f, 0f, i * sectionDistance);
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

    // Método público para ajustar secciones iniciales en runtime si es necesario
    public void SetInitialSections(int count)
    {
        initialSections = Mathf.Clamp(count, 1, 10);
    }
}
