using UnityEngine;

/// <summary>
/// Gestiona el audio específico de la escena de juego
/// </summary>
public class GameSceneAudio : MonoBehaviour
{
	[Header("Música del Juego")]
	[Tooltip("Música específica para esta escena (diferente al menú)")]
	[SerializeField] private AudioClip gameMusic;
	
	[Tooltip("¿Reproducir música al iniciar?")]
	[SerializeField] private bool playMusic = true;

	[Header("Ambiente del Juego")]
	[Tooltip("¿Reproducir ambiente urbano al iniciar?")]
	[SerializeField] private bool playUrbanAmbience = true;

	void Start()
	{
		if (AudioManager.Instance != null)
		{
			// Cambiar a la música del juego
			if (playMusic && gameMusic != null)
			{
				AudioManager.Instance.PlayMusic(gameMusic);
			}
			
			// Reproducir ambiente urbano
			if (playUrbanAmbience)
			{
				AudioManager.Instance.PlayUrbanAmbience();
			}
		}
		else
		{
			Debug.LogWarning("⚠️ AudioManager no encontrado en la escena de juego");
		}
	}
}
