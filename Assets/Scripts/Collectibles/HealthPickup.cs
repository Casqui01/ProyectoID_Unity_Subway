using UnityEngine;

/// <summary>
/// Coleccionable que otorga una vida extra al jugador
/// </summary>
public class HealthPickup : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Efectos")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 0.7f;

    [Header("Valores")]
    [SerializeField] private int healthAmount = 1; // Cantidad de vidas que otorga

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
        Debug.Log($"💚 HealthPickup: Intentando curar al jugador {player.name}");
        
        // Buscar el componente PlayerHealth
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log($"💚 HealthPickup: PlayerHealth encontrado. Vida actual: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
            
            // Dar vida extra
            playerHealth.Heal(healthAmount);

            Debug.Log($"💚 Vida extra recogida! +{healthAmount} vida(s). Nueva vida: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
        }
        else
        {
            Debug.LogWarning("⚠️ HealthPickup: No se encontró PlayerHealth en el jugador");
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
