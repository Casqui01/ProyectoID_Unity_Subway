using UnityEngine;

/// <summary>
/// Script base para obstáculos que causan colisiones con el jugador
/// </summary>
public class Obstacle : MonoBehaviour
{
	[Header("Configuración")]
	[Tooltip("Daño que causa el obstáculo (1 = muerte instantánea por defecto)")]
	[SerializeField] private int damage = 1;
	
	[Tooltip("¿Debe destruirse el obstáculo al chocar?")]
	[SerializeField] private bool destroyOnHit = false;
	
	[Header("Efectos")]
	[SerializeField] private GameObject hitEffect; // Efecto de partículas al chocar
	[SerializeField] private AudioClip hitSound;   // Sonido al chocar
	
	[Header("Debug")]
	[SerializeField] private bool showDebug = false;

	private bool hasCollided = false;

	void Start()
	{
		// Asegurar que tiene un collider configurado como trigger
		Collider col = GetComponent<Collider>();
		if (col == null)
		{
			Debug.LogWarning($"⚠️ Obstáculo {gameObject.name} no tiene Collider! Añadiendo Box Collider...");
			col = gameObject.AddComponent<BoxCollider>();
		}
		
		// Configurar como trigger para no bloquear físicamente al jugador
		col.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		HandleCollision(other.gameObject);
	}

	private void HandleCollision(GameObject other)
	{
		// Evitar múltiples colisiones
		if (hasCollided) return;

		// Solo reacciona al jugador
		if (!other.CompareTag("Player") && other.GetComponent<Movement>() == null)
		{
			return;
		}

		// Verificar si tiene PlayerHealth
		PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
		if (playerHealth == null)
		{
			if (showDebug)
			{
				Debug.LogWarning("⚠️ Jugador no tiene componente PlayerHealth!");
			}
			return;
		}

		// Si el jugador es invulnerable, no hacer daño
		if (playerHealth.IsInvulnerable())
		{
			if (showDebug)
			{
				Debug.Log("🛡️ Jugador invulnerable - Obstáculo ignorado");
			}
			return;
		}

		hasCollided = true;

		if (showDebug)
		{
			Debug.Log($"💥 Jugador chocó con obstáculo: {gameObject.name}");
		}

		// Reproducir efecto visual
		if (hitEffect != null)
		{
			GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
			Destroy(effect, 2f);
		}

		// Reproducir sonido
		if (hitSound != null)
		{
			AudioSource.PlayClipAtPoint(hitSound, transform.position);
		}

		// Aplicar daño al jugador
		playerHealth.TakeDamage(damage);

		// Destruir obstáculo si está configurado
		if (destroyOnHit)
		{
			Destroy(gameObject, 0.1f);
		}
	}
}
