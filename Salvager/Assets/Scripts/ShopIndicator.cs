using UnityEngine;

public class ShopIndicator : MonoBehaviour
{
    public GameObject indicator;

    [Header("Prices")]
    public int upgrade1Price;
    public int upgrade2Price;
    public int upgrade3Price;

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        int coins = PlayerPrefs.GetInt("Coins", 0);

        bool hasUpgrade1 = PlayerPrefs.GetInt("PlayerUpgrade1", 0) == 1;
        bool hasUpgrade2 = PlayerPrefs.GetInt("PlayerUpgrade2", 0) == 1;
        bool hasUpgrade3 = PlayerPrefs.GetInt("PlayerUpgrade3", 0) == 1;

        bool canBuySomething = false;

        // check each upgrade
        if (!hasUpgrade1 && coins >= upgrade1Price)
            canBuySomething = true;

        if (!hasUpgrade2 && coins >= upgrade2Price)
            canBuySomething = true;

        if (!hasUpgrade3 && coins >= upgrade3Price)
            canBuySomething = true;

        if (indicator != null)
        {
            indicator.SetActive(canBuySomething);
        }
    }
}