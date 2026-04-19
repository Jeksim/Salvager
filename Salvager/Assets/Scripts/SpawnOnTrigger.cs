using UnityEngine;

public class SpawnOnTrigger : MonoBehaviour
{
    public GameObject objectToSpawn;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned) return;

        // optional: only player
        // if (!other.CompareTag("Player")) return;

        Vector3 spawnPos = new Vector3(
            transform.position.x,
            transform.position.y - 18f,
            transform.position.z
        );

        Instantiate(objectToSpawn, spawnPos, Quaternion.identity);

        hasSpawned = true;
    }
}