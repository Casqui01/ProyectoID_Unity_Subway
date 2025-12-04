using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Controlador para la escena de Game Over
/// Muestra puntuación final y permite volver al menú
/// </summary>
public class GameOverController : MonoBehaviour
{
	[Header("Configuración")]
	[SerializeField] private string menuSceneName = "Menu";
	[SerializeField] private float autoReturnTime = 10f; // Tiempo en segundos para volver automáticamente
	[SerializeField] private bool enableAutoReturn = true;

	[Header("Referencias UI")]
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI distanceText;
	[SerializeField] private TextMeshProUGUI promptText; // Texto de "Presiona cualquier tecla"

	[Header("Animación del texto")]
	[SerializeField] private bool blinkPrompt = true;
	[SerializeField] private float blinkSpeed = 0.5f;

	private float timer = 0f;
	private bool canReturn = true;

	void Start()
	{
		// Asegurar que el tiempo está normal
		Time.timeScale = 1f;

		// Mostrar puntuación final
		DisplayFinalScore();

		// Iniciar parpadeo del texto de prompt
		if (blinkPrompt && promptText != null)
		{
			InvokeRepeating(nameof(BlinkPromptText), 0f, blinkSpeed);
		}
	}

	void Update()
	{
		// Timer para retorno automático
		if (enableAutoReturn)
		{
			timer += Time.deltaTime;
			if (timer >= autoReturnTime)
			{
				ReturnToMenu();
			}
		}

		// Detectar cualquier tecla presionada
		if (canReturn && AnyKeyPressed())
		{
			ReturnToMenu();
		}
	}

	/// <summary>
	/// Muestra la puntuación final del jugador
	/// </summary>
	private void DisplayFinalScore()
	{
		int finalScore = 0;
		int finalDistance = 0;

		// Obtener datos del GameManager (que persiste entre escenas)
		if (GameManager.Instance != null)
		{
			finalScore = GameManager.Instance.GetScore();
			finalDistance = GameManager.Instance.GetFinalDistance();
		}
		else
		{
			Debug.LogWarning("⚠️ GameManager no encontrado para mostrar puntuación");
		}

		// Actualizar textos
		if (scoreText != null)
		{
			scoreText.text = $"Puntuación: {finalScore}";
		}

		if (distanceText != null)
		{
			distanceText.text = $"{finalDistance}m";
		}

		Debug.Log($"📊 Puntuación: {finalScore} | Distancia: {finalDistance}m");
	}

	/// <summary>
	/// Detecta si se ha presionado cualquier tecla
	/// </summary>
	private bool AnyKeyPressed()
	{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
		// Nuevo Input System
		if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
		{
			return true;
		}
		if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
		{
			return true;
		}
		if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame)
		{
			return true;
		}
#else
		// Input Manager clásico
		if (Input.anyKeyDown)
		{
			return true;
		}
#endif
		return false;
	}

	/// <summary>
	/// Anima el texto de prompt (parpadeo)
	/// </summary>
	private void BlinkPromptText()
	{
		if (promptText != null)
		{
			promptText.enabled = !promptText.enabled;
		}
	}

	/// <summary>
	/// Volver al menú principal
	/// </summary>
	public void ReturnToMenu()
	{
		if (!canReturn) return;

		canReturn = false;
		CancelInvoke(); // Detener el parpadeo

		Debug.Log($"🔙 Volviendo al menú: {menuSceneName}");

		// Destruir el GameManager para que se reinicie en la próxima partida
		if (GameManager.Instance != null)
		{
			Destroy(GameManager.Instance.gameObject);
		}

		// Cargar el menú
		SceneManager.LoadScene(menuSceneName);
	}

	/// <summary>
	/// Reintentar el juego (volver a la escena de juego)
	/// </summary>
	public void RetryGame()
	{
		if (!canReturn) return;

		canReturn = false;
		CancelInvoke();

		Debug.Log("🔄 Reintentando juego...");

		// Destruir el GameManager para reiniciar
		if (GameManager.Instance != null)
		{
			Destroy(GameManager.Instance.gameObject);
		}

		// Cargar la escena de juego (asume que es "Ciudad")
		SceneManager.LoadScene("Ciudad");
	}

	void OnDestroy()
	{
		CancelInvoke();
	}
}
