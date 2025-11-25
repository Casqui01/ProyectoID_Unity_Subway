using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;        // El objeto a crear
    public Transform spawnPoint;     // Dónde aparece
    public Transform targetPoint;    // Hacia dónde se mueve
    public float objectSpeed = 5f;   // velocidad configurable para los objetos instanciados

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        GameObject obj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        MovingObject mo = obj.AddComponent<MovingObject>();
        mo.target = targetPoint;
        mo.speed = objectSpeed;
    }

}
