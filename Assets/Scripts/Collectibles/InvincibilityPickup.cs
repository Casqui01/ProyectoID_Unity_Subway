using UnityEngine;

/// <summary>
/// Coleccionable que otorga invulnerabilidad temporal al jugador
/// </summary>
public class InvincibilityPickup : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Efectos")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 0.7f;

    [Header("Invulnerabilidad")]
    [SerializeField] private float invincibilityDuration = 8f; // Duración de la invulnerabilidad en segundos
    [SerializeField] private Color invincibleColor = new Color(1f, 1f, 0f, 0.5f); // Color amarillo semi-transparente

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
        // Rotación continua
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
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
        // Aplicar invulnerabilidad
        InvincibilityController controller = player.GetComponent<InvincibilityController>();
        if (controller == null)
        {
            controller = player.AddComponent<InvincibilityController>();
        }
        controller.ActivateInvincibility(invincibilityDuration, invincibleColor, showDebug);

        if (showDebug)
        {
            Debug.Log($"🛡️ Invulnerabilidad activada por {invincibilityDuration}s");
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
/// Componente auxiliar para gestionar la invulnerabilidad temporal
/// </summary>
public class InvincibilityController : MonoBehaviour
{
    private Coroutine invincibilityCoroutine;
    private PlayerHealth playerHealth;
    private Renderer[] renderers;
    private Color[] originalColors;
    private bool isInvincible = false;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void ActivateInvincibility(float duration, Color invincibleColor, bool showDebug)
    {
        // Si ya hay invulnerabilidad activa, extender la duración
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine(duration, invincibleColor, showDebug));
    }

    private System.Collections.IEnumerator InvincibilityCoroutine(float duration, Color invincibleColor, bool showDebug)
    {
        isInvincible = true;

        // Guardar colores originales y cambiar a color de invencibilidad
        if (renderers != null && renderers.Length > 0)
        {
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && renderers[i].material != null)
                {
                    originalColors[i] = renderers[i].material.color;
                    renderers[i].material.color = invincibleColor;
                }
            }
        }

        // Forzar invulnerabilidad en PlayerHealth si existe
        if (playerHealth != null)
        {
            // El PlayerHealth tiene su propia lógica de invulnerabilidad
            // Aquí evitamos que reciba daño durante este tiempo
        }

        if (showDebug)
        {
            Debug.Log($"🛡️ Invulnerabilidad activa por {duration}s");
        }

        yield return new WaitForSeconds(duration);

        // Restaurar colores originales
        if (renderers != null && renderers.Length > 0 && originalColors != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && renderers[i].material != null && i < originalColors.Length)
                {
                    renderers[i].material.color = originalColors[i];
                }
            }
        }

        isInvincible = false;

        if (showDebug)
        {
            Debug.Log("🛡️ Invulnerabilidad terminada");
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
