using UnityEngine;

public class LevelAudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip musicaDaFase;

    void Start()
    {
        if (audioSource != null && musicaDaFase != null)
        {
            audioSource.clip = musicaDaFase;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}