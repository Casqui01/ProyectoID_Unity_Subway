using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Genera obstáculos aleatorios en una sección de carretera al instanciarse.
/// Añade este script a cada prefab de sección de carretera.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
	[Header("Configuración de Obstáculos")]
	[Tooltip("Prefabs de obstáculos que pueden aparecer")]
	[SerializeField] private GameObject[] obstaclePrefabs;
	
	[Tooltip("Offset en X para centrar obstáculos (ajustar si el pivot no está centrado)")]
	[SerializeField] private float obstacleXOffset = 0f;
	
	[Tooltip("Mínimo número de obstáculos a generar")]
	[SerializeField] private int minObstacles = 2;
	
	[Tooltip("Máximo número de obstáculos a generar")]
	[SerializeField] private int maxObstacles = 4;
	
	[Tooltip("Garantizar distribución equilibrada entre carriles")]
	[SerializeField] private bool balancedDistribution = true;
	
	[Header("Configuración de Carriles")]
	[Tooltip("Número de carriles disponibles")]
	[SerializeField] private int lanes = 3;
	
	[Tooltip("Posiciones X específicas de los carriles")]
	[SerializeField] private float[] lanePositionsX = new float[] { -15.02f, -13.02f, -11.02f };
	
	[Tooltip("Altura Y donde se spawean los obstáculos")]
	[SerializeField] private float spawnHeight = 4.4418f;
	
	[Tooltip("Rotación de los obstáculos en grados (eje Y)")]
	[SerializeField] private float obstacleRotationY = 90f;
	
	[Header("Configuración de Spawn")]
	[Tooltip("Distancia mínima en Z desde el inicio de la sección")]
	[SerializeField] private float minZDistance = 5f;
	
	[Tooltip("Distancia máxima en Z desde el inicio de la sección")]
	[SerializeField] private float maxZDistance = 35f;
	
	[Tooltip("Espaciado mínimo entre obstáculos en Z")]
	[SerializeField] private float minObstacleSpacing = 8f;
	
	[Tooltip("¿Permitir múltiples obstáculos en la misma fila?")]
	[SerializeField] private bool allowMultipleInRow = false;
	
	[Header("Monedas")]
	[Tooltip("Prefab de moneda")]
	[SerializeField] private GameObject coinPrefab;
	
	[Tooltip("Número de monedas a generar")]
	[SerializeField] private int coinsToSpawn = 3;
	
	[Header("Debug")]
	[SerializeField] private bool showDebug = false;
	[SerializeField] private bool generateOnStart = true;

	private List<Vector3> occupiedPositions = new List<Vector3>();

	void Start()
	{
		if (generateOnStart)
		{
			GenerateObstacles();
			GenerateCoins();
		}
	}

	/// <summary>
	/// Genera obstáculos aleatorios en la sección
	/// </summary>
	public void GenerateObstacles()
	{
		if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
		{
			if (showDebug) Debug.LogWarning("⚠️ No hay prefabs de obstáculos asignados");
			return;
		}

		int obstacleCount = Random.Range(minObstacles, maxObstacles + 1);
		
		if (showDebug)
		{
			Debug.Log($"🚧 Generando {obstacleCount} obstáculos en {gameObject.name}");
		}

		List<float> usedZPositions = new List<float>();
		List<int> laneUsageCount = new List<int> { 0, 0, 0 }; // Contador por carril
		int maxAttempts = 50; // Límite de intentos para evitar loops infinitos

		for (int i = 0; i < obstacleCount; i++)
		{
			// Generar posición Z aleatoria con límite de intentos
			float zPos = Random.Range(minZDistance, maxZDistance);
			int attempts = 0;
			
			// Asegurar espaciado mínimo entre obstáculos
			bool validPosition = true;
			while (attempts < maxAttempts)
			{
				validPosition = true;
				foreach (float usedZ in usedZPositions)
				{
					if (Mathf.Abs(zPos - usedZ) < minObstacleSpacing)
					{
						validPosition = false;
						break;
					}
				}
				
				if (validPosition)
				{
					break; // Posición válida encontrada
				}
				
				// Reintentar con nueva posición
				zPos = Random.Range(minZDistance, maxZDistance);
				attempts++;
			}
			
			if (!validPosition)
			{
				if (showDebug) Debug.LogWarning($"⚠️ No se pudo encontrar posición válida después de {maxAttempts} intentos");
				continue; // Saltar este obstáculo
			}

			// Determinar cuántos obstáculos en esta fila
			int obstaclesInRow = allowMultipleInRow ? Random.Range(1, lanes) : 1;
			
			// Seleccionar carriles con distribución equilibrada
			List<int> availableLanes = new List<int>();
			
			if (balancedDistribution)
			{
				// Ordenar carriles por menor uso
				for (int l = 0; l < lanes; l++)
				{
					availableLanes.Add(l);
				}
				availableLanes.Sort((a, b) => laneUsageCount[a].CompareTo(laneUsageCount[b]));
			}
			else
			{
				// Random normal
				for (int l = 0; l < lanes; l++)
				{
					availableLanes.Add(l);
				}
			}

			for (int j = 0; j < obstaclesInRow && availableLanes.Count > 0; j++)
			{
				int laneIndex = Random.Range(0, availableLanes.Count);
				int lane = availableLanes[laneIndex];
				availableLanes.RemoveAt(laneIndex);

				// Usar las posiciones X específicas de los carriles
				float xPos = (lane < lanePositionsX.Length) ? lanePositionsX[lane] : -11f;
				xPos += obstacleXOffset; // Aplicar offset para centrar obstáculos

				// Posición absoluta en el mundo (no relativa a la sección)
				Vector3 spawnPosition = new Vector3(xPos, spawnHeight, transform.position.z + zPos);
				
				// Rotación configurable en el eje Y (por defecto 90 grados)
				Quaternion rotation = Quaternion.Euler(0, obstacleRotationY, 0);
				
				// DEBUG: Descomentar para ver posiciones en consola
				Debug.Log($"Obstáculo spawneado en Carril {lane}: X={xPos:F2}, Y={spawnHeight:F2}, Z={transform.position.z + zPos:F2}");
				
				// Instanciar obstáculo aleatorio con rotación
				GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
				GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, rotation, transform);
				
				occupiedPositions.Add(spawnPosition);
				laneUsageCount[lane]++; // Incrementar contador del carril usado
				
				if (showDebug)
				{
					string[] laneNames = new string[] { "IZQUIERDA", "CENTRO", "DERECHA" };
					Debug.Log($"🚧 Obstáculo spawneado en carril {lane} ({laneNames[lane]}), Z: {zPos:F1}");
				}
			}

			usedZPositions.Add(zPos);
		}
		
		if (showDebug)
		{
			Debug.Log($"📊 Distribución final - Izquierda: {laneUsageCount[0]}, Centro: {laneUsageCount[1]}, Derecha: {laneUsageCount[2]}");
		}
	}

	/// <summary>
	/// Genera monedas en los espacios libres
	/// </summary>
	public void GenerateCoins()
	{
		if (coinPrefab == null)
		{
			if (showDebug) Debug.LogWarning("⚠️ No hay prefab de moneda asignado");
			return;
		}

		for (int i = 0; i < coinsToSpawn; i++)
		{
			float zPos = Random.Range(minZDistance, maxZDistance);
			int lane = Random.Range(0, lanes);
			
			// Usar las posiciones X específicas de los carriles
			float xPos = (lane < lanePositionsX.Length) ? lanePositionsX[lane] : -11f;
			
			Vector3 spawnPosition = new Vector3(xPos, spawnHeight + 1f, transform.position.z + zPos);
			
			// Verificar que no haya obstáculo muy cerca
			bool tooClose = false;
			foreach (Vector3 obsPos in occupiedPositions)
			{
				if (Vector3.Distance(obsPos, spawnPosition) < 3f)
				{
					tooClose = true;
					break;
				}
			}
			
			if (!tooClose)
			{
				GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform);
				
				if (showDebug)
				{
					Debug.Log($"🪙 Moneda spawneada en carril {lane}, Z: {zPos:F1}");
				}
			}
		}
	}

	/// <summary>
	/// Pega un objeto al suelo usando Raycast
	/// </summary>
	private void SnapToGround(GameObject obj, float heightOffset = 0f)
	{
		if (obj == null) return;

		// Hacer raycast desde arriba hacia abajo
		RaycastHit hit;
		Vector3 rayStart = obj.transform.position + Vector3.up * 5f;
		
		if (Physics.Raycast(rayStart, Vector3.down, out hit, 20f))
		{
			// Obtener el tamaño del collider para ajustar la altura
			Collider col = obj.GetComponent<Collider>();
			float objectHeight = 0f;
			
			if (col != null)
			{
				objectHeight = col.bounds.extents.y;
			}
			
			// Posicionar en el suelo + altura del objeto + offset adicional
			obj.transform.position = new Vector3(
				obj.transform.position.x,
				hit.point.y + objectHeight + heightOffset,
				obj.transform.position.z
			);
			
			if (showDebug)
			{
				Debug.Log($"✅ {obj.name} pegado al suelo en Y: {obj.transform.position.y:F2}");
			}
		}
		else
		{
			if (showDebug)
			{
				Debug.LogWarning($"⚠️ No se encontró suelo para {obj.name}");
			}
		}
	}

	/// <summary>
	/// Limpia todos los obstáculos generados (útil para regenerar)
	/// </summary>
	public void ClearObstacles()
	{
		occupiedPositions.Clear();
		
		// Destruir todos los hijos que sean obstáculos
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}
