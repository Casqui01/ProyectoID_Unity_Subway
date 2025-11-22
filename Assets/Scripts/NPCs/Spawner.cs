using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;        // El objeto a crear
    public Transform spawnPoint;     // Dónde aparece
    public Transform targetPoint;    // Hacia dónde se mueve

    void Start()
    {
        InvokeRepeating("SpawnObject", 0f, 2f); // crea uno cada 2 segundos
    }

    void SpawnObject()
    {
        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        MovingObject mo = obj.AddComponent<MovingObject>();
        mo.target = targetPoint;
        mo.speed = 5f;
    }

}
