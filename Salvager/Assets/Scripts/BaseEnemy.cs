using UnityEngine;
public class BaseEnemy : MonoBehaviour
{
    protected bool hasHitPlayer = false;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer) return;
        if (!other.CompareTag("Player")) return;

        Magnet magnet = other.GetComponent<Magnet>();
        if (magnet != null)
        {
            magnet.Hit();
            hasHitPlayer = true;
            OnFirstHit();
        }
    }

    protected virtual void OnFirstHit()
    {
        // child classes override this
    }
}