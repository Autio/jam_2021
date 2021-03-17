using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "ScriptableObjects/ResourceData", order = 1)]

public class ResourceDataScriptableObject : ScriptableObject
{
    public float Health;
    // What gets spawned when the resource is harvested
    public GameObject lootPrefab;
    public ResourceType Type;
    // How much of the given type does the resource produce when hacked?
    public float yield;

}