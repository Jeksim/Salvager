using System.Collections;
using UnityEngine;
using TMPro;

public class DisableAtDistance : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    public int triggerMeters = 50;

    private TextMeshProUGUI text;
    private Transform player;
    private bool hasFaded = false;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (hasFaded || player == null || text == null) return;

        int meterCount = Mathf.RoundToInt(Mathf.Abs(player.position.y) * 2f);

        if (meterCount >= triggerMeters)
        {
            hasFaded = true;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color startColor = text.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        text.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }
}