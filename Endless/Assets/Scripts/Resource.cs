using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceDataScriptableObject ResourceData;
    //private float health;
    Health health;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
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
        GameObject.Destroy(gameObject);
    }


}
