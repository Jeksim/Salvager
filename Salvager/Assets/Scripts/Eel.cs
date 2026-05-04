using UnityEngine;

public class Eel : BaseEnemy
{
    public enum MoveDirection
    {
        Left,
        Right
    }

    [Header("Movement Settings")]
    public MoveDirection direction = MoveDirection.Left;
    public float moveDistance = 5f;
    public float moveDuration = 1f;

    [Header("Lifetime")]
    public float delayBeforeDestroy = 1f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer = 0f;
    private bool isMoving = true;

    void Start()
    {
        startPos = transform.position;
        SetTargetFromDirection();
    }

    void Update()
    {
        if (!isMoving) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / moveDuration);

        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (t >= 1f)
        {
            isMoving = false;
            Invoke(nameof(DestroySelf), delayBeforeDestroy);
        }
    }

    void SetTargetFromDirection()
    {
        float dir = (direction == MoveDirection.Left) ? -1f : 1f;
        targetPos = startPos + new Vector3(dir * moveDistance, 0f, 0f);
    }

    void ReverseDirection()
    {
        // Flip enum
        direction = (direction == MoveDirection.Left) ? MoveDirection.Right : MoveDirection.Left;

        // Flip the sprite
        FlipSprite();

        // Restart movement from current position
        startPos = transform.position;
        timer = 0f;
        isMoving = true;

        CancelInvoke(nameof(DestroySelf)); // prevent premature destroy

        SetTargetFromDirection();

        // OPTIONAL: allow multiple hits
        hasHitPlayer = false;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected override void OnFirstHit()
    {
        // Instead of destroying ? reverse!
        ReverseDirection();
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}