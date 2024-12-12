using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer; 
    public Slider volumeSlider;

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
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        
        SetVolume(savedVolume);
    }

    

    public void SetVolume(float volume)
    {
        // Adjust the volume on the AudioMixer (convert linear value to decibels)
        float dbVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("Master Volume", dbVolume);

        // Save the volume setting
        PlayerPrefs.SetFloat("Master Volume", volume);
    }
}
