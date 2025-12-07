using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;        // El objeto a crear
    public Transform spawnPoint;     // Dónde aparece
    public Transform targetPoint;    // Hacia dónde se mueve
    public float minSpeed = 3f;      // Velocidad mínima
    public float maxSpeed = 7f;      // Velocidad máxima

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        GameObject obj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        
        // IMPORTANTE: Desparentar el objeto spawneado para que no se destruya con la sección
        obj.transform.SetParent(null);
        
        // Renombrar para debug
        obj.name = $"{prefab.name}_Vehicle";

        // Buscar el componente MovingObject existente o agregarlo si no existe
        MovingObject mo = obj.GetComponent<MovingObject>();
        if (mo == null)
        {
            mo = obj.AddComponent<MovingObject>();
        }

        // Habilitar el componente y asignar valores
        mo.enabled = true;
        mo.target = targetPoint;
        mo.speed = Random.Range(minSpeed, maxSpeed);
        
        // Añadir componente de gestión de vida
        VehicleLifetime lifetime = obj.GetComponent<VehicleLifetime>();
        if (lifetime == null)
        {
            lifetime = obj.AddComponent<VehicleLifetime>();
            lifetime.maxLifetime = 90f; // 1.5 minutos máximo
            lifetime.maxDistanceFromPlayer = 250f; // 250 metros del jugador (suficiente para 3 secciones adelante)
        }
        
        // Añadir sistema de faros automático
        VehicleHeadlights headlights = obj.GetComponent<VehicleHeadlights>();
        if (headlights == null)
        {
            headlights = obj.AddComponent<VehicleHeadlights>();
            // Los valores por defecto están bien, pero puedes personalizar aquí:
            // headlights.headlightIntensity = 3f;
            // headlights.headlightRange = 30f;
        }
        
        Debug.Log($"🚗 Vehículo spawneado en {spawnPoint.position}, velocidad: {mo.speed:F2}, target: {targetPoint?.name ?? "NULL"}");
    }

}
