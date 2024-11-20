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
    [SerializeField] private GameObject audioSource3DPrefab;

    // Add any new audio clips here!
    [Header("----- Audio Clips -----")]
    public AudioClipWrapper explosionSound;
    public AudioClipWrapper footstepSound;
    public AudioClipWrapper shovelHitSound;
    public AudioClipWrapper playerJumpSound;
    public AudioClipWrapper playerLandSound;
    public AudioClipWrapper playerHurtSound;
    public AudioClipWrapper shovelPickupSound;
    public AudioClipWrapper healSound;
    public AudioClipWrapper shootSound;
    public AudioClipWrapper enemyHitSound;
    public AudioClipWrapper enemyDeathSound;
    public AudioClipWrapper enemyShootSound;
    public AudioClipWrapper cropHitSound;
    public AudioClipWrapper cropDeathSound;

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

        public void PlayAtPoint(Vector3 point)
        {
            if (audioClips == null || AudioManager.Instance.audioSource3DPrefab == null) { return; }

            AudioSource audioSource = Instantiate(AudioManager.Instance.audioSource3DPrefab, point, Quaternion.identity).GetComponent<AudioSource>();
            int clipIndex = Random.Range(0, audioClips.Length);
            audioSource.PlayOneShot(audioClips[clipIndex], volume);

            AudioManager.Instance.StartCoroutine(DestroySource(audioSource, audioClips[clipIndex].length));
        }

        IEnumerator DestroySource(AudioSource audioSource, float duration)
        {
            yield return new WaitForSeconds(duration);

            Destroy(audioSource.gameObject);
        }
    }
}
