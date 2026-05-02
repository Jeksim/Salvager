using UnityEngine;

public class StartArmorChange : MonoBehaviour
{
    public SpriteRenderer[] characterSuitRenderers;
    public Sprite[] characterSuitUpgradedSprites;

    public bool hasUpgrade5 = false;

    void Start()
    {
        UpgradeArmorVisual();
    }

    public void UpgradeArmorVisual()
    {
        hasUpgrade5 = PlayerPrefs.GetInt("PlayerUpgrade5", 0) == 1;

        if (hasUpgrade5)
        {
            for (int i = 0; i < characterSuitRenderers.Length; i++)
            {
                characterSuitRenderers[i].sprite = characterSuitUpgradedSprites[i];
            }
        }

        return;
    }
}
