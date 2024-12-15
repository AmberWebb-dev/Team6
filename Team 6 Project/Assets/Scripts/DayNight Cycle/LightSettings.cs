using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Light Settings", menuName = "Scriptables/Light Settings", order = 1)]

public class LightSettings : ScriptableObject
{
    [SerializeField] public Gradient ambientColour;
    [SerializeField] public Gradient directionalColour;
    [SerializeField] public Gradient fogColour;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
