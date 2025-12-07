using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla todos los elementos de la UI del juego
/// </summary>
public class GameUI : MonoBehaviour
{
	[Header("Textos del HUD")]
	[SerializeField] private TextMeshProUGUI distanceText;
	[SerializeField] private TextMeshProUGUI coinsText;
	[SerializeField] private TextMeshProUGUI livesText;

	[Header("Iconos (Sprites)")]
	[Tooltip("Imagen del icono de moneda (opcional)")]
	[SerializeField] private Image coinIcon;
	
	[Tooltip("Imagen del icono de vida (opcional)")]
	[SerializeField] private Image lifeIcon;

	[Header("Formato")]
	[SerializeField] private bool showDistanceLabel = false;
	[SerializeField] private bool useTextIcons = false; // false = usar sprites, true = usar emojis (requiere fuente con emojis)

	void Start()
	{
		// Inicializar textos
		UpdateAll();
	}

	void Update()
	{
		UpdateAll();
	}

	private void UpdateAll()
	{
		UpdateDistance();
		UpdateCoins();
		UpdateLives();
	}

	private void UpdateDistance()
	{
		if (distanceText == null) return;

		if (DistanceTracker.Instance != null)
		{
			int meters = DistanceTracker.Instance.GetDistanceMeters();
			
			if (showDistanceLabel)
			{
				distanceText.text = $"Distancia: {meters}m";
			}
			else
			{
				distanceText.text = $"{meters}m";
			}
		}
		else
		{
			distanceText.text = "0m";
		}
	}

	private void UpdateCoins()
	{
		if (coinsText == null) return;

		if (GameManager.Instance != null)
		{
			int score = GameManager.Instance.GetScore();
			
			if (useTextIcons)
			{
				// Usar símbolo ASCII simple en lugar de emoji
				coinsText.text = $"$ {score}";
			}
			else if (coinIcon != null)
			{
				// Si hay imagen sprite, solo mostrar número
				coinsText.text = $"{score}";
				coinIcon.gameObject.SetActive(true);
			}
			else
			{
				// Fallback con texto
				coinsText.text = $"$ {score}";
			}
		}
		else
		{
			coinsText.text = useTextIcons ? "$ 0" : "0";
		}
	}

	private void UpdateLives()
	{
		if (livesText == null) return;

		if (GameManager.Instance != null)
		{
			int lives = GameManager.Instance.GetCurrentLives();
			int maxLives = GameManager.Instance.GetMaxLives();
			
			if (useTextIcons)
			{
				// Usar símbolo ASCII simple en lugar de emoji
				livesText.text = $"<3 {lives}/{maxLives}";
			}
			else if (lifeIcon != null)
			{
				// Mostrar imagen + número
				livesText.text = $"{lives}/{maxLives}";
				lifeIcon.gameObject.SetActive(true);
				// Asegurar que la imagen sea visible (opacidad máxima)
				Color color = lifeIcon.color;
				color.a = 1f;
				lifeIcon.color = color;
			}
			else
			{
				// Fallback con texto
				livesText.text = $"HP: {lives}/{maxLives}";
			}
		}
		else
		{
			livesText.text = useTextIcons ? "<3 3/3" : "3/3";
		}
	}
}
