using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class Movement : MonoBehaviour
{
	[Header("Carriles")]
	[SerializeField] private int lanes = 3;                // número de carriles (3)
	[SerializeField] private float laneWidth = 2f;         // distancia entre carriles en unidades
	[SerializeField] private int startLane = 1;            // carril inicial (0 = izquierda)

	[Header("Movimiento")]
	[SerializeField] private float lateralSpeed = 10f;    // velocidad de desplazamiento lateral

	private int currentLane;
	private Vector3 targetPosition;
	private float baseY;
	private float baseZ;
	private float baseX;

	void Start()
	{
		// clamp por seguridad
		lanes = Mathf.Max(1, lanes);
		currentLane = Mathf.Clamp(startLane, 0, lanes - 1);

		baseY = transform.position.y;
		baseZ = transform.position.z;
		baseX = transform.position.x; // usar X inicial como referencia para los carriles

		UpdateTargetPosition();
		// asegúrate de empezar en la posición del carril relativo a baseX
		transform.position = new Vector3(targetPosition.x, baseY, baseZ);
	}

	void Update()
	{
		// Entradas teclado: soporta Input Manager clásico y el nuevo Input System (condicional)
#if ENABLE_LEGACY_INPUT_MANAGER
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
#elif ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
		if (Keyboard.current != null)
		{
			if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) MoveLeft();
			if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) MoveRight();
		}
#else
		// Fallback (intenta el Input clásico)
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
#endif

		// Movimiento suave hacia la posición objetivo
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, lateralSpeed * Time.deltaTime);
	}

	private void MoveLeft()
	{
		if (currentLane <= 0) return; // ya en límite izquierdo
		currentLane--;
		UpdateTargetPosition();
	}

	private void MoveRight()
	{
		if (currentLane >= lanes - 1) return; // ya en límite derecho
		currentLane++;
		UpdateTargetPosition();
	}

	private void UpdateTargetPosition()
	{
		float centerIndex = (lanes - 1) / 2f; // por ejemplo para 3 carriles, centerIndex = 1
		float xOffset = (currentLane - centerIndex) * laneWidth;
		// ahora relativo a la posición inicial X (baseX)
		targetPosition = new Vector3(baseX + xOffset, baseY, baseZ);
	}

	// Método público para mover a un carril específico (0..lanes-1)
	public void MoveToLane(int laneIndex)
	{
		int clamped = Mathf.Clamp(laneIndex, 0, lanes - 1);
		if (clamped == currentLane) return;
		currentLane = clamped;
		UpdateTargetPosition();
	}
}
