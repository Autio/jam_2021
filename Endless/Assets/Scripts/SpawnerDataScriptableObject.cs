using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "ScriptableObjects/SpawnerData", order = 1)]
public class SpawnerDataScriptableObject : ScriptableObject
{
    public int UnitsSpawnedPerTick;
    public float MinTickPeriod;
    public float MaxTickPeriod;
    public float SpawnRadius;
    public CharacterBase[] CharacterTypesToSpawn;
    

}
