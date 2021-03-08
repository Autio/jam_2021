using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem{
    public int amountToPool;
    public GameObject objectToPool;
    public bool shouldExpand = true;

}

public class ObjectPooler : Singleton<ObjectPooler>
{
    // public static ObjectPooler SharedInstance;

    public List<GameObject> pooledObjects; 

    public List<ObjectPoolItem> itemsToPool;


    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
        for (int i = 0; i < item.amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(item.objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
        }
    }

    public GameObject GetPooledCharacter(CharacterBase cb)
    {
        // Return gameobject from the pool if available
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == cb.tag) // Matches enemy types by tag. What's a better way? 
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if(item.objectToPool.tag == tag) {
                if(item.shouldExpand) {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }

        Debug.Log("Nothing returned from pool - Is this type of GameObject allocated in the pool?");
        return null;   
    }

    public GameObject GetPooledObject(GameObject go)
    {
        // Return gameobject from the pool if available
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == go.tag) // Matches enemy types by tag. What's a better way? 
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if(item.objectToPool.tag == tag) {
                if(item.shouldExpand) {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }

        Debug.Log("Nothing returned from pool - Is this type of GameObject allocated in the pool?");
        return null;   
    }

}
