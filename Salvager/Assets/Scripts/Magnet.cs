using System.Collections;
using UnityEngine;
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

    [Header("Visual Juice")]
    public Transform ropeVisual;
    public Transform magnetVisual;

    public LineRenderer ropeLine;

    public float upgradedWidth = 0.2f;

    public Color upgradedColor = Color.black;
    public Color upgradedColor1 = Color.black;

    public SpriteRenderer Lamp;
    public Sprite LampDefault;
    public Sprite LampUpgrade;

    public float ropeTiltAmount = 8f;
    public float ropeTiltSmooth = 8f;

    public float magnetTiltAmount = 10f;
    public float magnetTiltSmooth = 10f;

    [Header("UI")]
    public TextMeshProUGUI meterText;
    public int meterCount;

    public GameObject durabilityPopupPrefab;
    public Transform durabilityPopupSpawnPoint;

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

        rb.linearVelocity = new Vector2(horizontalInput * adjustedHorizontalSpeed, -currentFallSpeed);
    }

    void UpdateMeters()
    {
        float y = transform.position.y;

        meterCount = Mathf.RoundToInt(Mathf.Abs(y) * 2f);

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
    public void Win()
    {
        if (isDead) return;

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