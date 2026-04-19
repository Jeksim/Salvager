using UnityEngine;
using TMPro;

public class PopupUI : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float lifetime = 1.5f;

    private TextMeshProUGUI text;
    private float timer;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // fade out
        timer += Time.deltaTime;
        float t = timer / lifetime;

        if (text != null)
        {
            Color c = text.color;
            c.a = 1f - t;
            text.color = c;
        }

        // destroy
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    // call this when spawning
    public void SetText(string message)
    {
        if (text != null)
        {
            text.text = message;
        }
    }
}