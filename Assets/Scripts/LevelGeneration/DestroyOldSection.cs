using UnityEngine;

public class DestroyOldSection : MonoBehaviour
{
    private Transform player;
    
    [Header("Configuración de Destrucción")]
    [Tooltip("Distancia detrás del jugador para destruir la sección")]
    public float destroyDistance = 50f;
    
    [Tooltip("Si está marcado, esta sección NUNCA se destruirá (para sección inicial)")]
    public bool isPermanent = false;
    
    [Header("Debug")]
    [Tooltip("Mostrar información de debug en consola")]
    public bool showDebug = false;

    private bool hasFoundPlayer = false;

    void Start()
    {
        FindPlayer();
        
        if (showDebug)
        {
            Debug.Log($"🔍 DestroyOldSection iniciado en: {gameObject.name} (Z: {transform.position.z})");
        }
    }

    void FindPlayer()
    {
        // Busca al jugador por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj == null)
        {
            // Si no tiene tag Player, busca por el script Movement
            Movement movementScript = FindFirstObjectByType<Movement>();
            if (movementScript != null)
            {
                playerObj = movementScript.gameObject;
            }
        }
        
        if (playerObj != null)
        {
            player = playerObj.transform;
            hasFoundPlayer = true;
            
            if (showDebug)
            {
                Debug.Log($"✅ DestroyOldSection: Jugador encontrado en {player.position}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ DestroyOldSection en {gameObject.name}: No se encontró el jugador. Reintentando...");
            // Reintenta en 1 segundo
            Invoke(nameof(FindPlayer), 1f);
        }
    }

    void Update()
    {
        // Si es permanente, nunca destruir
        if (isPermanent)
        {
            return;
        }
        
        if (!hasFoundPlayer || player == null)
        {
            return;
        }

        // Calcula la distancia entre el jugador y esta sección
        float distance = player.position.z - transform.position.z;

        // Debug opcional
        if (showDebug && Time.frameCount % 60 == 0) // Cada 60 frames
        {
            Debug.Log($"📏 Distancia de {gameObject.name}: {distance:F2} (Destruir en: {destroyDistance})");
        }

        // Si el jugador está muy adelante de esta sección, destrúyela
        if (distance > destroyDistance)
        {
            Debug.Log($"🗑️ Destruyendo sección antigua: {gameObject.name} (Jugador Z: {player.position.z:F2}, Sección Z: {transform.position.z:F2}, Distancia: {distance:F2})");
            Destroy(gameObject);
        }
    }

    // Visualización en el editor
    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - destroyDistance));
        }
    }
}
