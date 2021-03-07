using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureDataScriptableObject StructureData;
    private float currentHealth;

    void Awake()
    {
        StructuresManager.Instance.AddNewStructure(this);
        currentHealth = StructureData.Health;
    }

    public void GetHit(float damage){
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die(){
        StructuresManager.Instance.RemoveDeadStructure(this);
        GameObject.Destroy(gameObject);
    }
}
