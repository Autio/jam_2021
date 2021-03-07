using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureDataScriptableObject StructureData;

    // Handle health in the separate component
    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        StructuresManager.Instance.AddNewStructure(this);
        try {
        health.SetCurrentHealth(StructureData.Health);
        health.SetMaxHealth(StructureData.Health);
        } catch{
            Debug.Log("Couldn't set health for " + gameObject.name);
        }
    }

    public void GetHit(float damage){
        health.ModifyHealth(-damage);
        if (health.GetCurrentHealth() <= 0) {
            Die();
        }
    }

    public void Die(){
        StructuresManager.Instance.RemoveDeadStructure(this);
        GameObject.Destroy(gameObject);
    }
}
