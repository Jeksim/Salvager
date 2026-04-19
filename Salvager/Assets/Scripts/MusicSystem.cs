using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MusicRange
{
    public int meterThreshold;
    public AudioSource track;
}

[DisallowMultipleComponent]
public class MusicSystem : MonoBehaviour
{
    public static MusicSystem instance;

    [Header("Special Tracks")]
    public AudioSource startTrack;
    public AudioSource deathTrack;

    [Header("Meter Tracks")]
    public List<MusicRange> musicRanges;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayAllTracks();
        MuteAllTracks();
    }

    void Update()
    {
        UpdateMusic();
    }

    void PlayAllTracks()
    {
        PlayTrack(startTrack);
        PlayTrack(deathTrack);

        for (int i = 0; i < musicRanges.Count; i++)
        {
            if (musicRanges[i].track != null)
            {
                PlayTrack(musicRanges[i].track);
            }
        }
    }

    void PlayTrack(AudioSource track)
    {
        if (track != null && !track.isPlaying)
        {
            track.loop = true;
            track.Play();
        }
    }

    void MuteAllTracks()
    {
        SetMuted(startTrack, true);
        SetMuted(deathTrack, true);

        for (int i = 0; i < musicRanges.Count; i++)
        {
            SetMuted(musicRanges[i].track, true);
        }
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
            UnmuteOnly(startTrack);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Magnet magnet = player.GetComponent<Magnet>();
        if (magnet == null) return;

        if (magnet.isDead)
        {
            UnmuteOnly(deathTrack);
            return;
        }

        int meters = magnet.meterCount;

        AudioSource chosenTrack = null;
        int bestThreshold = -1;

        for (int i = 0; i < musicRanges.Count; i++)
        {
            if (musicRanges[i].track != null &&
                meters >= musicRanges[i].meterThreshold &&
                musicRanges[i].meterThreshold > bestThreshold)
            {
                bestThreshold = musicRanges[i].meterThreshold;
                chosenTrack = musicRanges[i].track;
            }
        }

        if (chosenTrack != null)
        {
            UnmuteOnly(chosenTrack);
        }
    }
}