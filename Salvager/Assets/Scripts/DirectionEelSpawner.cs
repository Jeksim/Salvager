using System.Collections;
using TMPro;
using UnityEngine;

public class DirectionalEelSpawner : MonoBehaviour
{
    [Header("Detection")]
    public float playerDetectionRadius = 5f;
    public string playerTag = "Player";

    [Header("Warnings")]
    public GameObject warningGameObjectLeft;
    public GameObject warningGameObjectRight;
    public GameObject warningSounds;

    [Header("TMP Text")]
    public TMP_Text tmpTextLeft;
    public TMP_Text tmpTextRight;

    [Header("Spawner Points")]
    public Transform leftSpawner;
    public Transform rightSpawner;

    [Header("Eel Prefabs")]
    public GameObject leftEelPrefab;
    public GameObject rightEelPrefab;

    [Header("Warning Pulse")]
    public float pulseSpeed = 8f;
    public float pulseAmount = 0.2f;

    private bool spawnIsLeft;
    private bool hasSpawnBeenCalled;

    private Vector3 warningLeftOriginalScale;
    private Vector3 warningRightOriginalScale;

    private void Awake()
    {
        LockXPosition();
    }

    private void Start()
    {
        spawnIsLeft = Random.value < 0.5f;

        warningLeftOriginalScale = warningGameObjectLeft.transform.localScale;
        warningRightOriginalScale = warningGameObjectRight.transform.localScale;

        warningGameObjectLeft.SetActive(spawnIsLeft);
        warningGameObjectRight.SetActive(!spawnIsLeft);

        SetupTMP(tmpTextLeft, spawnIsLeft);
        SetupTMP(tmpTextRight, !spawnIsLeft);
    }

    private void Update()
    {
        LockXPosition();

        if (hasSpawnBeenCalled)
            return;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= playerDetectionRadius)
        {
            hasSpawnBeenCalled = true;
            Spawn();
        }
    }

    private void LateUpdate()
    {
        LockXPosition();
    }

    private void LockXPosition()
    {
        transform.position = new Vector3(0f, transform.position.y, transform.position.z);
    }

    private void SetupTMP(TMP_Text text, bool isCorrectDirection)
    {
        if (text == null)
            return;

        text.text = isCorrectDirection ? "..." : "";

        Color color = text.color;
        color.a = isCorrectDirection ? 1f : 0f;
        text.color = color;
    }

    public void Spawn()
    {
        warningSounds.SetActive(true);
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        GameObject activeWarning = spawnIsLeft ? warningGameObjectLeft : warningGameObjectRight;
        Vector3 originalScale = spawnIsLeft ? warningLeftOriginalScale : warningRightOriginalScale;

        string[] countdown = { "3", "2", "1", "" };

        foreach (string number in countdown)
        {
            if (tmpTextLeft != null)
                tmpTextLeft.text = number;

            if (tmpTextRight != null)
                tmpTextRight.text = number;

            float timer = 0f;

            while (timer < 0.3f)
            {
                timer += Time.deltaTime;

                if (activeWarning != null)
                {
                    float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                    activeWarning.transform.localScale = originalScale * pulse;
                }

                yield return null;
            }
        }

        if (activeWarning != null)
            activeWarning.transform.localScale = originalScale;

        if (spawnIsLeft)
        {
            if (leftEelPrefab != null && leftSpawner != null)
                Instantiate(leftEelPrefab, leftSpawner.position, leftSpawner.rotation);
        }
        else
        {
            if (rightEelPrefab != null && rightSpawner != null)
                Instantiate(rightEelPrefab, rightSpawner.position, rightSpawner.rotation);
        }

        warningGameObjectLeft.SetActive(false);
        warningGameObjectRight.SetActive(false);

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}