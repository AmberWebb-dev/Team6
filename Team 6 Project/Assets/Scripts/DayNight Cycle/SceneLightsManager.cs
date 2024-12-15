using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]

public class SceneLightsManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightSettings lightSettings;
    [SerializeField, Range(0, 24)] private float timeOfDay;

    public GameObject clouds;

    [SerializeField] private bool isDaytime;

    // Update is called once per frame
    void Update()
    {
        if (lightSettings == null)
        { return; }
        
        if(timeOfDay <= 5.3f || timeOfDay >= 18.6)
        {
            isDaytime = false;
            clouds.SetActive(false);
        }
        else
        {
            isDaytime = true;
            clouds.SetActive(true);
        }

        if (Application.isPlaying)
        {
            float time = Time.deltaTime / 4;
            timeOfDay += time;
            timeOfDay %= 24f;
            UpdateLights(timeOfDay / 24f);
        }
        else
        {
            UpdateLights(timeOfDay / 24f);
        }
    }
    
    private void UpdateLights(float timePercent)
    {
        RenderSettings.ambientLight = lightSettings.ambientColour.Evaluate(timePercent);
        RenderSettings.fogColor = lightSettings.fogColour.Evaluate(timePercent);

        if(directionalLight != null)
        {
            directionalLight.color = lightSettings.directionalColour.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void OnValidate()
    {
        if (directionalLight != null)
        { return; }

        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                directionalLight = light;
                return;
            }
        }
    }
}
