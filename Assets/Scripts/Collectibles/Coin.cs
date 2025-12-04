using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Rotación")]
    public float rotationSpeed = 180f; // Grados por segundo
    public Vector3 rotationAxis = Vector3.up; // Eje de rotación (Y por defecto)

    [Header("Efecto al recoger")]
    public GameObject sparkleEffect; // Prefab del efecto de brillo (partículas)
    public AudioClip collectSound; // Sonido al recoger (opcional)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Asegurar que el collider sea trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // Rotación continua sobre su propio eje Y (local)
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo el jugador puede recoger la moneda
        if (other.CompareTag("Player") || other.GetComponent<Movement>() != null)
        {
            Collect();
        }
    }

    void Collect()
    {
        // Crear efecto de brillo
        if (sparkleEffect != null)
        {
            GameObject effect = Instantiate(sparkleEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Destruir el efecto después de 2 segundos
        }

        // Reproducir sonido usando AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinSound();
        }
        // Fallback si no existe AudioManager
        else if (collectSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Añadir puntos al jugador
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1);
        }

        // Destruir la moneda
        Destroy(gameObject);
    }
}
