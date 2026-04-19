using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource[] sounds;

    public void Play(int index)
    {
        if (sounds == null || sounds.Length == 0) return;

        if (index < 0 || index >= sounds.Length) return;

        sounds[index].Play();
    }
}