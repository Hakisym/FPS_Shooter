using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZedSpawner : MonoBehaviour
{
    public GameObject zedPrefab;
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(zedPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
