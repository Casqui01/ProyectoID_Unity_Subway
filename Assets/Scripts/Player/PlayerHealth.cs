using UnityEngine;
using System.Collections;

/// <summary>
/// Componente de salud del jugador con invulnerabilidad temporal
/// </summary>
public class PlayerHealth : MonoBehaviour
{
	[Header("Salud")]
	[SerializeField] private int maxHealth = 3;
	[SerializeField] private int currentHealth = 3;

	[Header("Invulnerabilidad")]
	[SerializeField] private float invulnerabilityDuration = 2f;
	[SerializeField] private float blinkInterval = 0.1f;
	private bool isInvulnerable = false;
	private Coroutine invulnerabilityCoroutine;
	private Coroutine slowdownCoroutine;

	[Header("Efectos de Daño")]
	[SerializeField] private float slowdownFactor = 0.5f; // Reduce velocidad al 50%
	[SerializeField] private float slowdownDuration = 1f;
	[SerializeField] private GameObject hitEffect;
	[SerializeField] private AudioClip hitSound;

	[Header("Debug")]
	[SerializeField] private bool showDebug = true;

	// Referencias privadas (se obtienen automáticamente)
	private Movement movementScript;
	private AudioSource audioSource;
	private Renderer[] renderers;
	private float originalForwardSpeed;

	void Start()
	{
		currentHealth = maxHealth;
		
		// Obtener referencias automáticamente
		movementScript = GetComponent<Movement>();
		if (movementScript == null)
		{
			Debug.LogError("⚠️ PlayerHealth: No se encontró el componente Movement!");
		}
		
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			Debug.LogWarning("⚠️ PlayerHealth: No se encontró AudioSource (opcional)");
		}

		// Obtener todos los renderers para el efecto de parpadeo
		renderers = GetComponentsInChildren<Renderer>();
		if (renderers.Length == 0)
		{
			Debug.LogWarning("⚠️ PlayerHealth: No se encontraron Renderers para efecto de parpadeo");
		}

