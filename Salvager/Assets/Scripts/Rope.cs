using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    public Transform topAnchor;
    public Transform magnet;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void LateUpdate()
    {
        if (topAnchor == null || magnet == null) return;

        lr.SetPosition(0, topAnchor.position);
        lr.SetPosition(1, magnet.position);
    }
}