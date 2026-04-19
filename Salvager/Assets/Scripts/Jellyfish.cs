using UnityEngine;

public class Jellyfish : BaseEnemy
{
    public float detectionRadius = 9f;
    public float moveUpSpeed = 1f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
        }
    }
}