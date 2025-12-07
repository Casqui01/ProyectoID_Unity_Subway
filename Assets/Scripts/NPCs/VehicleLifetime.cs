using UnityEngine;

public class VehicleLifetime : MonoBehaviour
{
    [Header("Destrucción automática")]
    [Tooltip("Tiempo máximo de vida del vehículo (0 = infinito)")]
    public float maxLifetime = 60f;
    
    [Tooltip("Destruir si cae por debajo de esta altura Y")]
    public float minHeight = -10f;
    
    [Tooltip("Distancia máxima del jugador antes de destruir (0 = no limitar)")]
    public float maxDistanceFromPlayer = 200f;
    
    private float spawnTime;
    private Transform player;

    void Start()
    {
        spawnTime = Time.time;
        
        // Buscar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Movement movementScript = FindFirstObjectByType<Movement>();
            if (movementScript != null)
            {
                playerObj = movementScript.gameObject;
            }
        }
        
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Destruir si ha vivido demasiado tiempo
        if (maxLifetime > 0 && Time.time - spawnTime > maxLifetime)
        {
            Debug.Log($"🚗 Vehículo destruido por tiempo de vida: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        
        // Destruir si cae por debajo del mínimo
        if (transform.position.y < minHeight)
        {
            Debug.Log($"🚗 Vehículo destruido por caída: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        
        // Destruir si está muy lejos del jugador (hacia atrás o adelante)
        if (maxDistanceFromPlayer > 0 && player != null)
        {
            float distanceZ = Mathf.Abs(transform.position.z - player.position.z);
            if (distanceZ > maxDistanceFromPlayer)
            {
                Debug.Log($"🚗 Vehículo destruido por distancia: {gameObject.name} (distancia: {distanceZ:F2})");
                Destroy(gameObject);
                return;
            }
        }
    }
}
