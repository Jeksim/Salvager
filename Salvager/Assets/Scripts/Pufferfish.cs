using UnityEngine;

public class Pufferfish : BaseEnemy
{
    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite hitSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float moveRadiusX = 1.5f;
    public float moveRadiusY = 1.5f;
    public float pointReachedDistance = 0.1f;

    [Header("World X Limits")]
    public float minX = -4f;
    public float maxX = 4f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform player;

    void Start()
    {
        startPosition = transform.position;
        PickNewTarget();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        FloatAround();
        FacePlayer();
    }

    void FloatAround()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        transform.position = clampedPos;

        if (Vector2.Distance(transform.position, targetPosition) <= pointReachedDistance)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        float targetX = Random.Range(startPosition.x - moveRadiusX, startPosition.x + moveRadiusX);
        float targetY = Random.Range(startPosition.y - moveRadiusY, startPosition.y + moveRadiusY);

        targetX = Mathf.Clamp(targetX, minX, maxX);

        targetPosition = new Vector3(targetX, targetY, transform.position.z);
    }

    void FacePlayer()
    {
        if (player == null) return;

        if (transform.position.x < player.position.x)
        {
            // player is to the right
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            // player is to the left
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    protected override void OnFirstHit()
    {
        if (spriteRenderer != null && hitSprite != null)
        {
            spriteRenderer.sprite = hitSprite;
        }

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}