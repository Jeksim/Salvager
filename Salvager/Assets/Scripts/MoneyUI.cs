using System.Collections;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public float countDuration = 2f;

    private Coroutine countRoutine;

    void Start()
    {
        UpdateMoneyCount();
    }

    public void UpdateMoneyCount()
    {
        int oldCoins = PlayerPrefs.GetInt("LastMoneyHad", 0);
        int newCoins = PlayerPrefs.GetInt("Coins", 0);

        if (countRoutine != null)
        {
            StopCoroutine(countRoutine);
        }

        countRoutine = StartCoroutine(CountMoney(oldCoins, newCoins));
    }

    IEnumerator CountMoney(int oldCoins, int newCoins)
    {
        if (moneyText == null) yield break;

        if (oldCoins == newCoins)
        {
            moneyText.text = "x " + newCoins;
            yield break;
        }

        float timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / countDuration);

            int currentCoins = Mathf.RoundToInt(Mathf.Lerp(oldCoins, newCoins, t));
            moneyText.text = "x " + currentCoins;

            yield return null;
        }

        // re-read the actual saved amount at the end
        int finalCoins = PlayerPrefs.GetInt("Coins", 0);
        moneyText.text = "x " + finalCoins;

        PlayerPrefs.SetInt("LastMoneyHad", finalCoins);
        PlayerPrefs.Save();
    }
}