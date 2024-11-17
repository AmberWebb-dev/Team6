using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("----- Audio Source -----")]
    [SerializeField] private AudioSource audioSource;

    // Add any new audio clips here!
    [Header("----- Audio Clips -----")]
    public AudioClipWrapper explosionSound;

    private void Awake()
    {
        Instance = this;
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
    }
}
