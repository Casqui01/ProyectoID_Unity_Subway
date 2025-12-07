using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    
    private Vector3 targetPosition; // Posición fija del destino
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // Guardar la posición del target INMEDIATAMENTE
        if (target != null)
        {
            targetPosition = target.position;
            isInitialized = true;
            
            // Ya no necesitamos la referencia, romper la conexión
            target = null;
        }
        else
        {
            Debug.LogWarning($"⚠️ MovingObject en {gameObject.name} no tiene target asignado!");
        }
    }

    void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        // Mover hacia el destino guardado
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Cuando llega al destino, destruirse
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
