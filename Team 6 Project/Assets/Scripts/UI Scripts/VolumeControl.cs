using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer; 
    public Slider volumeSlider;

    private float savedVolume;

    private void Awake()
    {
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Master Volume", 0.75f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(ApplyTemporaryVolume);
        }

        ApplyVolumeToMixer(savedVolume);
    }

    public void ApplyTemporaryVolume(float volume)
    {
        // Temporarily apply the volume to the AudioMixer
        ApplyVolumeToMixer(volume);
    }
    private void ApplyVolumeToMixer(float volume)
    {
        // Convert linear slider value to decibels (-80 to 0)
        float dbVolume = Mathf.Log10(volume) * 20;
        if (volume == 0) { dbVolume = -80.0f; }
        audioMixer.SetFloat("Master Volume", dbVolume);
    }

    public void Accept()
    {
        // Save the current slider value permanently
        float volumeToSave = volumeSlider.value;
        PlayerPrefs.SetFloat("Master Volume", volumeToSave);
        PlayerPrefs.Save(); // Force save to disk
        savedVolume = volumeToSave; // Update the saved volume reference
        Debug.Log("Settings saved!");
    }

    public void Cancel()
    {
        // Revert the slider and AudioMixer to the saved value
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        ApplyVolumeToMixer(savedVolume);
        Debug.Log("Settings reverted!");
    }
}
