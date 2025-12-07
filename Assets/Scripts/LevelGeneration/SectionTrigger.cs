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
        if (!other.CompareTag("Player") && other.GetComponent<Movement>() == null)
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
        float timeSinceLastSpawn = Time.time - lastSpawnTime;
        if (lastSpawnTime > 0 && timeSinceLastSpawn < spawnCooldown)
        {
            if (showDebug)
            {
                Debug.Log($"⏱️ Cooldown activo. Esperando {spawnCooldown - timeSinceLastSpawn:F2}s");
            }
            hasSpawned = true; // Marca como spawneado aunque no lo haga
            return;
        }
        
        hasSpawned = true; // Marca INMEDIATAMENTE para evitar múltiples activaciones
        lastSpawnTime = Time.time;
        
        Debug.Log($"🎯 TRIGGER ACTIVADO en: {gameObject.name} por {other.gameObject.name} (Time: {Time.time:F2})");
        
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
        
        // Obtiene la posición base desde donde spawnear (la sección padre del trigger)
        float baseZ = transform.parent != null ? transform.parent.position.z : 0f;
        
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
            
            // Calcula la siguiente posición basada en la sección actual
            // Primera iteración: baseZ + 42, Segunda: baseZ + 84
            float nextZ = RoadSectionManager.Instance.GetNextSpawnZFrom(baseZ + (spawnDistance * i));
            Vector3 spawnPosition = new Vector3(0f, 0f, nextZ);
            
            GameObject newSection = Instantiate(selectedSection, spawnPosition, Quaternion.identity);
            
            // Añade el script de destrucción si no lo tiene
            if (newSection.GetComponent<DestroyOldSection>() == null)
            {
                DestroyOldSection destroyer = newSection.AddComponent<DestroyOldSection>();
                destroyer.destroyDistance = 50f;
            }
            
            Debug.Log($"✅ Spawneada sección {i + 1}: {selectedSection.name} en {spawnPosition} (base: {baseZ})");
        }
    }
    
    /// <summary>
    /// Resetea el trigger para poder usarlo de nuevo (útil al reiniciar el juego)
    /// </summary>
    public void ResetTrigger()
    {
        hasSpawned = false;
        Debug.Log($"🔄 Trigger reseteado: {gameObject.name} (hasSpawned={hasSpawned})");
    }
    
    /// <summary>
    /// Resetea todos los triggers en la escena (llamar al reiniciar juego)
    /// </summary>
    public static void ResetAllTriggers()
    {
        // Resetear el tiempo estático primero
        lastSpawnTime = 0f;
        
        SectionTrigger[] allTriggers = FindObjectsByType<SectionTrigger>(FindObjectsSortMode.None);
        
        Debug.Log($"🔄 Reseteando {allTriggers.Length} triggers (lastSpawnTime={lastSpawnTime}, Time.time={Time.time:F2})");
        
        if (allTriggers.Length == 0)
        {
            Debug.LogError("❌ NO SE ENCONTRARON TRIGGERS EN LA ESCENA! Asegúrate de que la sección permanente tiene un SectionTrigger.");
        }
        
        foreach (SectionTrigger trigger in allTriggers)
        {
            Debug.Log($"   - Trigger encontrado en: {trigger.gameObject.name} (parent: {trigger.transform.parent?.name})");
            trigger.ResetTrigger();
        }
        
        Debug.Log($"✅ Todos los triggers reseteados correctamente");
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
