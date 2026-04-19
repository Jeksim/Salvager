using UnityEngine;

public class Radar : MonoBehaviour
{
    public float detectionRadius = 20f;
    public GameObject radarVisual;

    private Transform target;

    void Update()
    {
        FindNearestGem();
        HandleRadarVisibility();
        RotateTowardTarget();
    }

    void FindNearestGem()
    {
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject gem in gems)
        {
            float distance = Vector2.Distance(transform.position, gem.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = gem.transform;
            }
        }

        target = closest;
    }

    void HandleRadarVisibility()
    {
        if (radarVisual == null) return;

        if (target == null)
        {
            radarVisual.SetActive(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= detectionRadius)
        {
            radarVisual.SetActive(true);
        }
        else
        {
            radarVisual.SetActive(false);
        }
    }

    void RotateTowardTarget()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > detectionRadius) return;

        Vector2 direction = transform.position - target.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}