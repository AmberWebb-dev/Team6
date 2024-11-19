using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("----- Audio Sources -----")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource playerAudioSource;

    // Add any new audio clips here!
    [Header("----- Audio Clips -----")]
    public AudioClipWrapper explosionSound;
    public AudioClipWrapper footstepSound;
    public AudioClipWrapper shovelHitSound;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerAudioSource = GameManager.Instance.player.GetComponent<AudioSource>();
        }
    }

    [System.Serializable]
    public class AudioClipWrapper
    {
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField][Range(0, 1)] private float volume;

        public void Play()
        {
            if (audioClips == null || AudioManager.Instance.audioSource == null) { return; }

            AudioManager.Instance.audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], volume);
        }

        public void PlayOnPlayer()
        {
            if (audioClips == null || AudioManager.Instance.playerAudioSource == null) { return; }

            AudioManager.Instance.playerAudioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], volume);
        }
    }
}
