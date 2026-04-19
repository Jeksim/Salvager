using UnityEngine;

public class Fish : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Upgrade 3")]
    public float upgrade3MoveSpeed = 1f;

    [Header("Ranges")]
    public float minPatrolX = -4f;
    public float maxPatrolX = 4f;
    public float patrolYRange = 4f;
    public float chaseRange = 4f;
    public float destroyRange = 50f;
    public float pointReachedDistance = 0.2f;

    private Transform player;
    private Vector3 patrolTarget;
    private float startY;

    private bool hasUpgrade3 = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        hasUpgrade3 = PlayerPrefs.GetInt("PlayerUpgrade3", 0) == 1;

        if (hasUpgrade3)
        {
            moveSpeed = upgrade3MoveSpeed;
        }

        startY = transform.position.y;
        PickNewPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer >= destroyRange)
        {
            Destroy(gameObject);
            return;
        }

        // Upgrade 3 = never chase player
        if (!hasUpgrade3 && distanceToPlayer <= chaseRange)
        {
            MoveTowards(player.position);
        }
        else
        {
            MoveTowards(patrolTarget);

            if (Vector2.Distance(transform.position, patrolTarget) <= pointReachedDistance)
            {
                PickNewPatrolPoint();
            }
        }

        UpdateFacing();
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 newPosition = Vector2.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        newPosition.x = Mathf.Clamp(newPosition.x, minPatrolX, maxPatrolX);

        transform.position = newPosition;
    }

    void PickNewPatrolPoint()
    {
        patrolTarget = new Vector3(
            Random.Range(minPatrolX, maxPatrolX),
            startY + Random.Range(-patrolYRange, patrolYRange),
            transform.position.z
        );
    }

    void UpdateFacing()
    {
        if (player == null) return;

        if (transform.position.x < player.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}