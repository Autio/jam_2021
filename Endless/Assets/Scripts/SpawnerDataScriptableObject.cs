using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "ScriptableObjects/SpawnerData", order = 1)]
public class SpawnerDataScriptableObject : ScriptableObject
{
    // Roughly how much time before the spawner gets more difficult
    public float ProgessionTime;
    public int UnitsSpawnedPerTick;
    // Optional: How many times will the spawner spawn
    public int maxSpawns;
    public float MinTickPeriod;
    public float MaxTickPeriod;
    public float SpawnRadius;
    public CharacterBase[] CharacterTypesToSpawn;
    

}
