using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RockSpawnOverride
{
    public int meterThreshold;
    public List<GameObject> rocks;
    public List<GameObject> gems;
    public int minSpawn = 2;
    public int maxSpawn = 4;
}

public class RockSpawn : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> rocks;
    public List<GameObject> gems;

    public List<RockSpawnOverride> meterOverrides;

    public int minSpawn = 2;
    public int maxSpawn = 4;

    public float minDistance = 1f;

    [Range(0f, 1f)]
    public float gemSpawnChance = 0.1f;

    void Start()
    {
        SpawnRocks();
        TrySpawnGem();
    }

    void SpawnRocks()
    {
        List<GameObject> activeRockList = rocks;
        List<GameObject> activeGemList = gems;
        int activeMinSpawn = minSpawn;
        int activeMaxSpawn = maxSpawn;

        GetSpawnSettingsForCurrentMeter(ref activeRockList, ref activeGemList, ref activeMinSpawn, ref activeMaxSpawn);

        if (spawnPoints.Count == 0 || activeRockList.Count == 0) return;

        int spawnCount = Random.Range(activeMinSpawn, activeMaxSpawn + 1);

        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        List<Vector3> usedPositions = new List<Vector3>();

        int safety = 100;

        while (spawnCount > 0 && availablePoints.Count > 0 && safety > 0)
        {
            safety--;

            int index = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[index];
            Vector3 pos = point.position;

            bool valid = true;

            foreach (Vector3 used in usedPositions)
            {
                if (Vector3.Distance(pos, used) < minDistance)
                {
                    valid = false;
                    break;
                }
            }

            availablePoints.RemoveAt(index);

            if (!valid) continue;

            GameObject rock = activeRockList[Random.Range(0, activeRockList.Count)];
            Instantiate(rock, pos, Quaternion.identity);

            usedPositions.Add(pos);
            spawnCount--;
        }
    }

    void TrySpawnGem()
    {
        if (Random.value > gemSpawnChance) return;
        if (spawnPoints.Count == 0) return;

        List<GameObject> activeRockList = rocks;
        List<GameObject> activeGemList = gems;
        int activeMinSpawn = minSpawn;
        int activeMaxSpawn = maxSpawn;

        GetSpawnSettingsForCurrentMeter(ref activeRockList, ref activeGemList, ref activeMinSpawn, ref activeMaxSpawn);

        if (activeGemList == null || activeGemList.Count == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject gem = activeGemList[Random.Range(0, activeGemList.Count)];

        Instantiate(gem, point.position, Quaternion.identity);
    }

    void GetSpawnSettingsForCurrentMeter(ref List<GameObject> resultRocks, ref List<GameObject> resultGems, ref int resultMinSpawn, ref int resultMaxSpawn)
    {
        GameObject magnetObject = GameObject.FindGameObjectWithTag("Player");
        if (magnetObject == null) return;

        Magnet magnet = magnetObject.GetComponent<Magnet>();
        if (magnet == null) return;

        int currentMeter = magnet.meterCount;
        int bestThreshold = -1;

        for (int i = 0; i < meterOverrides.Count; i++)
        {
            RockSpawnOverride currentOverride = meterOverrides[i];

            if (currentMeter >= currentOverride.meterThreshold &&
                currentOverride.meterThreshold > bestThreshold)
            {
                bestThreshold = currentOverride.meterThreshold;

                if (currentOverride.rocks != null && currentOverride.rocks.Count > 0)
                {
                    resultRocks = currentOverride.rocks;
                }

                if (currentOverride.gems != null && currentOverride.gems.Count > 0)
                {
                    resultGems = currentOverride.gems;
                }

                resultMinSpawn = currentOverride.minSpawn;
                resultMaxSpawn = currentOverride.maxSpawn;
            }
        }
    }
}