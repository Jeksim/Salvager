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

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip coinTickSound;
    public int coinsPerSound = 5;

    public float minPitch = 0.6f;
    public float maxPitch = 2.2f;
    public float pitchRandomness = 0.03f;

    public int coinsPerSoundHigh = 20;
    public int highCoinThreshold = 400;

    void Start()
    {
        RemoveOlderDisplays();

        // Example: trigger counting when this spawns
        int startValue = PlayerPrefs.GetInt("LastMoneyHad", 0);
        int endValue = PlayerPrefs.GetInt("Coins", 0);

        StartCoroutine(CountMoney(startValue, endValue));
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

    IEnumerator CountMoney(int startValue, int endValue)
    {
        int lastValue = startValue;

        float duration = 1.5f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));

            PlayCoinTick(currentValue, lastValue, startValue, endValue);

            lastValue = currentValue;

            if (tmpText != null)
            {
                tmpText.text = "x " + currentValue;
            }

            yield return null;
        }

        if (tmpText != null)
        {
            tmpText.text = "x " + endValue;
        }
    }
    void PlayCoinTick(int currentValue, int lastValue, int startValue, int endValue)
    {
        if (audioSource == null) return;
        if (coinTickSound == null) return;

        //choose step size based on total coins gained
        int totalGain = Mathf.Abs(endValue - startValue);
        int step = totalGain >= highCoinThreshold ? coinsPerSoundHigh : coinsPerSound;

        if (step <= 0) return;

        if (currentValue / step > lastValue / step)
        {
            float progress = 0f;

            if (totalGain > 0)
            {
                progress = (float)Mathf.Abs(currentValue - startValue) / totalGain;
            }

            progress = Mathf.Clamp01(progress);

            float pitch = Mathf.Lerp(minPitch, maxPitch, progress);
            pitch += Random.Range(-pitchRandomness, pitchRandomness);
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(coinTickSound);
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