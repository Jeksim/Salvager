using UnityEngine;

public class ShopIndicator : MonoBehaviour
{
    public GameObject indicator;

    [Header("Prices")]
    public int upgrade1Price;
    public int upgrade2Price;
    public int upgrade3Price;
    public int upgrade4Price;
    public int upgrade5Price;
    public int upgrade6Price;
    public int upgrade8Price;

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        int coins = PlayerPrefs.GetInt("Coins", 0);

        bool hasUpgrade1 = PlayerPrefs.GetInt("PlayerUpgrade1", 0) == 1;
        bool hasUpgrade2 = PlayerPrefs.GetInt("PlayerUpgrade2", 0) == 1;
        bool hasUpgrade3 = PlayerPrefs.GetInt("PlayerUpgrade3", 0) == 1;
        bool hasUpgrade4 = PlayerPrefs.GetInt("PlayerUpgrade4", 0) == 1;
        bool hasUpgrade5 = PlayerPrefs.GetInt("PlayerUpgrade5", 0) == 1;
        bool hasUpgrade6 = PlayerPrefs.GetInt("PlayerUpgrade6", 0) == 1;
        bool hasUpgrade8 = PlayerPrefs.GetInt("PlayerUpgrade8", 0) == 1;


        bool canBuySomething = false;

        // check each upgrade
        if (!hasUpgrade1 && coins >= upgrade1Price)
            canBuySomething = true;

        if (!hasUpgrade2 && coins >= upgrade2Price)
            canBuySomething = true;

        if (!hasUpgrade3 && coins >= upgrade3Price)
            canBuySomething = true;

        if (!hasUpgrade4 && coins >= upgrade4Price)
            canBuySomething = true;

        if (!hasUpgrade5 && coins >= upgrade5Price)
            canBuySomething = true;

        if (!hasUpgrade6 && coins >= upgrade6Price)
            canBuySomething = true;

        if (!hasUpgrade8 && coins >= upgrade8Price)
            canBuySomething = true;

        if (indicator != null)
        {
            indicator.SetActive(canBuySomething);
        }
    }
}