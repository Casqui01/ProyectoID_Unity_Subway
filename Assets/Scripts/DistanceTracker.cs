using UnityEngine;
using TMPro;

/// <summary>
/// Gestiona la distancia recorrida, aceleración progresiva y UI
/// </summary>
public class DistanceTracker : MonoBehaviour
{
	public static DistanceTracker Instance { get; private set; }

	[Header("Distancia")]
	[SerializeField] private float distanceTraveled = 0f;

	[Header("Aceleración")]
	[Tooltip("Velocidad inicial del jugador")]
	[SerializeField] private float startSpeed = 4f;
	
	[Tooltip("Velocidad máxima del jugador")]
	[SerializeField] private float maxSpeed = 12f;
	
	[Tooltip("Cada cuántos metros aumenta la velocidad")]
	[SerializeField] private float speedIncreaseInterval = 50f;
	
	[Tooltip("Cuánto aumenta la velocidad cada intervalo")]
	[SerializeField] private float speedIncreaseAmount = 0.5f;

	[Header("Referencias")]
	[SerializeField] private Movement playerMovement;

	[Header("Debug")]
	[SerializeField] private bool showDebug = false;

	private float currentSpeed;
	private float nextSpeedIncreaseAt;

	void Awake()
	{
		// Singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		// Obtener referencia al jugador si no está asignada
		if (playerMovement == null)
		{
			playerMovement = FindFirstObjectByType<Movement>();
			
			if (playerMovement == null)
			{
				Debug.LogError("⚠️ DistanceTracker: No se encontró el script Movement del jugador!");
			}
		}

		// Inicializar velocidad
		currentSpeed = startSpeed;
		nextSpeedIncreaseAt = speedIncreaseInterval;
	}

	void Update()
	{
		if (playerMovement == null) return;

		// Calcular distancia recorrida (basada en la posición Z del jugador)
		float playerZ = playerMovement.transform.position.z;
		distanceTraveled = Mathf.Max(distanceTraveled, playerZ);

		// Verificar si debe aumentar la velocidad
		if (distanceTraveled >= nextSpeedIncreaseAt && currentSpeed < maxSpeed)
		{
			IncreaseSpeed();
		}
	}

	/// <summary>
	/// Aumenta la velocidad del jugador progresivamente
	/// </summary>
	private void IncreaseSpeed()
	{
		currentSpeed += speedIncreaseAmount;
		currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

		// Aplicar nueva velocidad al jugador
		if (playerMovement != null)
		{
			playerMovement.SetForwardSpeed(currentSpeed);
		}

		// Calcular siguiente punto de aumento
		nextSpeedIncreaseAt += speedIncreaseInterval;

		if (showDebug)
		{
			Debug.Log($"⚡ Velocidad aumentada a {currentSpeed:F1} m/s (próxima en {nextSpeedIncreaseAt:F0}m)");
		}
	}

	/// <summary>
	/// Reinicia la distancia y velocidad (para restart)
	/// </summary>
	public void ResetDistance()
	{
		distanceTraveled = 0f;
		currentSpeed = startSpeed;
		nextSpeedIncreaseAt = speedIncreaseInterval;

		if (playerMovement != null)
		{
			playerMovement.SetForwardSpeed(startSpeed);
		}

		if (showDebug)
		{
			Debug.Log("🔄 Distancia y velocidad reiniciadas");
		}
	}

	// Getters públicos
	public float GetDistance() => distanceTraveled;
	public float GetCurrentSpeed() => currentSpeed;
	public int GetDistanceMeters() => Mathf.FloorToInt(distanceTraveled);
}
