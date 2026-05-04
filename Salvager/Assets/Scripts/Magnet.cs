using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Magnet : MonoBehaviour
{
    [Header("Movement")]
    public float horizontalSpeed = 5f;
    public float fallSpeed = 5f;
    public float speedChange = 1f;

    [Space(15)]

    [Header("Visual Juice")]
    public Transform ropeVisual;
    public Transform magnetVisual;

    public LineRenderer ropeLine;

    public float upgradedWidth = 0.2f;

    public Color upgradedColor = Color.black;
    public Color upgradedColor1 = Color.black;

    [Space(8)]

    public SpriteRenderer Lamp;
    public Sprite LampDefault;
    public Sprite LampUpgrade;

    [Space(8)]

    public float ropeTiltAmount = 8f;
    public float ropeTiltSmooth = 8f;

    public float magnetTiltAmount = 10f;
    public float magnetTiltSmooth = 10f;

    [Space(15)]
    public SpriteRenderer[] characterSuitRenderers;
    public Sprite[] characterSuitUpgradedSprites;

    [Space(15)]

    public GameObject DeepPostProcess;

    [Header("UI")]
    public TextMeshProUGUI meterText;
    public int meterCount;

    public GameObject durabilityPopupPrefab;
    public Transform durabilityPopupSpawnPoint;

    public GameObject[] GemInventorySlots;
    public GameObject[] GemInventoryBubbles;

    [Header("Movement Input Sounds")]
    public AudioSource pressLeftAudioSource;
    public AudioSource pressRightAudioSource;
    public AudioSource pressUpAudioSource;
    public AudioSource pressDownAudioSource;

    [Header("Upgrade 8 Stop")]
    public int stopMovementUpgradeID = 8;

    [Header("Rotation Reset")]
    public float rotationResetSnapAmount = 0.1f;

    [Header("Fish Slowdown")]
    public FishFinder fishFinder;
    public float horizontalSlowPerFish = 0.5f;
    public float horizontalSlowPerFishUpgrade = 0.2f;
    public float verticalSlowPerFish = 0.5f;

    [Header("Light")]
    public Light2D magnetLight;
    public float startLightIntensity = 1.5f;
    public int lightDecreaseAmount = 3;

    [Header("Hit Effects")]
    public Camera cameraScript;
    public float hitFreezeTime = 0.2f;
    public float hitShakeAmount = 0.15f;
    public float hitShakeDuration = 0.2f;

    [Header("Gem Holding")]
    [HideInInspector]
    public int amountToHold = 1;
    [HideInInspector]
    public int currentGemsHeld = 0;

    public List<SpriteRenderer> gemSpots;

    [Header("Gem Capacity Upgrades")]
    public int[] gemCapacityUpgradeIDs;

    [Header("Death")]
    public bool isDead = false;
    public GameObject deathObj;
    public float deathRiseSpeed = 5f;

    public GameObject deadLamp;
    public GameObject deadLampUpgrade;
    public Transform deadLampSpawnPoint;

    private float lightDecreasePerHit;
    private Rigidbody2D rb;

    public int hitCount = 0;
    public bool isWinning = false;

    private bool hasUpgrade1 = false;
    private bool hasUpgrade2 = false;
    private bool hasUpgrade4 = false;
    private bool hasUpgrade5 = false;

    // ADDED
    private bool hasUpgrade8 = false;

    private float meterCountTimesBy = 2;
    private enum VerticalMode
    {
        Normal,
        Up,
        Down
    }

    private VerticalMode currentVerticalMode = VerticalMode.Normal;

    void Awake()
    {
        hasUpgrade1 = PlayerPrefs.GetInt("PlayerUpgrade1", 0) == 1;
        hasUpgrade2 = PlayerPrefs.GetInt("PlayerUpgrade2", 0) == 1;
        hasUpgrade5 = PlayerPrefs.GetInt("PlayerUpgrade5", 0) == 1;

        // ADDED
        hasUpgrade8 = PlayerPrefs.GetInt("PlayerUpgrade" + stopMovementUpgradeID, 0) == 1;

        if (hasUpgrade1)
        {
            horizontalSlowPerFish = horizontalSlowPerFishUpgrade;

            ropeLine.startWidth = upgradedWidth;
            ropeLine.endWidth = upgradedWidth;

            ropeLine.startColor = upgradedColor;
            ropeLine.endColor = upgradedColor1;
        }

        if (hasUpgrade2)
        {
            Lamp.sprite = LampUpgrade;
            lightDecreaseAmount = 5;
            startLightIntensity = 2;
        }

        if (hasUpgrade5)
        {
            meterCountTimesBy = 3f;
            fallSpeed = 6;
            speedChange = 3.5f;

            for (int i = 0; i < characterSuitRenderers.Length; i++)
            {
                characterSuitRenderers[i].sprite = characterSuitUpgradedSprites[i];
            }

        }

        amountToHold = 1;

        foreach (int upgradeID in gemCapacityUpgradeIDs)
        {
            if (PlayerPrefs.GetInt("PlayerUpgrade" + upgradeID, 0) == 1)
            {
                amountToHold++;
            }
        }

        if (amountToHold > 1)
        {
            for (int i = 0; i < GemInventorySlots.Length; i++)
            {
                if (GemInventorySlots[i] != null)
                {
                    GemInventorySlots[i].SetActive(i < amountToHold);
                    GemInventoryBubbles[i].SetActive(i < amountToHold);
                }
            }
        }

        rb = GetComponent<Rigidbody2D>();

        lightDecreasePerHit = startLightIntensity / lightDecreaseAmount;

        if (magnetLight != null)
        {
            magnetLight.intensity = startLightIntensity;
        }
    }

    void Update()
    {
        if (isDead)
        {
            UpdateMeters();
            return;
        }

        // ADDED
        HandleMovementInputSounds();

        HandleVerticalInput();
        HandleVisuals();
        UpdateMeters();
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1f;

        int fishCount = fishFinder != null ? fishFinder.fishCount : 0;

        float adjustedHorizontalSpeed = horizontalSpeed - (fishCount * horizontalSlowPerFish);
        float adjustedFallSpeed = fallSpeed - (fishCount * verticalSlowPerFish);

        adjustedHorizontalSpeed = Mathf.Max(0f, adjustedHorizontalSpeed);
        adjustedFallSpeed = Mathf.Max(0.1f, adjustedFallSpeed);

        float currentFallSpeed = adjustedFallSpeed;

        switch (currentVerticalMode)
        {
            case VerticalMode.Up:
                currentFallSpeed = adjustedFallSpeed - speedChange;
                break;

            case VerticalMode.Down:
                currentFallSpeed = adjustedFallSpeed + speedChange;
                break;

            case VerticalMode.Normal:
                currentFallSpeed = adjustedFallSpeed;
                break;
        }

        if (hasUpgrade8 && currentVerticalMode == VerticalMode.Up)
        {
            rb.linearVelocity = new Vector2(horizontalInput * adjustedHorizontalSpeed, 0f);
            return;
        }

        if (meterCount >= 1300)
        {
            DeepPostProcess.SetActive(true);
        }

        rb.linearVelocity = new Vector2(horizontalInput * adjustedHorizontalSpeed, -currentFallSpeed);
    }
    void HandleMovementInputSounds()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayMovementInputSound(pressLeftAudioSource);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayMovementInputSound(pressRightAudioSource);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayMovementInputSound(pressUpAudioSource);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayMovementInputSound(pressDownAudioSource);
        }
    }

    void PlayMovementInputSound(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        source.Play();
    }
    void UpdateMeters()
    {
        float y = transform.position.y;

        meterCount = Mathf.RoundToInt(Mathf.Abs(y) * meterCountTimesBy);

        if (meterText != null)
        {
            meterText.text = meterCount + "m";
        }
    }

    void HandleVerticalInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            currentVerticalMode = VerticalMode.Up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            currentVerticalMode = VerticalMode.Down;

        bool upHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool downHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (currentVerticalMode == VerticalMode.Up && !upHeld)
        {
            currentVerticalMode = downHeld ? VerticalMode.Down : VerticalMode.Normal;
        }

        if (currentVerticalMode == VerticalMode.Down && !downHeld)
        {
            currentVerticalMode = upHeld ? VerticalMode.Up : VerticalMode.Normal;
        }

        if (!upHeld && !downHeld)
            currentVerticalMode = VerticalMode.Normal;
    }

    void HandleVisuals()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1f;

        if (ropeVisual != null)
        {
            float targetRopeZ = -horizontalInput * ropeTiltAmount;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetRopeZ);

            ropeVisual.localRotation = Quaternion.Lerp(
                ropeVisual.localRotation,
                targetRot,
                Time.deltaTime * ropeTiltSmooth
            );

            // ADDED
            if (horizontalInput == 0f && Quaternion.Angle(ropeVisual.localRotation, Quaternion.identity) <= rotationResetSnapAmount)
            {
                ropeVisual.localRotation = Quaternion.identity;
            }
        }

        if (magnetVisual != null)
        {
            float targetMagnetZ = -horizontalInput * magnetTiltAmount;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetMagnetZ);

            magnetVisual.localRotation = Quaternion.Lerp(
                magnetVisual.localRotation,
                targetRot,
                Time.deltaTime * magnetTiltSmooth
            );

            // ADDED
            if (horizontalInput == 0f && Quaternion.Angle(magnetVisual.localRotation, Quaternion.identity) <= rotationResetSnapAmount)
            {
                magnetVisual.localRotation = Quaternion.identity;
            }
        }
    }

    public void Hit()
    {
        if (isDead) return;

        hitCount++;
        SpawnDurabilityPopup();

        if (magnetLight != null)
        {
            magnetLight.intensity -= lightDecreasePerHit;
            magnetLight.intensity = Mathf.Max(0f, magnetLight.intensity);
        }

        StartCoroutine(HitEffects());

        if (hitCount >= lightDecreaseAmount)
        {
            isDead = true;

            if (DamageMusicSystem.instance != null)
            {
                DamageMusicSystem.instance.PlayDeathNow();
                DamageMusicSystem.instance.ResetAllPitches();
            }

            rb.linearVelocity = Vector2.zero;

            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("death");
            }

            if (cameraScript != null)
            {
                cameraScript.stopVerticalOffset = true;
            }

            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator HitEffects()
    {
        if (cameraScript != null)
        {
            cameraScript.Shake(hitShakeAmount, hitShakeDuration);
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitFreezeTime);
        Time.timeScale = 1f;
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1f);

        float timer = 0f;

        while (timer < 1f)
        {
            transform.position += Vector3.up * deathRiseSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        if (deathObj != null)
        {
            Instantiate(deathObj, transform.position, Quaternion.identity);
        }

        timer = 0f;

        while (timer < 1f)
        {
            transform.position += Vector3.up * deathRiseSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Start");
    }

    public void Win(Sprite gemSprite)
    {
        if (isDead) return;

        currentGemsHeld++;

        if (amountToHold > 1)
        {
            int uiIndex = currentGemsHeld - 1;

            if (uiIndex >= 0 && uiIndex < GemInventorySlots.Length && GemInventorySlots[uiIndex] != null)
            {
                Image img = GemInventorySlots[uiIndex].GetComponent<Image>();

                if (img != null)
                {
                    img.sprite = gemSprite;

                    // ADDED
                    img.SetNativeSize();
                }
            }
        }

        if (currentGemsHeld < amountToHold)
        {
            int spotIndex = currentGemsHeld - 1;

            if (spotIndex >= 0 && spotIndex < gemSpots.Count && gemSpots[spotIndex] != null)
            {
                gemSpots[spotIndex].sprite = gemSprite;
            }

            return;
        }

        GameObject finalGemSpot = GameObject.FindGameObjectWithTag("GemSpot");

        if (finalGemSpot != null)
        {
            SpriteRenderer sr = finalGemSpot.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sprite = gemSprite;
            }
        }

        isDead = true;

        if (DamageMusicSystem.instance != null)
        {
            DamageMusicSystem.instance.PlayDeathNow();
            DamageMusicSystem.instance.ResetAllPitches();
        }

        rb.linearVelocity = Vector2.zero;

        if (cameraScript != null)
        {
            cameraScript.stopVerticalOffset = true;
        }

        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        for (int i = 0; i < gems.Length; i++)
        {
            Destroy(gems[i]);
        }

        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(1f);

        if (DamageMusicSystem.instance != null)
        {
            DamageMusicSystem.instance.ResetAllPitches();
        }

        float timer = 0f;

        while (timer < 1f)
        {
            transform.position += Vector3.up * deathRiseSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        if (deathObj != null)
        {
            Instantiate(deathObj, transform.position, Quaternion.identity);
        }

        timer = 0f;

        while (timer < 1f)
        {
            transform.position += Vector3.up * deathRiseSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Start");
    }

    public void SpawnDeadLamp()
    {
        if (deadLamp == null) return;

        Lamp.sprite = null;

        Vector3 spawnPos = transform.position;

        if (deadLampSpawnPoint != null)
        {
            spawnPos = deadLampSpawnPoint.position;
        }

        if (hasUpgrade2 == true)
        {
            Instantiate(deadLampUpgrade, spawnPos, Quaternion.identity);
        }
        else
        {
            Instantiate(deadLamp, spawnPos, Quaternion.identity);
        }
    }

    void SpawnDurabilityPopup()
    {
        if (durabilityPopupPrefab == null) return;

        Vector3 spawnPos = transform.position;

        if (durabilityPopupSpawnPoint != null)
        {
            spawnPos = durabilityPopupSpawnPoint.position;
        }

        GameObject popupObj = Instantiate(durabilityPopupPrefab, spawnPos, Quaternion.identity);

        TextMeshProUGUI popupText = popupObj.GetComponentInChildren<TextMeshProUGUI>();
        if (popupText != null)
        {
            int remainingDurability = Mathf.Max(0, lightDecreaseAmount - hitCount);
            popupText.text = "Durability\n" + remainingDurability + "/" + lightDecreaseAmount;
        }
    }
}