using UnityEngine;

public class Gem : MonoBehaviour
{
    public int id;
    public Sprite[] gemSprites;

    public GameObject popUpObject;
    public GameObject moneyCountUIPrefab;

    public int coinAmount = 20;
    public GameObject coinPopupPrefab;
    public Transform coinPopupSpawnPoint;

    [Header("Audio Proximity")]
    public AudioSource audioSource;

    public float range1 = 7f;
    public float volume1 = 0.2f;

    public float range2 = 5f;
    public float volume2 = 0.4f;

    public float range3 = 3f;
    public float volume3 = 0.6f;

    public float range4 = 2f;
    public float volume4 = 0.8f;

    public float range5 = 1f;
    public float volume5 = 1f;

    [Header("Floating")]
    public float moveSpeed = 1f;
    public float moveRange = 1.5f;

    public float bobSpeed = 2f;
    public float bobHeight = 0.25f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private Transform player;

    private Vector3 basePos;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        if (audioSource != null)
            audioSource.volume = 0f;

        startPos = transform.position;
        basePos = transform.position;
        PickNewTarget();
    }
    void Update()
    {
        UpdateAudioVolume();
        HandleFloating();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // GemSpot sprite update
        GameObject gemSpot = GameObject.FindGameObjectWithTag("GemSpot");
        if (gemSpot != null)
        {
            SpriteRenderer sr = gemSpot.GetComponent<SpriteRenderer>();
            if (sr != null && id > 0 && id <= gemSprites.Length)
            {
                sr.sprite = gemSprites[id - 1];
            }
        }

        string gemKey = "gemstone" + id;

        bool firstTime = PlayerPrefs.GetInt(gemKey, 0) != 1;

        // Save collection
        if (firstTime)
        {
            PlayerPrefs.SetInt(gemKey, 1);
            PlayerPrefs.Save();

            if (popUpObject != null)
            {
                Instantiate(popUpObject, transform.position, Quaternion.identity);
            }
        }

        // Call win
        Magnet magnet = other.GetComponent<Magnet>();
        if (magnet != null)
        {
            magnet.Win();
        }

        AddCoins();
        SpawnCoinPopup();
        Instantiate(moneyCountUIPrefab, transform.position, Quaternion.identity);

        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        for (int i = 0; i < gems.Length; i++)
        {
            Destroy(gems[i]);
        }
    }

    void UpdateAudioVolume()
    {
        if (audioSource == null || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= range5)
        {
            audioSource.volume = volume5;
        }
        else if (distance <= range4)
        {
            audioSource.volume = volume4;
        }
        else if (distance <= range3)
        {
            audioSource.volume = volume3;
        }
        else if (distance <= range2)
        {
            audioSource.volume = volume2;
        }
        else if (distance <= range1)
        {
            audioSource.volume = volume1;
        }
        else
        {
            audioSource.volume = 0f;
        }
    }
    void PickNewTarget()
    {
        float randomX = Random.Range(-moveRange, moveRange);
        float randomY = Random.Range(-moveRange, moveRange);

        targetPos = startPos + new Vector3(randomX, randomY, 0f);
    }

    void HandleFloating()
    {
        basePos = Vector3.MoveTowards(
            basePos,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(basePos, targetPos) < 0.1f)
        {
            PickNewTarget();
        }

        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(
            basePos.x,
            basePos.y + bob,
            transform.position.z
        );
    }
    void AddCoins()
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);

        PlayerPrefs.SetInt("LastMoneyHad", currentCoins);

        currentCoins += coinAmount;

        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();
    }

    void SpawnCoinPopup()
    {
        if (coinPopupPrefab == null) return;

        Vector3 spawnPos = transform.position;

        if (coinPopupSpawnPoint != null)
        {
            spawnPos = coinPopupSpawnPoint.position;
        }

        GameObject popupObj = Instantiate(coinPopupPrefab, spawnPos, Quaternion.identity);

        TMPro.TextMeshProUGUI popupText = popupObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (popupText != null)
        {
            popupText.text = "+" + coinAmount + " Coins";
        }
    }
}