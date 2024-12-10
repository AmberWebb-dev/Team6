using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource; // Assign the AudioSource in the Inspector
    [SerializeField] private float fadeDuration = 1f; // Fade duration in seconds

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene changes
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // Reset volume in case the music plays again
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene changes
    }
}