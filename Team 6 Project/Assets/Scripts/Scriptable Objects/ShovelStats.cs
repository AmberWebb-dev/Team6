using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class ShovelStats : ScriptableObject
{
    public GameObject shovelModel;
    [Range(1, 15)] public int shovelDMG, shovelDist;
    [Range(0.1f, 10.5f)] public float swingRate;
    [Range(1, 50)] public int durability;
}
