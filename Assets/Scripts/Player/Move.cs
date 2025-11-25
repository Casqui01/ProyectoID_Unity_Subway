using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(0,0,4) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            // Destruye la sección completa (el padre del trigger)
            GameObject sectionToDestroy = other.transform.parent?.gameObject ?? other.gameObject;
            Debug.Log($"🗑️ Destruyendo sección: {sectionToDestroy.name}");
            Destroy(sectionToDestroy);
        }
    }
}
