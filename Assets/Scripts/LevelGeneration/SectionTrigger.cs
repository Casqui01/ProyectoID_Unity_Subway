using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Distancia en Z donde se spawneará la siguiente sección")]
    public float spawnDistance = 42f;
    
    [Header("Debug")]
    [Tooltip("Mostrar mensajes de debug")]
    public bool showDebug = false;
    
    private bool hasSpawned = false;
    private static float lastSpawnTime = 0f;
    private static float spawnCooldown = 0.5f; // Medio segundo de cooldown entre spawns

    private void OnTriggerEnter(Collider other)
    {
        // Solo se activa con el jugador
        if (!other.CompareTag("Player"))
        {
            if (showDebug)
            {
                Debug.Log($"🚫 Trigger ignorado - No es jugador: {other.gameObject.name}");
            }
            return;
        }
        
        // Solo se ejecuta una vez
        if (hasSpawned)
        {
            if (showDebug)
            {
                Debug.Log($"🚫 Trigger ya activado en: {gameObject.name}");
            }
            return;
        }
        
        // Cooldown global para evitar múltiples spawns simultáneos
        if (Time.time - lastSpawnTime < spawnCooldown)
        {
            if (showDebug)
            {
                Debug.Log($"⏱️ Cooldown activo. Esperando {spawnCooldown - (Time.time - lastSpawnTime):F2}s");
            }
            hasSpawned = true; // Marca como spawneado aunque no lo haga
            return;
        }
        
        hasSpawned = true; // Marca INMEDIATAMENTE para evitar múltiples activaciones
        lastSpawnTime = Time.time;
        
        if (showDebug)
        {
            Debug.Log($"🎯 Trigger activado en: {gameObject.name} por {other.gameObject.name}");
        }
        
        // Verifica que el manager existe y tiene secciones
        if (RoadSectionManager.Instance == null)
        {
            Debug.LogError("⚠️ SectionTrigger: No se encontró RoadSectionManager en la escena!");
            return;
        }

        if (!RoadSectionManager.Instance.HasSections())
        {
            Debug.LogError("⚠️ SectionTrigger: RoadSectionManager no tiene prefabs asignados!");
            return;
        }
        
        // Genera 2 secciones consecutivas para mayor profundidad
        for (int i = 0; i < 2; i++)
        {
            // Obtiene una sección aleatoria del manager
            GameObject selectedSection = RoadSectionManager.Instance.GetRandomSection();
            
            if (selectedSection == null)
            {
                Debug.LogError("⚠️ SectionTrigger: No se pudo obtener una sección del manager!");
                continue;
            }
            
            // Calcula la posición: spawnDistance unidades adelante en Z desde la sección actual
            // Multiplica por (i + 1) para la segunda sección
            Vector3 spawnPosition = new Vector3(
                transform.parent.position.x, 
                transform.parent.position.y, 
                transform.parent.position.z + (spawnDistance * (i + 1))
            );
            
            GameObject newSection = Instantiate(selectedSection, spawnPosition, Quaternion.identity);
            
            // Añade el script de destrucción si no lo tiene
            if (newSection.GetComponent<DestroyOldSection>() == null)
            {
                DestroyOldSection destroyer = newSection.AddComponent<DestroyOldSection>();
                destroyer.destroyDistance = 50f;
            }
            
            Debug.Log($"✅ Spawneada sección {i + 1}: {selectedSection.name} en {spawnPosition}");
        }
    }
    
    // Visualización en Scene view
    private void OnDrawGizmos()
    {
        if (transform.parent != null)
        {
            Gizmos.color = hasSpawned ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}
