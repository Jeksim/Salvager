using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeterSpriteOverride
{
    public int meterThreshold;
    public Sprite sprite;
}

public class MeterSpriteSetter : MonoBehaviour
{
    public Sprite defaultSprite;
    public List<MeterSpriteOverride> meterOverrides;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.sprite = GetSpriteForCurrentMeter();
    }

    Sprite GetSpriteForCurrentMeter()
    {
        Sprite result = defaultSprite;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return result;

        Magnet magnet = player.GetComponent<Magnet>();
        if (magnet == null) return result;

        int currentMeter = magnet.meterCount;
        int bestThreshold = -1;

        for (int i = 0; i < meterOverrides.Count; i++)
        {
            MeterSpriteOverride currentOverride = meterOverrides[i];

            if (currentMeter >= currentOverride.meterThreshold &&
                currentOverride.meterThreshold > bestThreshold &&
                currentOverride.sprite != null)
            {
                bestThreshold = currentOverride.meterThreshold;
                result = currentOverride.sprite;
            }
        }

        return result;
    }
}