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
        
        Debug.Log($"NPC spawneado con velocidad: {mo.speed}, target: {targetPoint.name}");
    }

}
