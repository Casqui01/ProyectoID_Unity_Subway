using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class SectionTrigger : MonoBehaviour
{

    public GameObject roadSection;

    public Vector3 posicion = new Vector3(30, 30, 0);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            Instantiate(roadSection, posicion, Quaternion.identity);
        }
    }
}
