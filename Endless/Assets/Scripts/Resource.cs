using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceDataScriptableObject ResourceData;
    public GameObject lootPrefab;
    //private float health;
    Health health;
    ResourceController.ResourceTypes resourceType;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        resourceType = ResourceController.ResourceTypes.stone; //TODO: FIX
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetHit(float damage){
        health.ModifyHealth(-damage);
        if (health.GetCurrentHealth() <= 0) {
            Die();
        }
    }

    public void Die(){
        // Run an effect

        // Spawn loot up to allowed amount
        GameObject loot = GameObject.Instantiate(lootPrefab, transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
        loot.GetComponent<Loot>().resourceType = resourceType;
        loot.GetComponent<Loot>().amount = (int)ResourceData.yield;
        GameObject.Destroy(gameObject);
    }


}
