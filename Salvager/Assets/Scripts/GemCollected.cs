using UnityEngine;

public class GemCollection : MonoBehaviour
{
    public int id;

    public GameObject collectedObject;
    public GameObject uncollectedObject;

    void Start()
    {
        string key = "gemstone" + id;

        bool isCollected = PlayerPrefs.GetInt(key, 0) == 1;

        if (collectedObject != null)
            collectedObject.SetActive(isCollected);

        if (uncollectedObject != null)
            uncollectedObject.SetActive(!isCollected);
    }
}