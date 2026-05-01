using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUIDisplayer : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public Image image;

    public float waitBeforeFade = 4f;
    public float fadeDuration = 1f;

    void Start()
    {
        RemoveOlderDisplays();
        StartCoroutine(FadeAndDestroy());
    }

    void RemoveOlderDisplays()
    {
        GameObject[] displays = GameObject.FindGameObjectsWithTag("MoneyDisplay");

        foreach (GameObject display in displays)
        {
            if (display != gameObject)
            {
                Destroy(display);
            }
        }
    }

    IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(waitBeforeFade);

        float timer = 0f;

        Color textColor = tmpText != null ? tmpText.color : Color.white;
        Color imageColor = image != null ? image.color : Color.white;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (tmpText != null)
            {
                Color c = textColor;
                c.a = Mathf.Lerp(textColor.a, 0f, t);
                tmpText.color = c;
            }

            if (image != null)
            {
                Color c = imageColor;
                c.a = Mathf.Lerp(imageColor.a, 0f, t);
                image.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}