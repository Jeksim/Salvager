using System.Collections.Generic;
using UnityEngine;

public class FishFinder : MonoBehaviour
{
    public List<SpriteRenderer> fishSpots; // assign 4 spots in order
    public Sprite fishSprite;

    public int fishCount = 0;
    public int maxFish = 4;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Fish")) return;

        if (fishCount >= maxFish) return;

        Destroy(other.gameObject);

        fishCount++;
        UpdateFishSpots();
    }

    void UpdateFishSpots()
    {
        for (int i = 0; i < fishSpots.Count; i++)
        {
            if (i < fishCount)
            {
                fishSpots[i].sprite = fishSprite;
            }
            else
            {
                fishSpots[i].sprite = null;
            }
        }
    }
}