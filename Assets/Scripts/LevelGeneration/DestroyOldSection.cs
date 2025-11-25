using UnityEngine;

public class DestroyOldSection : MonoBehaviour
{
    private Transform player;
    public float destroyDistance = 50f; // Distancia detrás del jugador para destruir

    void Start()
    {
        // Busca al jugador por tag o nombre
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            // Si no tiene tag Player, busca por el script Move
            playerObj = FindObjectOfType<Move>()?.gameObject;
        }
        
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("⚠️ DestroyOldSection: No se encontró el jugador!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Si el jugador está muy adelante de esta sección, destrúyela
        if (player.position.z - transform.position.z > destroyDistance)
        {
            Debug.Log($"🗑️ Destruyendo sección antigua: {gameObject.name}");
            Destroy(gameObject);
        }
    }
}
