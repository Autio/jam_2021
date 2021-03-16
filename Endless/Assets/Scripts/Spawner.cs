using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour{
    public SpawnerDataScriptableObject SpawnerData;
    private float nextSpawnTime;
    private float nextDifficultyTime;
    private int difficultyLevel = 1;
    private int spawns;

    // The original value is taken from SpawnerData but incremented over time
    private int unitsSpawnedPerTick;
    
    void Awake(){
        nextSpawnTime = GetNextSpawnTime();
        unitsSpawnedPerTick = SpawnerData.UnitsSpawnedPerTick;
        nextDifficultyTime = SpawnerData.ProgessionTime;
    }

    void Update(){
        if(SpawnerData.maxSpawns == 0 || spawns < SpawnerData.maxSpawns)
            {if (Time.time >= nextSpawnTime){
                Spawn();
                nextSpawnTime = GetNextSpawnTime();
            }
            if (Time.time >= nextDifficultyTime)
            {
                unitsSpawnedPerTick += GetNewEnemySpawnAmount();
                difficultyLevel++;
                nextDifficultyTime = GetNextDifficultyIncrementTime();
            }
        }
    }

    private float GetNextSpawnTime(){
        return Time.time + Mathf.Lerp(SpawnerData.MinTickPeriod, SpawnerData.MaxTickPeriod, Random.value);
    }

    private float GetNextDifficultyIncrementTime(){
        return Time.time + Mathf.Lerp(SpawnerData.ProgessionTime * 0.8f, SpawnerData.ProgessionTime * 1.2f, Random.value);
    }

    private int GetNewEnemySpawnAmount()
    {
        if(difficultyLevel <= 5)
        {
            return 1;
        }

        int r = Random.Range(0,100);

        if(r > 66)
        {
            return 5;
        } else if ( r < 66)
        {
            return 3;
        } else if (r < 33)
        {
            return 1;
        }
        return 1;
    }
    private int GetNthFibonacci_Ite(int n = 2)  
    {  
        int number = n - 1; //Need to decrement by 1 since we are starting from 0  
        int[] Fib = new int[number + 1];  
        Fib[0]= 0;  
        Fib[1]= 1;  
        for (int i = 2; i <= number;i++)  
        {  
        Fib[i] = Fib[i - 2] + Fib[i - 1];  
        }  
        return Fib[number];  
    }  

    private void Spawn()
    {
        for (int i = 0; i < unitsSpawnedPerTick; i++)
        {   

            var unitToSpawn = ObjectPooler.Instance.GetPooledCharacter(SpawnerData.CharacterTypesToSpawn[Random.Range(0, SpawnerData.CharacterTypesToSpawn.Length)]);
            // var unitToSpawn = SpawnerData.CharacterTypesToSpawn[Random.Range(0, SpawnerData.CharacterTypesToSpawn.Length)];

            Vector2 randomPositionInCircle = Random.insideUnitCircle * SpawnerData.SpawnRadius;
            var spawnPos = transform.position + new Vector3(randomPositionInCircle.x,0,randomPositionInCircle.y);
            // Move the object from the pool to the right spot and activate
            unitToSpawn.transform.position = spawnPos;
            unitToSpawn.SetActive(true);
        }
        spawns++;
    }
}