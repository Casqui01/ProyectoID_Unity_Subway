using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

	[Header("Música de Fondo")]
	[SerializeField] private AudioClip backgroundMusic;
	[SerializeField] private AudioSource musicSource;
	[SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;

	[Header("Ambiente Urbano")]
	[SerializeField] private AudioClip urbanAmbience;
	[SerializeField] private AudioSource ambienceSource;
	[SerializeField] [Range(0f, 1f)] private float ambienceVolume = 0.3f;

	[Header("Efectos de Sonido")]
	[SerializeField] private AudioClip coinSound;
	[SerializeField] private AudioSource sfxSource;
	[SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.7f;

	void Awake()
	{
		// Patrón Singleton para acceder desde cualquier lugar
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // Mantener entre escenas
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		ConfigureAudioSources();
	}

	void Start()
	{
		PlayBackgroundMusic();
		PlayUrbanAmbience();
	}

	private void ConfigureAudioSources()
	{
		// Crear AudioSources si no existen
		if (musicSource == null)
		{
			musicSource = gameObject.AddComponent<AudioSource>();
		}
		if (ambienceSource == null)
		{
			ambienceSource = gameObject.AddComponent<AudioSource>();
		}
		if (sfxSource == null)
		{
			sfxSource = gameObject.AddComponent<AudioSource>();
		}

		// Configurar música
		musicSource.loop = true;
		musicSource.playOnAwake = false;
		musicSource.volume = musicVolume;

		// Configurar ambiente
		ambienceSource.loop = true;
		ambienceSource.playOnAwake = false;
		ambienceSource.volume = ambienceVolume;

		// Configurar efectos
		sfxSource.loop = false;
		sfxSource.playOnAwake = false;
		sfxSource.volume = sfxVolume;
	}

	public void PlayBackgroundMusic()
	{
		if (backgroundMusic != null && !musicSource.isPlaying)
		{
			musicSource.clip = backgroundMusic;
			musicSource.Play();
		}
	}

	/// <summary>
	/// Cambia y reproduce una música específica
	/// </summary>
	public void PlayMusic(AudioClip musicClip)
	{
		if (musicClip == null || musicSource == null) return;

		// Detener música actual si está sonando
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}

		// Cambiar y reproducir nueva música
		musicSource.clip = musicClip;
		musicSource.Play();
	}

	public void PlayUrbanAmbience()
	{
		if (urbanAmbience != null && !ambienceSource.isPlaying)
		{
			ambienceSource.clip = urbanAmbience;
			ambienceSource.Play();
		}
	}

	public void PlayCoinSound()
	{
		if (coinSound != null && sfxSource != null)
		{
			sfxSource.PlayOneShot(coinSound);
		}
	}

	public void StopMusic()
	{
		if (musicSource != null)
		{
			musicSource.Stop();
		}
	}

	public void StopAmbience()
	{
		if (ambienceSource != null)
		{
			ambienceSource.Stop();
		}
	}

	public void SetMusicVolume(float volume)
	{
		musicVolume = Mathf.Clamp01(volume);
		if (musicSource != null)
		{
			musicSource.volume = musicVolume;
		}
	}

	public void SetAmbienceVolume(float volume)
	{
		ambienceVolume = Mathf.Clamp01(volume);
		if (ambienceSource != null)
		{
			ambienceSource.volume = ambienceVolume;
		}
	}

	public void SetSFXVolume(float volume)
	{
		sfxVolume = Mathf.Clamp01(volume);
		if (sfxSource != null)
		{
			sfxSource.volume = sfxVolume;
		}
	}
}
