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

    [SerializeField] private Light playerFlashlight;
    public GameObject flashlightPrefab;
    private GameObject spawnedFlashlight;
    public Vector3 mapMinBounds;
    public Vector3 mapMaxBounds;

    // Update is called once per frame
    void Update()
    {

        bool wasDaytime = isDaytime;

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

        if (wasDaytime != isDaytime)
        {
            if (isDaytime)
            {
                
                TurnOffPlayerFlashlight(); // Use this instead
                DestroyFlashlight();
            }
            else
            {
                SpawnFlashlight(); // Spawn flashlight at night
            }
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

    private void SpawnFlashlight()
    {
        Debug.Log("Attempting to spawn flashlight...");

        // Clean up any previous flashlight if it exists
        if (spawnedFlashlight != null)
        {
            Destroy(spawnedFlashlight);
            spawnedFlashlight = null;
        }

        // Generate a random position within map bounds
        Vector3 randomPosition = GetRandomMapPosition();

        // Spawn the flashlight prefab
        spawnedFlashlight = Instantiate(flashlightPrefab, randomPosition, Quaternion.identity);
        Debug.Log($"Flashlight spawned at: {randomPosition}");
    }

    private void DestroyFlashlight()
    {
        if (spawnedFlashlight != null) // Clean up the flashlight
        {
            if (spawnedFlashlight != null)
            {
                Debug.Log("DestroyFlashlight() called, destroying flashlight...");
                Destroy(spawnedFlashlight);
                spawnedFlashlight = null;
            }
            else
            {
                Debug.Log("DestroyFlashlight() called, but no flashlight exists.");
            }
        }
    }

    private Vector3 GetRandomMapPosition()
    {
        // Generate a random position within the map bounds
        float x = Random.Range(mapMinBounds.x, mapMaxBounds.x);
        float z = Random.Range(mapMinBounds.z, mapMaxBounds.z);
        float y = mapMinBounds.y; // Keep the Y consistent for ground level

        return new Vector3(x, y, z);
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

    private void TurnOffPlayerFlashlight()
    {
        if (playerFlashlight != null)
        {
            playerFlashlight.enabled = false;
            Debug.Log("Player's flashlight turned OFF for the day.");
        }
    }

    private void TurnOnPlayerFlashlight()
    {
        if (playerFlashlight != null)
        {
            playerFlashlight.enabled = true;
            Debug.Log("Player's flashlight turned ON for the night.");
        }
    }
}
