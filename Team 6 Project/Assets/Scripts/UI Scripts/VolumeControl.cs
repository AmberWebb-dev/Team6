using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer; 
    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Master Volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        // Adjust the volume on the AudioMixer (convert linear value to decibels)
        audioMixer.SetFloat("Master Volume", Mathf.Log10(volume) * 20);

        // Save the current volume setting
        PlayerPrefs.SetFloat("Master Volume", volume);
    }
}
