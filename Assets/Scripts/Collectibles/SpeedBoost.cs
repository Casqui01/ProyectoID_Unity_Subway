using UnityEngine;

/// <summary>
/// Coleccionable que otorga un boost temporal de velocidad al jugador
/// </summary>
public class SpeedBoost : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Efectos")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 0.7f;

    [Header("Boost de Velocidad")]
    [SerializeField] private float speedMultiplier = 2f; // Multiplica la velocidad actual
    [SerializeField] private float boostDuration = 5f; // Duración del boost en segundos

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    void Start()
    {
        // Asegurar que el collider sea trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // Rotación continua horizontal
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo el jugador puede recoger
        if (other.CompareTag("Player") || other.GetComponent<Movement>() != null)
        {
            Collect(other.gameObject);
        }
    }

    void Collect(GameObject player)
    {
        // Buscar el componente Movement
        Movement movement = player.GetComponent<Movement>();
        if (movement != null)
        {
            // Aplicar boost de velocidad
            float currentSpeed = movement.GetForwardSpeed();
            float boostedSpeed = currentSpeed * speedMultiplier;
            movement.SetForwardSpeed(boostedSpeed);

            if (showDebug)
            {
                Debug.Log($"⚡ Speed Boost activado! Velocidad: {currentSpeed:F1} → {boostedSpeed:F1} por {boostDuration}s");
            }

            // Iniciar corrutina para restaurar velocidad después del tiempo
            SpeedBoostController controller = player.GetComponent<SpeedBoostController>();
            if (controller == null)
            {
                controller = player.AddComponent<SpeedBoostController>();
            }
            controller.ApplyBoost(currentSpeed, boostDuration, showDebug);
        }
        else
        {
            Debug.LogWarning("⚠️ SpeedBoost: No se encontró Movement en el jugador");
        }

        // Crear efecto visual
        if (collectEffect != null)
        {
            GameObject effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Reproducir sonido
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
        }

        // Destruir el coleccionable
        Destroy(gameObject);
    }
}

/// <summary>
/// Componente auxiliar para gestionar el boost de velocidad temporal
/// </summary>
public class SpeedBoostController : MonoBehaviour
{
    private Coroutine boostCoroutine;

    public void ApplyBoost(float originalSpeed, float duration, bool showDebug)
    {
        // Si ya hay un boost activo, detenerlo
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }

        boostCoroutine = StartCoroutine(BoostCoroutine(originalSpeed, duration, showDebug));
    }

    private System.Collections.IEnumerator BoostCoroutine(float originalSpeed, float duration, bool showDebug)
    {
        yield return new WaitForSeconds(duration);

        // Restaurar velocidad original
        Movement movement = GetComponent<Movement>();
        if (movement != null)
        {
            movement.SetForwardSpeed(originalSpeed);

            if (showDebug)
            {
                Debug.Log($"⚡ Speed Boost terminado. Velocidad restaurada a {originalSpeed:F1}");
            }
        }
    }
}
