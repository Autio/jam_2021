using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    // Loot contains the resource the player can pick up 
    // Once they have smashed the resource item
    // Maybe later on something more like equipment
    public ResourceType resourceType;
    public int amount;
    private bool collected = false;

    // Add loot amount to the controller
    public void CollectLoot()
    {
        if(!collected){
            ResourceController.Instance.AddResource(resourceType, amount);
        }
        collected = true;
        Destroy(gameObject);
    }

}
