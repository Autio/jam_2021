using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceDataScriptableObject ResourceData;
    public GameObject lootPrefab;
    //private float health;
    Health health;
    ResourceType resourceType;
    private bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        health.SetCurrentHealth(ResourceData.Health);
        resourceType = ResourceData.Type; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetHit(float damage){
        Debug.Log("Rock taking " + damage);
        health.ModifyHealth(-damage);
        if (health.GetCurrentHealth() <= 0 && !dead) {
            dead = true;
            Die();
        }
        // Play an effect
    }

    public void Die(){
        // Run an effect
        // Spawn loot up to allowed amount
        for (int i = 0; i < 2; i++)
        {
            GameObject loot = GameObject.Instantiate(lootPrefab, transform.position + new Vector3(0,1.5f,0), Quaternion.identity);
            loot.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-.2f, .2f), Random.Range(.2f, .4f), Random.Range(-.2f, .2f)));
            loot.GetComponent<Loot>().resourceType = resourceType;
            loot.GetComponent<Loot>().amount = (int)ResourceData.yield;
        }
        GameObject.Destroy(gameObject);
        
    }


}
