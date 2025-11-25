using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    void Update()
    {
        // Mover hacia el destino
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Cuando llega, destruirlo
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
