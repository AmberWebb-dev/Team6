using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefManager
{
    private const string VolumeKey = "MasterVolume";

    public static void SaveVolume(float volume)
    {
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
        Debug.Log($"Volume saved: {volume}");
    }

    public static float LoadVolume()
    {
        return PlayerPrefs.GetFloat(VolumeKey, 0.75f);
    }
}