		// Guardar velocidad original
		if (movementScript != null)
		{
			originalForwardSpeed = movementScript.GetForwardSpeed();
		}
	}

	/// <summary>
	/// El jugador recibe daño
	/// </summary>
	public void TakeDamage(int damage = 1)
	{
		// Verificar invulnerabilidad del InvincibilityController
		InvincibilityController invincibilityController = GetComponent<InvincibilityController>();
		if (invincibilityController != null && invincibilityController.IsInvincible())
		{
			if (showDebug)
			{
				Debug.Log("🛡️ Jugador invulnerable (power-up) - Daño bloqueado");
			}
			return;
		}

		// Si es invulnerable por el sistema normal, ignorar daño
		if (isInvulnerable)
		{
			if (showDebug)
			{
				Debug.Log("🛡️ Jugador invulnerable - Daño bloqueado");
			}
			return;
		}

		// Reducir salud
		currentHealth -= damage;
		currentHealth = Mathf.Max(0, currentHealth);

		if (showDebug)
		{
			Debug.Log($"💔 Jugador golpeado! Vida: {currentHealth}/{maxHealth}");
		}

		// Notificar al GameManager
		if (GameManager.Instance != null)
		{
			GameManager.Instance.PlayerHit(damage);
		}

		// Efectos visuales y de audio
		PlayHitEffects();

		// Activar invulnerabilidad (detener la anterior si existe)
		if (invulnerabilityCoroutine != null)
		{
			StopCoroutine(invulnerabilityCoroutine);
		}
		invulnerabilityCoroutine = StartCoroutine(InvulnerabilityCoroutine());

		// Ralentizar temporalmente (detener la anterior si existe)
		if (slowdownCoroutine != null)
		{
			StopCoroutine(slowdownCoroutine);
		}
		slowdownCoroutine = StartCoroutine(SlowdownCoroutine());

		// Verificar Game Over
		if (currentHealth <= 0)
		{
			Die();
		}
	}

	/// <summary>
	/// Corrutina de invulnerabilidad con efecto de parpadeo
	/// </summary>
	private IEnumerator InvulnerabilityCoroutine()
	{
		isInvulnerable = true;
		float elapsed = 0f;

		if (showDebug)
		{
			Debug.Log($"🛡️ Invulnerabilidad activada por {invulnerabilityDuration}s");
		}

		// Efecto de parpadeo (solo si hay renderers)
		if (renderers != null && renderers.Length > 0)
		{
			bool visible = true;
			while (elapsed < invulnerabilityDuration)
			{
				// Alternar visibilidad
				visible = !visible;
				SetRenderersEnabled(visible);
				
				yield return new WaitForSeconds(blinkInterval);
				elapsed += blinkInterval;
			}

			// Asegurar que el jugador quede visible
			SetRenderersEnabled(true);
		}
		else
		{
			// Si no hay renderers, solo esperar el tiempo de invulnerabilidad
			yield return new WaitForSeconds(invulnerabilityDuration);
		}
		
		isInvulnerable = false;

		if (showDebug)
		{
			Debug.Log("🛡️ Invulnerabilidad terminada");
		}
	}

	/// <summary>
	/// Corrutina de ralentización temporal
	/// </summary>
	private IEnumerator SlowdownCoroutine()
	{
		if (movementScript == null)
		{
			if (showDebug)
			{
				Debug.LogWarning("⚠️ No se puede ralentizar - Movement script no encontrado");
			}
			yield break;
		}

		// Guardar la velocidad actual (puede haber aumentado por el DistanceTracker)
		float currentSpeed = movementScript.GetForwardSpeed();

		// Reducir velocidad
		float slowedSpeed = currentSpeed * slowdownFactor;
		movementScript.SetForwardSpeed(slowedSpeed);

		if (showDebug)
		{
			Debug.Log($"🐌 Velocidad reducida de {currentSpeed:F1} a {slowedSpeed:F1} por {slowdownDuration}s");
		}

		// Esperar
		yield return new WaitForSeconds(slowdownDuration);

		// Restaurar velocidad actual (no la original, sino la que tenía antes del choque)
		if (movementScript != null && this != null)
		{
			movementScript.SetForwardSpeed(currentSpeed);

			if (showDebug)
			{
				Debug.Log($"⚡ Velocidad restaurada a {currentSpeed:F1}");
			}
		}
	}

	/// <summary>
	/// Activar/desactivar todos los renderers
	/// </summary>
	private void SetRenderersEnabled(bool enabled)
	{
		if (renderers == null || renderers.Length == 0) return;

		foreach (Renderer renderer in renderers)
		{
			if (renderer != null)
			{
				renderer.enabled = enabled;
			}
		}
	}

	/// <summary>
	/// Reproducir efectos de impacto
	/// </summary>
	private void PlayHitEffects()
	{
		// Efecto visual
		if (hitEffect != null)
		{
			GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
			Destroy(effect, 2f);
		}

		// Sonido
		if (hitSound != null && audioSource != null)
		{
			audioSource.PlayOneShot(hitSound);
		}
	}

	/// <summary>
	/// Muerte del jugador
	/// </summary>
	private void Die()
	{
		if (showDebug)
		{
			Debug.Log("☠️ Jugador muerto");
		}

		// Detener movimiento
		if (movementScript != null)
		{
			movementScript.enabled = false;
		}

		// Aquí puedes añadir animación de muerte, etc.
	}

	/// <summary>
	/// Curar al jugador
	/// </summary>
	public void Heal(int amount)
	{
		int previousHealth = currentHealth;
		currentHealth += amount;
		currentHealth = Mathf.Min(currentHealth, maxHealth);

		if (showDebug)
		{
			Debug.Log($"💚 Jugador curado! Vida: {previousHealth} → {currentHealth}/{maxHealth}");
		}

		// Notificar al GameManager del cambio de vida
		if (GameManager.Instance != null && currentHealth != previousHealth)
		{
			GameManager.Instance.UpdateLives(currentHealth);
		}
	}

	// Getters públicos
	public int GetCurrentHealth() => currentHealth;
	public int GetMaxHealth() => maxHealth;
	public bool IsInvulnerable() => isInvulnerable;
}
