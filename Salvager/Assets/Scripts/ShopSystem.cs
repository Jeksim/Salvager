using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public void BuyUpgrade(int upgradeID)
    {
        if (upgradeID <= 0 || upgradeID > 3) return;

        string key = "PlayerUpgrade" + upgradeID;

        // already owned? do nothing
        if (PlayerPrefs.GetInt(key, 0) == 1) return;

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
    public void SpendCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);

        currentCoins -= amount;
        currentCoins = Mathf.Max(0, currentCoins);

        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();
    }
}