using UnityEngine;

public class Swordfish : BaseEnemy
{
    public float moveSpeed = 3f;
    public float minX = -4f;
    public float maxX = 4f;

    private int moveDirection = -1; // -1 = left, 1 = right

    void Start()
    {
        // Randomly choose starting direction
        if (Random.value > 0.5f)
        {
            moveDirection = 1;
            FaceRight();
        }
        else
        {
            moveDirection = -1;
            FaceLeft();
        }
    }

    void Update()
    {
        MoveHorizontal();
    }

    void MoveHorizontal()
    {
        transform.position += Vector3.right * moveDirection * moveSpeed * Time.deltaTime;

        if (transform.position.x <= minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
            moveDirection = 1;
            FaceRight();
        }
        else if (transform.position.x >= maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            moveDirection = -1;
            FaceLeft();
        }
    }

    void FaceLeft()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void FaceRight()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}