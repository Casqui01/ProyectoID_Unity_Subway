using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla los botones y funcionalidad del menú principal
/// </summary>
public class MainMenu : MonoBehaviour
{
	[Header("Nombres de Escenas")]
	[Tooltip("Nombre exacto de la escena del juego")]
	[SerializeField] private string gameSceneName = "Game";

	void Start()
	{
		// Asegurar que el juego no esté pausado
		Time.timeScale = 1f;
		
		// Reproducir música del menú si existe AudioManager
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayBackgroundMusic();
		}
	}

	/// <summary>
	/// Inicia el juego (conectar este método al botón Play)
	/// </summary>
	public void PlayGame()
	{
		Debug.Log("🎮 Iniciando juego...");
		
		// Detener música del menú
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.StopMusic();
			AudioManager.Instance.StopAmbience();
		}
		
		SceneManager.LoadScene(gameSceneName);
	}

	/// <summary>
	/// Carga una escena específica por nombre
	/// </summary>
	public void LoadScene(string sceneName)
	{
		Debug.Log($"🎮 Cargando escena: {sceneName}");
		SceneManager.LoadScene(sceneName);
	}

	/// <summary>
	/// Abre el menú de opciones (si tienes una escena de opciones)
	/// </summary>
	public void OpenOptions()
	{
		Debug.Log("⚙️ Abriendo opciones...");
		// SceneManager.LoadScene("Options");
		
		// O si es un panel dentro de la misma escena:
		// optionsPanel.SetActive(true);
		// menuPanel.SetActive(false);
	}

	/// <summary>
	/// Volver al menú principal desde otra escena
	/// </summary>
	public void BackToMainMenu()
	{
		Debug.Log("🏠 Volviendo al menú...");
		
		// Detener audio del juego
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.StopMusic();
			AudioManager.Instance.StopAmbience();
		}
		
		SceneManager.LoadScene("MainMenu");
	}

	/// <summary>
	/// Reiniciar la escena actual
	/// </summary>
	public void RestartScene()
	{
		Debug.Log("🔄 Reiniciando escena...");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	/// <summary>
	/// Salir del juego (conectar al botón Quit)
	/// </summary>
	public void QuitGame()
	{
		Debug.Log("👋 Saliendo del juego...");
		
#if UNITY_EDITOR
		// En el editor, detener el modo Play
		UnityEditor.EditorApplication.isPlaying = false;
#else
		// En la build, cerrar la aplicación
		Application.Quit();
#endif
	}
}
