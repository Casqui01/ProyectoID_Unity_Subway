using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("El Transform del jugador que la cámara seguirá")]
    public Transform target;

    [Header("Camera Position")]
    [Tooltip("Offset de posición relativo al jugador")]
    public Vector3 offset = new Vector3(0f, 2f, -5f);

    [Header("Movement Settings")]
    [Tooltip("Suavidad del movimiento de la cámara (mayor = más suave)")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    [Tooltip("Si está marcado, la cámara seguirá solo en ciertos ejes")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    [Header("Rotation Settings")]
    [Tooltip("Si la cámara debe rotar con el jugador")]
    public bool rotateWithTarget = false;
    [Tooltip("Suavidad de la rotación")]
    [Range(0.01f, 1f)]
    public float rotationSmoothSpeed = 0.1f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No se ha asignado un target (jugador)");
            return;
        }

        // Calcular la posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Aplicar restricciones de ejes
        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;
        if (!followZ) desiredPosition.z = transform.position.z;

        // Interpolar suavemente hacia la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Opcional: Rotar con el target
        if (rotateWithTarget)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed);
        }
        else
        {
            // Mantener la cámara mirando al jugador
            transform.LookAt(target);
        }
    }

    // Método para cambiar el offset en tiempo de ejecución
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    // Método para cambiar el target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
