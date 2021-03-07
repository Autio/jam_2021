using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour{
    public SpawnerDataScriptableObject SpawnerData;
    private float nextSpawnTime;

    void Awake(){
        nextSpawnTime = GetNextSpawnTime();
    }

    void Update(){
        if (Time.time >= nextSpawnTime){
            Spawn();
        nextSpawnTime = GetNextSpawnTime();
        }
    }

    private float GetNextSpawnTime(){
        return Time.time + Mathf.Lerp(SpawnerData.MinTickPeriod, SpawnerData.MaxTickPeriod, Random.value);
    }

    private void Spawn()
    {
        for (int i = 0; i < SpawnerData.UnitsSpawnedPerTick; i++)
        {   

            var unitToSpawn = ObjectPooler.SharedInstance.GetPooledObject(SpawnerData.CharacterTypesToSpawn[Random.Range(0, SpawnerData.CharacterTypesToSpawn.Length)]);
            // var unitToSpawn = SpawnerData.CharacterTypesToSpawn[Random.Range(0, SpawnerData.CharacterTypesToSpawn.Length)];

            Vector2 randomPositionInCircle = Random.insideUnitCircle * SpawnerData.SpawnRadius;
            var spawnPos = transform.position + new Vector3(randomPositionInCircle.x,0,randomPositionInCircle.y);
            // Move the object from the pool to the right spot and activate
            unitToSpawn.transform.position = spawnPos;
            unitToSpawn.SetActive(true);
        }
    }
}