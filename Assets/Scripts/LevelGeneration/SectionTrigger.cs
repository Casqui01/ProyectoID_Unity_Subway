using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;
    
    private bool hasSpawned = false;

    private void Start()
    {
        if (roadSection == null)
        {
            Debug.LogError("⚠️ SectionTrigger: No hay prefab asignado en roadSection!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔔 Trigger '{gameObject.name}' detectó: '{other.name}' con tag '{other.tag}' en posición {other.transform.position}");
        
        // Ignora si es otro trigger
        if (other.CompareTag("Trigger"))
        {
            Debug.Log("⚠️ Es otro trigger, ignorando...");
            return;
        }
        
        if (!hasSpawned)
        {
            hasSpawned = true;
            
            // Calcula la posición: 42 unidades adelante en Z desde la sección actual
            Vector3 spawnPosition = new Vector3(
                transform.parent.position.x, 
                transform.parent.position.y, 
                transform.parent.position.z + 42f
            );
            
            Debug.Log($"✅ ¡JUGADOR DETECTADO! Spawneando nueva sección en: {spawnPosition}");
            GameObject newSection = Instantiate(roadSection, spawnPosition, Quaternion.identity);
            Debug.Log($"✅ Nueva sección creada: {newSection.name}");
        }
        else
        {
            Debug.Log("⚠️ Ya spawneó antes, ignorando...");
        }
    }
}
