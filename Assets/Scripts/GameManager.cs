using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor principal del juego - maneja vidas, puntuación y estado del juego
/// </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("Estado del Juego")]
	[SerializeField] private int maxLives = 3;
	[SerializeField] private int currentLives = 3;
	[SerializeField] private int score = 0;
	[SerializeField] private int finalDistance = 0; // Distancia recorrida al morir
	[SerializeField] private bool isGameOver = false;

	[Header("Referencias")]
	[SerializeField] private GameObject gameOverUI; // Panel de Game Over (opcional)
	[SerializeField] private string gameOverSceneName = "Has Perdido"; // Nombre de la escena de Game Over
	[SerializeField] private float gameOverDelay = 1.5f; // Delay antes de cambiar de escena

	[Header("Debug")]
	[SerializeField] private bool showDebug = true;

	// Eventos para que otros scripts se suscriban
	public delegate void GameStateChange();
	public event GameStateChange OnGameOver;
	public event GameStateChange OnLivesChanged;
	public event GameStateChange OnScoreChanged;

	void Awake()
	{
		// Singleton
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		currentLives = maxLives;
		if (gameOverUI != null)
		{
			gameOverUI.SetActive(false);
		}
	}

	/// <summary>
	/// El jugador recibe daño
	/// </summary>
	public void PlayerHit(int damage = 1)
	{
		if (isGameOver) return;

		currentLives -= damage;
		currentLives = Mathf.Max(0, currentLives);

		if (showDebug)
		{
			Debug.Log($"💔 Vidas restantes: {currentLives}/{maxLives}");
		}

		OnLivesChanged?.Invoke();

		if (currentLives <= 0)
		{
			GameOver();
		}
	}

	/// <summary>
	/// Añadir puntos
	/// </summary>
	public void AddScore(int points)
	{
		if (isGameOver) return;

		score += points;
		
		if (showDebug)
		{
			Debug.Log($"🎯 Puntuación: {score}");
		}

		OnScoreChanged?.Invoke();
	}

	/// <summary>
	/// Actualiza la puntuación basada en la distancia
	/// </summary>
	public void UpdateScoreFromDistance()
	{
		if (DistanceTracker.Instance != null)
		{
			// La puntuación es la distancia en metros
			score = DistanceTracker.Instance.GetDistanceMeters();
			OnScoreChanged?.Invoke();
		}
	}

	/// <summary>
	/// Game Over
	/// </summary>
	private void GameOver()
	{
		if (isGameOver) return;

		isGameOver = true;
		
		// Guardar la distancia final antes de cambiar de escena
		if (DistanceTracker.Instance != null)
		{
			finalDistance = DistanceTracker.Instance.GetDistanceMeters();
		}
		
		Debug.Log($"☠️ GAME OVER - Puntuación: {score} | Distancia: {finalDistance}m");

		OnGameOver?.Invoke();

		// Mostrar UI de Game Over si existe (en la misma escena)
		if (gameOverUI != null)
		{
			gameOverUI.SetActive(true);
		}

		// Cargar escena de Game Over después de un delay
		if (!string.IsNullOrEmpty(gameOverSceneName))
		{
			Invoke(nameof(LoadGameOverScene), gameOverDelay);
		}
		else
		{
			// Si no hay escena de Game Over, pausar el juego
			Time.timeScale = 0f;
		}
	}

	/// <summary>
	/// Cargar la escena de Game Over
	/// </summary>
	private void LoadGameOverScene()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(gameOverSceneName);
	}

	/// <summary>
	/// Reiniciar el juego
	/// </summary>
	public void RestartGame()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	/// <summary>
	/// Volver al menú principal
	/// </summary>
	public void LoadMainMenu()
	{
		Time.timeScale = 1f;
		// Cambia "MainMenu" por el nombre de tu escena de menú
		SceneManager.LoadScene("MainMenu");
	}

	// Getters públicos
	public int GetCurrentLives() => currentLives;
	public int GetMaxLives() => maxLives;
	public int GetScore() => score;
	public int GetFinalDistance() => finalDistance;
	public bool IsGameOver() => isGameOver;
}
