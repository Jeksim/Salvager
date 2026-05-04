using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class DamageMusicSystem : MonoBehaviour
{
    public static DamageMusicSystem instance;

    [Header("Tracks")]
    public AudioSource startTrack;
    public AudioSource track0m;
    public AudioSource hitOnceTrack;
    public AudioSource hitTwiceTrack;
    public AudioSource deathTrack;
    public AudioSource depthsTrack;

    [Header("Depths Music")]
    public int depthsStartMeter = 1300;

    [Header("Depths Behavior")]
    public bool restartDepthsOnTrigger = true;
    private bool depthsTriggered = false;

    [Header("Pitch")]
    public float pitchDropPer50m = 0.1f;
    public float minPitch = 0.1f;

    public bool usePitchChangeSystem = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        PlayAllTracks();
        MuteAllTracks();
        ResetAllPitches();
    }

    void Update()
    {
        if (!usePitchChangeSystem)
        {
            ResetAllPitches();
        }

        UpdateMusic();

        if (usePitchChangeSystem)
        {
            UpdatePitchFromMeters();
        }
    }

    void PlayAllTracks()
    {
        PlayTrack(startTrack);
        PlayTrack(track0m);
        PlayTrack(hitOnceTrack);
        PlayTrack(hitTwiceTrack);
        PlayTrack(deathTrack);
        PlayTrack(depthsTrack);
    }

    void PlayTrack(AudioSource track)
    {
        if (track != null && !track.isPlaying)
        {
            track.loop = true;
            track.pitch = 1f;
            track.Play();
        }
    }

    void MuteAllTracks()
    {
        SetMuted(startTrack, true);
        SetMuted(track0m, true);
        SetMuted(hitOnceTrack, true);
        SetMuted(hitTwiceTrack, true);
        SetMuted(deathTrack, true);
        SetMuted(depthsTrack, true);
    }

    void SetMuted(AudioSource track, bool muted)
    {
        if (track != null)
        {
            track.mute = muted;
        }
    }

    void UnmuteOnly(AudioSource trackToUnmute)
    {
        MuteAllTracks();
        SetMuted(trackToUnmute, false);
    }

    void UpdateMusic()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Start")
        {
            depthsTriggered = false;
            UnmuteOnly(startTrack);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            UnmuteOnly(track0m);
            return;
        }

        Magnet magnet = player.GetComponent<Magnet>();
        if (magnet == null)
        {
            UnmuteOnly(track0m);
            return;
        }

        if (magnet.isDead)
        {
            UnmuteOnly(deathTrack);
            return;
        }

        if (magnet.meterCount >= depthsStartMeter)
        {
            if (!depthsTriggered)
            {
                depthsTriggered = true;

                if (depthsTrack != null && restartDepthsOnTrigger)
                {
                    depthsTrack.Stop();
                    depthsTrack.pitch = 1f;
                    depthsTrack.time = 0f;
                    depthsTrack.Play();
                }
            }
            usePitchChangeSystem = false;

            ResetAllPitches();

            if (depthsTrack != null)
            {
                depthsTrack.pitch = 1f;
            }

            UnmuteOnly(depthsTrack);
            return;
        }

        int remainingDurability = magnet.lightDecreaseAmount - magnet.hitCount;
        bool hasUpgrade2 = PlayerPrefs.GetInt("PlayerUpgrade2", 0) == 1;

        if (usePitchChangeSystem)
        {
            if (hasUpgrade2)
            {
                if (remainingDurability <= 1)
                {
                    UnmuteOnly(hitTwiceTrack);
                }
                else if (remainingDurability <= 3)
                {
                    UnmuteOnly(hitOnceTrack);
                }
                else
                {
                    UnmuteOnly(track0m);
                }
            }
            else
            {
                if (remainingDurability <= 1)
                {
                    UnmuteOnly(hitTwiceTrack);
                }
                else if (remainingDurability == 2)
                {
                    UnmuteOnly(hitOnceTrack);
                }
                else
                {
                    UnmuteOnly(track0m);
                }
            }
        }
    }

    void UpdatePitchFromMeters()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Start")
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Magnet magnet = player.GetComponent<Magnet>();
        if (magnet == null) return;

        if (magnet.isDead || magnet.isWinning) return;

        if (magnet.meterCount >= depthsStartMeter)
        {
            ResetAllPitches();
            return;
        }

        int hundreds = magnet.meterCount / 50;
        float newPitch = 1f - (hundreds * pitchDropPer50m);
        newPitch = Mathf.Max(minPitch, newPitch);

        SetPitch(startTrack, newPitch);
        SetPitch(track0m, newPitch);
        SetPitch(hitOnceTrack, newPitch);
        SetPitch(hitTwiceTrack, newPitch);
        SetPitch(deathTrack, newPitch);
        SetPitch(depthsTrack, newPitch);
    }

    void SetPitch(AudioSource track, float pitch)
    {
        if (track != null)
        {
            track.pitch = pitch;
        }
    }

    public void ResetAllPitches()
    {
        SetPitch(startTrack, 1f);
        SetPitch(track0m, 1f);
        SetPitch(hitOnceTrack, 1f);
        SetPitch(hitTwiceTrack, 1f);
        SetPitch(deathTrack, 1f);
        SetPitch(depthsTrack, 1f);
    }

    public void PlayDeathNow()
    {
        usePitchChangeSystem = true;
        ResetAllPitches();
        UnmuteOnly(deathTrack);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}