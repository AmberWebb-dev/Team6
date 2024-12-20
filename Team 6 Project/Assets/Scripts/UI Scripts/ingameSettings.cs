using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ingameSettings : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    private float savedVolume;

    private void Start()
    {
        // Load the saved volume value and set it on the slider
        savedVolume = PlayerPrefs.GetFloat("Master Volume", 0.75f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(ApplyTemporaryVolume);
        }

        ApplyVolumeToMixer(savedVolume);
    }

    // Function for the Settings button in the Pause Menu
    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false); // Hide Pause Menu
        settingsPanel.SetActive(true);  // Show Settings Panel
    }

    // Function for the Save button in the Settings Menu
    public void SaveSettings()
    {
        // Save the current slider value permanently
        float volumeToSave = volumeSlider.value;
        PlayerPrefs.SetFloat("Master Volume", volumeToSave);
        PlayerPrefs.Save(); // Force save to disk
        savedVolume = volumeToSave; // Update the saved volume reference
        Debug.Log("Settings saved!");

        // Close the settings panel and reopen the pause menu
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    // Function for the Cancel button in the Settings Menu
    public void CancelSettings()
    {
        // Revert the slider and AudioMixer to the saved value
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        ApplyVolumeToMixer(savedVolume);
        Debug.Log("Settings changes cancelled!");

        // Close the settings panel and reopen the pause menu
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
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
}

